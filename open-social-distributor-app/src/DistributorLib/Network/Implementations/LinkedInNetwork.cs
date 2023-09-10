using DistributorLib.Network;
using DistributorLib.Post;
using DistributorLib.Post.Formatters;
using Newtonsoft.Json;
using RestSharp;

public class LinkedInNetwork : AbstractNetwork
{
    private readonly string LINKEDIN_URL = "https://api.linkedin.com/v2";

    public enum Mode { Org, User }

    private Mode mode;
    private string token;
    private string orgId;

    private RestClient? client;

    public LinkedInNetwork(string shortcode, Mode mode, string token, string orgId) 
        : base(NetworkType.LinkedIn, shortcode, "linkedin", PostFormatVariantFactory.LinkedIn)
    {
        this.mode = mode;
        this.token = token;
        this.orgId = orgId;
    }

    protected override async Task InitClientAsync()
    {
        client = new RestClient(LINKEDIN_URL);
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
        switch (mode)
        {
            case Mode.Org:
                var request = new RestRequest($"/me", Method.Get);
                request.AddHeader("Authorization", $"Bearer {token}");
                var response = await client!.ExecuteAsync(request);
                var aok = response.IsSuccessful;
                return new ConnectionTestResult(this, aok, response.Content);

            default:
                throw new NotImplementedException($"LinkedInNetwork mode {mode} not implemented");
        }

    }
}