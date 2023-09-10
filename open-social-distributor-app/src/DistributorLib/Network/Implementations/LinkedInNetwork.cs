using DistributorLib.Network;
using DistributorLib.Post;
using DistributorLib.Post.Formatters;
using Newtonsoft.Json;
using RestSharp;

public class LinkedInNetwork : AbstractNetwork
{
    private readonly string LINKEDIN_API_POST_BASE = "https://api.linkedin.com/v2";
    private readonly string LINKEDIN_API_COMMENT_BASE = "https://api.linkedin.com/rest/socialActions";
    private readonly string LINKEDIN_INTROSPECTION_BASE = "https://www.linkedin.com/oauth/v2";

    public enum Mode { Org, User }

    private Mode mode;
    private string clientId;
    private string clientSecret;
    private string token;
    private string orgId;

    private RestClient? client;

    public LinkedInNetwork(string shortcode, Mode mode, string clientId, string clientSecret, string token, string orgId) 
        : base(NetworkType.LinkedIn, shortcode, "linkedin", PostFormatVariantFactory.LinkedIn)
    {
        this.mode = mode;
        this.token = token;
        this.orgId = orgId;
        this.clientId = clientId;
        this.clientSecret = clientSecret;
    }

    protected override async Task InitClientAsync()
    {
        client = new RestClient(LINKEDIN_API_POST_BASE);
    }

    protected override async Task DisposeClientAsync()
    {
        client?.Dispose();
        client = null;
    }

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message)
    {
        var texts = Formatter.FormatText(message);
        var responses = new List<Tuple<RestResponse, LinkedInResponse>>();
        foreach (var text in texts)
        {
            var first = responses.Count() == 0;
            switch (mode)
            {
                case Mode.Org:
                    if (first)
                    {
                        var request = new RestRequest($"/posts", Method.Post);
                        request.AddHeader("Authorization", $"Bearer {token}");
                        request.AddHeader("Content-Type", "application/json");
                        var content = new 
                        { 
                            author = $"urn:li:organization:{orgId}", 
                            commentary = text,
                            visibility = "PUBLIC",
                            distribution = new 
                            {
                                feedDistribution = "MAIN_FEED",
                                targetEntities = new object[] { },
                                thirdPartyDistributionChannels = new object[] { }
                            },
                            lifecycleState = "PUBLISHED",
                            isReshareDisabledByAuthor = false
                        };
                        request.AddJsonBody(content); // JsonConvert.SerializeObject(content);
                        var response = await client!.ExecuteAsync(request);
                        var idUrn = response.Headers!.SingleOrDefault(h => h.Name=="x-linkedin-id")?.Value?.ToString();
                        var aok = response.IsSuccessful && !string.IsNullOrWhiteSpace(idUrn);
                        var liResponse = new LinkedInResponse() { Success = aok, Id = idUrn };
                        responses.Add(new Tuple<RestResponse, LinkedInResponse>(response, liResponse));
                    }
                    else
                    {
                        var firstId = responses.First().Item2.Id;
                        if (!string.IsNullOrWhiteSpace(firstId))
                        {
                            using (var commentClient = new RestClient(LINKEDIN_API_COMMENT_BASE))
                            {
                                var request = new RestRequest($"/{firstId}/comments", Method.Post);
                                request.AddHeader("Authorization", $"Bearer {token}");
                                request.AddHeader("Content-Type", "application/json");
                                var content = new 
                                { 
                                    actor = $"urn:li:organization:{orgId}", 
                                    @object = firstId,
                                    message = new { text = text },
                                    content = new object[] { }
                                };
                                request.AddJsonBody(content);
                                var response = await commentClient!.ExecuteAsync(request);
                                var idUrn = response.Headers!.SingleOrDefault(h => h.Name=="x-linkedin-id")?.Value?.ToString();
                                var aok = response.IsSuccessful && !string.IsNullOrWhiteSpace(idUrn);
                                var liResponse = new LinkedInResponse() { Success = aok, Id = idUrn };
                                responses.Add(new Tuple<RestResponse, LinkedInResponse>(response, liResponse));
                            }
                        }
                        else
                        {
                            throw new Exception("Cannot post comments - main post does not have an id.");
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException($"LinkedInNetwork mode {mode} not implemented");
            }
        }
        var all_ok = responses.All(r => r.Item1.IsSuccessful && r.Item2.Success);
        var error = all_ok ? null : "One or more posts failed";
        var ids = responses.Select(r => r.Item2.Id);
        return new PostResult(this, message, all_ok, ids, error);
    }

    public class LinkedInResponse
    {
        public bool Success { get; set; }
        public string? Id { get; set; }
    }

    protected override async Task<ConnectionTestResult> TestConnectionImplementationAsync()
    {
        using (var testClient = new RestClient(LINKEDIN_INTROSPECTION_BASE))
        {
            switch (mode)
            {
                case Mode.Org:
                    var request = new RestRequest($"/introspectToken", Method.Post);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("token", token);
                    request.AddParameter("client_id", clientId);
                    request.AddParameter("client_secret", clientSecret);
                    var response = await testClient!.ExecuteAsync(request);
                    var introspection = JsonConvert.DeserializeObject<LinkedInIntrospectionResponse>(response.Content!);
                    var aok = response.IsSuccessful && introspection!.active == true && introspection!.status == "active";
                    var scopeOk = 
                        (mode == Mode.Org && (introspection!.scope?.Contains("w_organization_social") ?? false)) ||
                        (mode == Mode.User && (introspection!.scope?.Contains("w_member_social") ?? false));
                    return new ConnectionTestResult(this, aok && scopeOk, response.Content);

                default:
                    throw new NotImplementedException($"LinkedInNetwork mode {mode} not implemented");
            }
        }
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
}

// x-li-responseorigin=RGW
// Location=/posts/urn%3Ali%3Ashare%3A7106697166089269248
// x-linkedin-id=urn:li:share:7106697166089269248
// x-restli-protocol-version=1.0.0
// Date=Sun, 10 Sep 2023 17:57:20 GMT
// X-Li-Fabric=prod-ltx1
// Connection=keep-alive
// X-Li-Source-Fabric=prod-lva1
// X-Li-Pop=prod-lva1-x
// X-LI-Proto=http/1.1
// X-LI-UUID=AAYFBPDs9ZaGJ88vWwff2w==
// Set-Cookie=bcookie="v=2&32857c53-17ea-426c-88a3-33af51b87db0"; Domain=.linkedin.com; Expires=Mon, 09-Sep-2024 17:57:20 GMT; Path=/; Secure; SameSite=None
// Set-Cookie=li_gc=MTswOzE2OTQzNjg2NDA7MjswMjGiG9FjYkOuIfSWzMJkMPpZYtEsi3b0bSEqKPjKVFYG0w==; Domain=.linkedin.com; Expires=Fri, 08 Mar 2024 17:57:20 GMT; Path=/; Secure; SameSite=None
// Set-Cookie=lidc="b=TB87:s=T:r=T:a=T:p=T:g=3043:u=909:x=1:i=1694368640:t=1694437254:v=2:sig=AQFq9r4NXuou8UFuHSIAxBw_nvNWJIKv"
// X-LI-Route-Key="b=TB87:s=T:r=T:a=T:p=T:g=3043:u=909:x=1:i=1694368640:t=1694437254:v=2:sig=AQFq9r4NXuou8UFuHSIAxBw_nvNWJIKv"