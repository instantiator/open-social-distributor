using DistributorLib.Post;
using DistributorLib.Post.Assigners;
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
            : base(NetworkType.Facebook, code, "facebook", PostFormatVariantFactory.Facebook, ImageAssignerVariantFactory.Facebook)
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
        var imageResponses = new List<FacebookPostResponse>();

        try
        {
            for (int t = 0; t < texts.Count(); t++)
            {
                var text = texts.ElementAt(t);
                if (!DryRunPosting)
                {
                    if (t < images.Count())
                    {
                        foreach (var image in images.ElementAt(t))
                        {
                            var imgRequest = new RestRequest($"/{actorId}/photos", Method.Post);
                            imgRequest.AddParameter("access_token", token);
                            imgRequest.AddParameter("published", false);
                            imgRequest.AddParameter("temporary", true);
                            if (image.SourceUri.IsFile)
                            {
                                imgRequest.AddFile("source", image.AbsoluteLocalPath!);
                            }
                            else
                            {
                                imgRequest.AddParameter("url", image.SourceUri.ToString());
                            }
                            var response = await graphClient!.ExecuteAsync(imgRequest);
                            if (!response.IsSuccessful) throw new Exception($"Could not upload image for post {t}", new Exception(response.Content));
                            var fb_response = JsonConvert.DeserializeObject<FacebookPostResponse>(response.Content!);
                            imageResponses.Add(fb_response!);
                        }
                    }

                    var first = responses.Count() == 0;
                    if (first)
                    {
                        // first post is a real post
                        var request = new RestRequest($"/{actorId}/feed", Method.Post);
                        request.AddJsonBody(new 
                        {
                            message = text,
                            access_token = token,
                            link = link?.ToStringFor(NetworkType),
                            published = true,
                            attached_media = imageResponses.Select(r => new { media_fbid = r.id })
                        });

                        var response = await graphClient!.ExecuteAsync(request);
                        if (!response.IsSuccessful) throw new Exception($"Could not post status {t}", new Exception(response.Content));
                        var fb_response = JsonConvert.DeserializeObject<FacebookPostResponse>(response.Content!);
                        responses.Add(new Tuple<RestResponse, FacebookPostResponse>(response, fb_response!));
                    }
                    else
                    {
                        // all subsequent posts are comments
                        // seems unlikely you'd post something long enough to exceed the facebook character limit, I guess...
                        var postId = responses.First().Item2.id!;
                        var request = new RestRequest($"/{postId}/comments", Method.Post);
                        request.AddJsonBody(new 
                        {
                            message = text,
                            access_token = token,
                            published = true,
                            attached_media = imageResponses.Select(r => new { media_fbid = r.id })
                        });
                        var response = await graphClient!.ExecuteAsync(request);
                        if (!response.IsSuccessful) throw new Exception($"Could not post comment {t}", new Exception(response.Content));
                        var fb_response = JsonConvert.DeserializeObject<FacebookPostResponse>(response.Content!);
                        responses.Add(new Tuple<RestResponse, FacebookPostResponse>(response, fb_response!));
                    }
                }
            }
        }
        catch (Exception e)
        {
            var eids = responses.Select(r => r.Item2.id);
            var errs = responses.Where(r => !r.Item1.IsSuccessful).Select(r => r.Item2.error?.message);
            var imgerrs = imageResponses.Where(r => r.error != null).Select(r => r.error!.message);
            var allerrs = errs.Concat(imgerrs);
            return new PostResult(this, message, false, eids, string.Join('\n', allerrs), e);
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