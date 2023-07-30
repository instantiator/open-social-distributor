using DistributorLib.Post;
using RestSharp;

namespace DistributorLib.Network.Implementations;

/// <summary>
/// LinkedIn network implementation.
/// See: https://learn.microsoft.com/en-us/linkedin/shared/authentication/client-credentials-flow?context=linkedin%2Fcontext
/// See: https://learn.microsoft.com/en-us/linkedin/shared/authentication/postman-getting-started
/// </summary>
public class LinkedInNetwork : AbstractNetwork
{
    private readonly string ACCESS_TOKEN_URI = "https://www.linkedin.com/oauth/v2/accessToken";

    private string clientId;
    private string clientSecret;

    private string accessToken;
    private DateTime accessTokenExpiry;

    public LinkedInNetwork(string code, string clientId, string clientSecret) : base(code, "LinkedIn", PostVariantFactory.LinkedIn)
    {
        this.clientId = clientId;
        this.clientSecret = clientSecret;
    }

    public override ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public override async Task InitClientAsync()
    {
        var moment = DateTime.Now;
        var access = await GetAccessTokenAsync(clientId, clientSecret);
        accessToken = access.Item1;
        accessTokenExpiry = moment.Add(access.Item2);
    }

    private async Task<Tuple<string, TimeSpan>> GetAccessTokenAsync(string id, string secret)
    {
        using (var client = new RestClient())
        {
            var request = new RestRequest(ACCESS_TOKEN_URI, Method.Post);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", id);
            request.AddParameter("client_secret", secret);
            var response = await client.PostAsync<LinkedInAccessTokenResponse>(request);
            return new Tuple<string, TimeSpan>(
                response!.access_token!,
                TimeSpan.FromSeconds(response!.expires_in));
        }
    }

    public override Task<PostResult> PostAsync(ISocialMessage message)
    {
        throw new NotImplementedException("It is not possible to post with just the client credentials flow.");
    }

    private class LinkedInAccessTokenResponse
    {
        public string? access_token { get; set; }
        public double expires_in { get; set; }
    }
}