using System.Net.Http.Headers;
using System.Web;
using DistributorLib.Network;
using DistributorLib.Post;
using DistributorLib.Post.Assigners;
using DistributorLib.Post.Formatters;
using DistributorLib.Post.Images;
using Newtonsoft.Json;
using RestSharp;

namespace DistributorLib.Network.Implementations;

// See: https://learn.microsoft.com/en-us/linkedin/marketing/integrations/community-management/shares/posts-api
public class LinkedInNetwork : AbstractNetwork
{
    private readonly string LINKEDIN_API_REST_BASE = "https://api.linkedin.com/rest";
    private readonly string LINKEDIN_INTROSPECTION_BASE = "https://www.linkedin.com/oauth/v2";

    public enum Mode { Org, User }

    private Mode mode;
    private string clientId;
    private string clientSecret;
    private string token;
    private string authorId;

    private RestClient? client;

    public LinkedInNetwork(string shortcode, Mode mode, string clientId, string clientSecret, string token, string authorId) 
        : base(NetworkType.LinkedIn, shortcode, "linkedin", PostFormatVariantFactory.LinkedIn, ImageAssignerVariantFactory.LinkedIn)
    {
        this.mode = mode;
        this.token = token;
        this.authorId = authorId;
        this.clientId = clientId;
        this.clientSecret = clientSecret;
    }

    protected override async Task InitClientAsync()
    {
        client = new RestClient(LINKEDIN_API_REST_BASE);
    }

