using DistributorLib.Post;
using DistributorLib.Post.Formatters;
using Newtonsoft.Json;
using RestSharp;

namespace DistributorLib.Network.Implementations;

// Get a page access token: https://developers.facebook.com/docs/pages/access-tokens#get-a-page-access-token
// Publish a page post: https://developers.facebook.com/docs/pages/publishing#publish-a-page-post

public class FacebookNetwork : AbstractNetwork
{
    private static string FB_URL = "https://graph.facebook.com";

    public enum Mode { Page, User }

    public string? pageId, pageToken;
    public string? userToken;
    public Mode mode;

    private RestClient? graphClient;
    
    public FacebookNetwork(string code, Mode mode, 
        string? userToken = null, 
        string? pageId = null, string? pageToken = null) 
            : base(NetworkType.Facebook, code, "facebook", PostFormatVariantFactory.Facebook)
    {
        this.mode = mode;
        this.pageToken = pageToken;
        this.pageId = pageId;
        this.userToken = userToken;
    }

    protected override async Task InitClientAsync()
    {
        graphClient = new RestClient(FB_URL);

        if (userToken == null && pageToken == null)
        {
            throw new ArgumentException("Cannot initialise FacebookNetwork without a user or page access token");
        }

        if (mode == Mode.Page && pageToken == null) 
        {
            // TODO: get the page access token using the user access token
            // See: https://developers.facebook.com/docs/pages/access-tokens#get-a-page-access-token
            throw new NotImplementedException("TODO: get page access token from user access token");
        }
    }

    protected override async Task DisposeClientAsync()
    {
        graphClient?.Dispose();
        graphClient = null;
    }

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message)
    {
        var texts = Formatter.FormatText(message);
        var link = Formatter.GetLink(message);
        var responses = new List<Tuple<RestResponse, FacebookPostResponse>>();
        foreach (var text in texts)
        {
            // TODO: ensure that each post is a response to the previous
            // link is not in the text
            switch (mode)
            {
                case Mode.Page:
                    if (responses.Count() == 0)
                    {
                        // first post is a real post
                        var request = new RestRequest($"/{pageId}/feed", Method.Post);
                        request.AddParameter("message", text);
                        request.AddParameter("access_token", pageToken);
                        if (link != null) { request.AddParameter("link", link); }
                        var response = await graphClient!.ExecuteAsync(request);
                        var fb_response = JsonConvert.DeserializeObject<FacebookPostResponse>(response.Content!);
                        responses.Add(new Tuple<RestResponse, FacebookPostResponse>(response, fb_response!));
                    }
                    else
                    {
                        // all subsequent posts are comments
                        // if you post something long enough to exceed the post character limit, good luck to you
                        var postId = responses.First().Item2.id!;
                        var request = new RestRequest($"/{postId}/comments", Method.Post);
                        request.AddParameter("message", text);
                        request.AddParameter("access_token", pageToken);
                        var response = await graphClient!.ExecuteAsync(request);
                        var fb_response = JsonConvert.DeserializeObject<FacebookPostResponse>(response.Content!);
                        responses.Add(new Tuple<RestResponse, FacebookPostResponse>(response, fb_response!));
                    }
                    break;
                case Mode.User:
                    throw new NotImplementedException("TODO: user posts not implemented");
                default:
                    throw new NotImplementedException($"{mode} not supported");
            }
        }
        var aok = responses.All(r => r.Item1.IsSuccessful && !string.IsNullOrWhiteSpace(r.Item2.id));
        var errors = responses.Where(r => !r.Item1.IsSuccessful).Select(r => r.Item1.ErrorMessage);
        return new PostResult(this, message, aok, string.Join('\n', errors));
    }

    protected override async Task<ConnectionTestResult> TestConnectionImplementationAsync()
    {
        switch (mode)
        {
            case Mode.Page:
                var request = new RestRequest($"/me?fields=id,name");
                request.AddParameter("access_token", pageToken);
                var response = await graphClient!.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    return new ConnectionTestResult(this, true, "Connection successful");
                }
                else
                {
                    return new ConnectionTestResult(this, false, response.ErrorMessage ?? response.Content);
                }
            case Mode.User:
                throw new NotImplementedException("TODO: user posts not implemented");
            default:
                throw new NotImplementedException($"{mode} not supported");
        }
    }

    private class FacebookPostResponse
    {
        public string? id { get; set; }
        public FacebookPostResponseError? error { get; set; }

        public class FacebookPostResponseError
        {
            public string? message { get; set; }
            public string? type { get; set; }
            public int? code { get; set; }
            public string? fbtrace_id { get; set; }
        }
    }

}