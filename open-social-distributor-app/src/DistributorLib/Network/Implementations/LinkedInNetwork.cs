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
    private string authorId;

    private RestClient? client;

    public LinkedInNetwork(string shortcode, Mode mode, string clientId, string clientSecret, string token, string authorId) 
        : base(NetworkType.LinkedIn, shortcode, "linkedin", PostFormatVariantFactory.LinkedIn)
    {
        this.mode = mode;
        this.token = token;
        this.authorId = authorId;
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
        var author = mode == Mode.Org ? $"urn:li:organization:{authorId}" : $"urn:li:member:{authorId}";
        foreach (var text in texts)
        {
            var first = responses.Count() == 0;
            if (first)
            {
                var request = new RestRequest($"/posts", Method.Post);
                request.AddHeader("Authorization", $"Bearer {token}");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-Restli-Protocol-Version", "2.0.0");
                request.AddHeader("Linkedin-Version","202309");
                var content = new 
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
                    lifecycleState = "PUBLISHED",
                    isReshareDisabledByAuthor = false
                };
                Console.WriteLine(JsonConvert.SerializeObject(content));
                //throw new Exception("Not gonna work anyway: {\"serviceErrorCode\":100,\"message\":\"Field Value validation failed in REQUEST_BODY: Data Processing Exception while processing fields [/author]\",\"status\":403}");
                request.AddJsonBody(content); // JsonConvert.SerializeObject(content);
                var response = await client!.ExecuteAsync(request);
                var idUrn = response.Headers!.SingleOrDefault(h => h.Name=="x-linkedin-id")?.Value?.ToString();
                var aok = response.IsSuccessful && !string.IsNullOrWhiteSpace(idUrn);
                var headers = response.Headers!.Select(h => new Tuple<string,string?>(h.Name!, h.Value?.ToString()));
                var liResponse = new LinkedInResponse() { Success = aok, Id = idUrn, Content = response.Content, Headers = headers };
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
                        request.AddHeader("X-Restli-Protocol-Version", "2.0.0");
                        request.AddHeader("Linkedin-Version","202309");
                        var content = new 
                        { 
                            actor = author, 
                            @object = firstId,
                            message = new { text = text },
                            content = new object[] { }
                        };
                        request.AddJsonBody(content);
                        var response = await commentClient!.ExecuteAsync(request);
                        var idUrn = response.Headers!.SingleOrDefault(h => h.Name=="x-linkedin-id")?.Value?.ToString();
                        var aok = response.IsSuccessful && !string.IsNullOrWhiteSpace(idUrn);
                        var headers = response.Headers!.Select(h => new Tuple<string,string?>(h.Name!, h.Value?.ToString()));
                        var liResponse = new LinkedInResponse() { Success = aok, Id = idUrn, Content = response.Content, Headers = headers };
                        responses.Add(new Tuple<RestResponse, LinkedInResponse>(response, liResponse));
                    }
                }
                else
                {
                    throw new Exception("Cannot post comments - main post does not have an id.");
                }
            }
        }
        var all_ok = responses.All(r => r.Item1.IsSuccessful && r.Item2.Success);
        var errorStr = string.Join('\n', responses.Where(r => !r.Item2.Success).Select(r => r.Item2.Content));
        var ids = responses.Select(r => r.Item2.Id);
        return new PostResult(this, message, all_ok, ids, errorStr);
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
                (tokenIntrospectionResponse.IsSuccessful && introspection!.active == true && introspection!.status == "active") &&
                ((mode == Mode.Org && (introspection!.scope?.Contains("w_organization_social") ?? false)) ||
                (mode == Mode.User && (introspection!.scope?.Contains("w_member_social") ?? false)));

            if (!tokenIntrospectionOk)
            {
                return new ConnectionTestResult(this, false, tokenIntrospectionResponse.Content);
            }

            // fetch the user or organisation profile
            var meRequest = new RestRequest("/me", Method.Get);
            meRequest.AddHeader("Authorization", $"Bearer {token}");
            var meResponse = await client!.ExecuteAsync(meRequest);
            var me = JsonConvert.DeserializeObject<LinkedInMeResponse>(meResponse.Content!);
            var meOk = meResponse.IsSuccessful && !string.IsNullOrWhiteSpace(me.id);

            if (!meOk)
            {
                return new ConnectionTestResult(this, false, meResponse.Content);
            }

            return new ConnectionTestResult(this, true, 
                $"Token for user id: {me.id} has scope: {introspection!.scope}, expires at: {introspection!.expires_at}");
        }
    }

    public class LinkedInMeResponse
    {
        public string id { get; set; }
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