    protected override async Task DisposeClientAsync()
    {
        client?.Dispose();
        client = null;
    }

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message, IEnumerable<string> texts, IEnumerable<IEnumerable<ISocialImage>> images)
    {
        var responses = new List<Tuple<RestResponse, LinkedInResponse>>();
        var author = mode == Mode.Org ? $"urn:li:organization:{authorId}" : $"urn:li:person:{authorId}";
        for (int t = 0; t < texts.Count(); t++)
        {
            var text = texts.ElementAt(t);
            if (!DryRunPosting)
            {
                var first = responses.Count() == 0;
                if (first)
                {
                    object? imagesContent = null;
                    var imagesData = new List<LinkedInImageData>();
                    if (t < images.Count())
                    {
                        foreach (var image in images.ElementAt(t))
                        {
                            var initData = await InitialiseUploadAsync(image, author);
                            imagesData.Add(initData);
                            await UploadImageAsync(image, initData.uploadUrl);
                            await PollUntilImageAvailableAsync(initData.id);
                        } // each image
                    } 

                    var postRequest = new RestRequest("/posts", Method.Post);
                    postRequest.AddHeader("Authorization", $"Bearer {token}");
                    postRequest.AddHeader("Content-Type", "application/json");
                    postRequest.AddHeader("X-Restli-Protocol-Version", "2.0.0");
                    postRequest.AddHeader("LinkedIn-Version","202309");
                    imagesContent = PrepareContent(imagesData);
                    var postRequestContent = new 
                    { 
                        author = author, 
                        commentary = text,
                        visibility = "PUBLIC",
                        distribution = new 
                        {
                            feedDistribution = "MAIN_FEED",
                            targetEntities = new object[] { },
                            thirdPartyDistributionChannels = new object[] { }
                        },
                        content = imagesContent,
                        contentLandingPage = message.Link?.ToStringFor(NetworkType) ?? "",
                        lifecycleState = "PUBLISHED",
                        isReshareDisabledByAuthor = false
                    };
                    postRequest.AddJsonBody(postRequestContent);
                    var postResponse = await client!.ExecuteAsync(postRequest);
                    var postIdUrn = postResponse.Headers!.SingleOrDefault(h => h.Name=="x-linkedin-id" || h.Name=="x-restli-id")?.Value?.ToString();
                    var postIsError = postResponse.Headers!.SingleOrDefault(h => h.Name=="x-restli-error-response")?.Value?.ToString() == "true";
                    var postAok = postResponse.IsSuccessful && !postIsError && !string.IsNullOrWhiteSpace(postIdUrn);
                    var postHeaders = postResponse.Headers!.Select(h => new Tuple<string,string?>(h.Name!, h.Value?.ToString()));
                    var postLiResponse = new LinkedInResponse() { Success = postAok, Id = postIdUrn, Content = postResponse.Content, Headers = postHeaders };
                    if (!postAok) throw new Exception($"Cannot post message {t}", new Exception(NetworkDebugHelper.Summarise(postResponse)));
                    responses.Add(new Tuple<RestResponse, LinkedInResponse>(postResponse, postLiResponse));
                }
                else
                {
                    var firstId = responses.First().Item2.Id;
                    if (!string.IsNullOrWhiteSpace(firstId))
                    {
                        var request = new RestRequest($"/socialActions/{firstId}/comments", Method.Post);
                        request.AddHeader("Authorization", $"Bearer {token}");
                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("X-Restli-Protocol-Version", "2.0.0");
                        request.AddHeader("LinkedIn-Version","202309");
                        var content = new 
                        { 
                            actor = author, 
                            @object = firstId,
                            message = new { text = text },
                            content = new object[] { }
                        };
                        request.AddJsonBody(content);
                        var response = await client!.ExecuteAsync(request);
                        var idUrn = response.Headers!.SingleOrDefault(h => h.Name=="x-linkedin-id" || h.Name=="x-restli-id")?.Value?.ToString();
                        var isError = response.Headers!.SingleOrDefault(h => h.Name=="x-restli-error-response")?.Value?.ToString() == "true";
                        var aok = response.IsSuccessful && !isError && !string.IsNullOrWhiteSpace(idUrn);
                        var headers = response.Headers!.Select(h => new Tuple<string,string?>(h.Name!, h.Value?.ToString()));
                        var liResponse = new LinkedInResponse() { Success = aok, Id = idUrn, Content = response.Content, Headers = headers };
                        responses.Add(new Tuple<RestResponse, LinkedInResponse>(response, liResponse));
                    }
                    else
                    {
                        throw new Exception("Cannot post comments - main post does not have an id.");
                    }
                }
            }
        }

        var all_ok = responses.All(r => r.Item2.Success);
        var ids = responses.Select(r => r.Item2.Id);
        var i = 0;
        var errorData = string.Join('\n', responses
            .Where(r => !r.Item2.Success)
            .Select(r => DescribeErrors(r.Item1, ++i))
        );
        return new PostResult(this, message, all_ok, ids, error: errorData);
    }

    private object? PrepareContent(IEnumerable<LinkedInImageData> imagesData)
    {
        if (imagesData.Count() == 1) return new 
        {
            media = new
            {
                image = new { id = imagesData.Single().id, altText = imagesData.Single().description }
            }
        };

        if (imagesData.Count() > 1) return new 
        {
            multiImage = new
            {
                images = imagesData.Select(i => new { id = i.id, altText = i.description })
            }
        };

        return null;
    }

    private async Task PollUntilImageAvailableAsync(string imageId, TimeSpan? delay = null, int maxAttempts = 24)
    {
        delay = delay ?? TimeSpan.FromSeconds(5);
        int attempts = 0;
        bool imageOk = false;
        do
        {
            Thread.Sleep((int)delay.Value.TotalMilliseconds);
            var pollRequest = new RestRequest($"images/{imageId}", Method.Get);
            pollRequest.AddHeader("Authorization", $"Bearer {token}");
            pollRequest.AddHeader("LinkedIn-Version","202309");
            var pollResponse = await client!.ExecuteAsync(pollRequest);
            var pollIsError = !pollResponse.IsSuccessful || pollResponse.Headers!.Any(h => h.Name=="x-restli-error-response");
            var pollData = JsonConvert.DeserializeObject<LinkedInPollResponse>(pollResponse.Content!);
            imageOk = pollData?.status == "AVAILABLE";
            var imageFailed = pollData?.status == "PROCESSING_FAILED";
            if (imageFailed) { throw new Exception($"Image status for post: {pollData?.status}", new Exception(NetworkDebugHelper.Summarise(pollRequest, null) + NetworkDebugHelper.Summarise(pollResponse))); }
            if (++attempts > maxAttempts) throw new Exception($"Cannot poll image status for post - too many attempts");
        } while (!imageOk);
    }

    private async Task<LinkedInImageData> InitialiseUploadAsync(ISocialImage image, string author)
    {
        var imgRequest = new RestRequest($"images?action=initializeUpload", Method.Post);
        imgRequest.AddHeader("Authorization", $"Bearer {token}");
        imgRequest.AddHeader("Content-Type", "application/json");
        imgRequest.AddHeader("X-Restli-Protocol-Version", "2.0.0");
        imgRequest.AddHeader("LinkedIn-Version","202309");
        imgRequest.AddJsonBody(new { initializeUploadRequest = new { owner = author }});
        var imgResponse = await client!.ExecuteAsync(imgRequest);
        var imgIsError = imgResponse.Headers!.SingleOrDefault(h => h.Name=="x-restli-error-response")?.Value?.ToString() == "true";
        var imgAok = imgResponse.IsSuccessful && !imgIsError;
        var imgHeaders = imgResponse.Headers!.Select(h => new Tuple<string,string?>(h.Name!, h.Value?.ToString()));
        if (!imgAok) throw new Exception($"Cannot init image upload for post", new Exception(NetworkDebugHelper.Summarise(imgResponse)));
        var imgResponseData = JsonConvert.DeserializeObject<LinkedInImageResponse>(imgResponse.Content!);
        return new LinkedInImageData(imgResponseData!.value.image, image.Description, imgResponseData!.value.uploadUrl);
    }

    private async Task UploadImageAsync(ISocialImage image, string uploadUrl)
    {
        using (var uploadClient = new HttpClient())
        {
            uploadClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var imageDataStream = await image.GetStreamAsync();
            var uploadContent = new StreamContent(imageDataStream);
            var uploadResponse = await uploadClient.PutAsync(uploadUrl, uploadContent);
            if (!uploadResponse.IsSuccessStatusCode) throw new Exception($"Cannot upload image", new Exception(NetworkDebugHelper.Summarise(uploadResponse)));
        }
    }

    private string DescribeErrors(RestResponse response, int? index = null)
    {
        var prefix = index.HasValue ? $"{index} - " : "";
        return string.Join('\n', new[] {
            $"{prefix}{response.ResponseStatus}, response: {response.StatusCode}",
            response.Request.Resource,
            response.Content,
            string.Join('\n', response.Headers?.Select(h => $"{h.Name}: {h.Value}") ?? new string[0])
        });
    }

    public class LinkedInResponse
    {
        public bool Success { get; set; }
        public string? Id { get; set; }
        public string? Content {get;set;}
        public IEnumerable<Tuple<string,string?>>? Headers { get; set; }
    }

    protected override async Task<ConnectionTestResult> TestConnectionImplementationAsync()
    {
        using (var introspectionClient = new RestClient(LINKEDIN_INTROSPECTION_BASE))
        {
            // introspect the token
            var tokenIntrospectionRequest = new RestRequest($"/introspectToken", Method.Post);
            tokenIntrospectionRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            tokenIntrospectionRequest.AddParameter("token", token);
            tokenIntrospectionRequest.AddParameter("client_id", clientId);
            tokenIntrospectionRequest.AddParameter("client_secret", clientSecret);
            var tokenIntrospectionResponse = await introspectionClient!.ExecuteAsync(tokenIntrospectionRequest);
            var introspection = JsonConvert.DeserializeObject<LinkedInIntrospectionResponse>(tokenIntrospectionResponse.Content!);
            var tokenIntrospectionOk = 
                tokenIntrospectionResponse.IsSuccessful && 
                introspection!.active == true && 
                introspection!.status == "active" &&
                ((mode == Mode.Org && (introspection!.scope?.Contains("w_organization_social") ?? false)) ||
                (mode == Mode.User && (introspection!.scope?.Contains("w_member_social") ?? false)));
            if (!tokenIntrospectionOk)
            {
                return new ConnectionTestResult(this, false, null, DescribeErrors(tokenIntrospectionResponse));
            }

            // fetch the user or organisation profile
            var meRequest = new RestRequest("/me", Method.Get);
            meRequest.AddHeader("Authorization", $"Bearer {token}");
            meRequest.AddHeader("LinkedIn-Version", "202309");
            var meResponse = await client!.ExecuteAsync(meRequest);
            var me = JsonConvert.DeserializeObject<LinkedInMeResponse>(meResponse.Content!);
            var meOk = meResponse.IsSuccessful && !string.IsNullOrWhiteSpace(me?.id);

            if (!meOk)
            {
                return new ConnectionTestResult(this, false, me?.id, DescribeErrors(meResponse));
            }

            return new ConnectionTestResult(this, true, me!.id, 
                $"Token scope: {introspection!.scope}, expires at: {introspection!.expires_at}");
        }
    }

    private class LinkedInImageData 
    {
        public LinkedInImageData(string id, string? description, string uploadUrl)
        {
            this.id = id;
            this.description = description;
            this.uploadUrl = uploadUrl;
        }
        public string id { get; set; }
        public string? description { get; set; }
        public string uploadUrl { get; set; }
    }

    public class LinkedInMeResponse
    {
        public string? id { get; set; }
    }

    public  class LinkedInIntrospectionResponse
    {
        public bool? active { get; set; }
        public string? client_id { get; set; }
        public long? authorized_at { get; set; }
        public long? created_at { get; set; }
        public string? status { get; set; }
        public long expires_at { get; set; }
        public string? scope { get; set; }
        public string? auth_type { get; set; }
    }

    public class LinkedInImageResponse
    {
        public LinkedInImageResponseValue value {get;set;} = null!;
        
        public class LinkedInImageResponseValue
        {
            public long uploadUrlExpiresAt {get;set;}
            public string uploadUrl {get;set;} = null!;
            public string image {get;set;} = null!;
        }
    }

    public class LinkedInPollResponse
    {
        public string owner { get; set; } = null!;
        public long downloadUrlExpiresAt { get; set; }
        public string downloadUrl { get; set; } = null!;
        public string id { get; set; } = null!;
        public string status { get; set; } = null!;
    }

}
