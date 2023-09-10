using DistributorLib.Network;
using DistributorLib.Post;
using DistributorLib.Post.Formatters;
using Newtonsoft.Json;
using RestSharp;

public class LinkedInNetwork : AbstractNetwork
{
    private readonly string LINKEDIN_API_BASE = "https://api.linkedin.com/v2";
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
        client = new RestClient(LINKEDIN_API_BASE);
    }

    protected override async Task DisposeClientAsync()
    {
        client?.Dispose();
        client = null;
    }

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message)
    {
        var texts = Formatter.FormatText(message);
        var link = message.Link;
        if (texts.Count() > 1) { throw new Exception($"Text was longer than the LinkedIn network limit of: 3000"); }

        var text = texts.Single();

        switch (mode)
        {
            case Mode.Org:
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
                var aok = response.IsSuccessful;
                return new PostResult(this, message, aok, response.Content);

            default:
                throw new NotImplementedException($"LinkedInNetwork mode {mode} not implemented");
        }
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

                    // request.AddParameter(
                    //     "application/x-www-form-urlencoded", 
                    //     $"token={token}&client_id={clientId}&client_secret={clientSecret}", 
                    //     ParameterType.RequestBody);

                    var response = await testClient!.ExecuteAsync(request);
                    var introspection = JsonConvert.DeserializeObject<LinkedInIntrospectionResponse>(response.Content!);
                    var aok = response.IsSuccessful && introspection!.active == true && introspection!.status == "active";
                    return new ConnectionTestResult(this, aok, response.Content);

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