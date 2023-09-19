using DistributorLib.Post;
using DistributorLib.Post.Formatters;
using DistributorLib.Post.Images;
using Newtonsoft.Json;
using RestSharp;

namespace DistributorLib.Network.Implementations;

// Publish a page post: https://developers.facebook.com/docs/pages/publishing#publish-a-page-post

public class FacebookNetwork : AbstractNetwork
{
    private static string FB_URL = "https://graph.facebook.com/v18.0/";

    public enum Mode { Page }

    public string? actorId, token;
    public string? userToken;
    public Mode mode;

    private RestClient? graphClient;
    
    public FacebookNetwork(string code, Mode mode, 
        string? token = null, 
        string? actorId = null) 
            : base(NetworkType.Facebook, code, "facebook", PostFormatVariantFactory.Facebook)
    {
        this.mode = mode;
        this.token = token;
        this.actorId = actorId;
    }

    protected override async Task InitClientAsync()
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Cannot initialise FacebookNetwork without a page access token");
        }
        graphClient = new RestClient(FB_URL);
    }

    protected override async Task DisposeClientAsync()
    {
        graphClient?.Dispose();
        graphClient = null;
    }

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message, IEnumerable<string> texts, IEnumerable<IEnumerable<ISocialImage>> images)
    {
        var link = message.Link;
        var responses = new List<Tuple<RestResponse, FacebookPostResponse>>();
        foreach (var text in texts)
        {
            if (!DryRunPosting)
            {
                var first = responses.Count() == 0;
                if (first)
                {
                    // first post is a real post
                    var request = new RestRequest($"/{actorId}/feed", Method.Post);
                    request.AddParameter("message", text);
                    request.AddParameter("access_token", token);
                    if (link != null) { request.AddParameter("link", link.ToStringFor(NetworkType)); }

                    var response = await graphClient!.ExecuteAsync(request);

                    var fb_response = JsonConvert.DeserializeObject<FacebookPostResponse>(response.Content!);
                    responses.Add(new Tuple<RestResponse, FacebookPostResponse>(response, fb_response!));
                }
                else
                {
                    // all subsequent posts are comments
                    // seems unlikely you'd post something long enough to exceed the facebook character limit, I guess...
                    var postId = responses.First().Item2.id!;
                    var request = new RestRequest($"/{postId}/comments", Method.Post);
                    request.AddParameter("message", text);
                    request.AddParameter("access_token", token);
                    var response = await graphClient!.ExecuteAsync(request);
                    var fb_response = JsonConvert.DeserializeObject<FacebookPostResponse>(response.Content!);
                    responses.Add(new Tuple<RestResponse, FacebookPostResponse>(response, fb_response!));
                }
            }
        }

        var aok = responses.All(r => r.Item1.IsSuccessful && !string.IsNullOrWhiteSpace(r.Item2.id));
        var ids = responses.Select(r => r.Item2.id);
        var errors = responses.Where(r => !r.Item1.IsSuccessful).Select(r => r.Item2.error?.message);
        return new PostResult(this, message, aok, ids, string.Join('\n', errors));
    }

    protected override async Task<ConnectionTestResult> TestConnectionImplementationAsync()
    {
        var request = new RestRequest($"/me?fields=id,name");
        request.AddParameter("access_token", token);
        var response = await graphClient!.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            var fb_me = JsonConvert.DeserializeObject<FacebookMeResponse>(response.Content!);
            return new ConnectionTestResult(this, true, fb_me?.id, response.Content);
        }
        else
        {
            var fb_me_err = JsonConvert.DeserializeObject<FacebookMeResponse>(response.Content!);
            return new ConnectionTestResult(this, false, null, response.ErrorMessage ?? fb_me_err?.error?.message ?? response.Content);
        }
    }

    protected override IEnumerable<IEnumerable<ISocialImage>> AssignImages(ISocialMessage message, int posts)
    {
        return AssignImagesToFirstPost(message, posts);
    }


    private class FacebookMeResponse
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public FacebookPostResponseError? error { get; set; }
    }

    private class FacebookPostResponse
    {
        public string? id { get; set; }
        public FacebookPostResponseError? error { get; set; }
    }

    public class FacebookPostResponseError
    {
        public string? message { get; set; }
        public string? type { get; set; }
        public int? code { get; set; }
        public string? fbtrace_id { get; set; }
    }
}