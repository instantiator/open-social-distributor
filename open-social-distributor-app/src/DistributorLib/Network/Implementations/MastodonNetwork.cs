using DistributorLib;
using DistributorLib.Post;
using Mastonet;

namespace DistributorLib.Network.Implementations;
public class MastodonNetwork : AbstractNetwork
{
    private string token;
    private MastodonClient? client;

    public MastodonNetwork(string code, string instance, string token) : base(NetworkType.Mastodon, code, instance, PostVariantFactory.Mastodon)
    {
        this.token = token;
    }

    protected override async Task DisposeClientAsync()
    {
    }

    protected override async Task InitClientAsync()
    {
        client = new MastodonClient(NetworkName, token);
    }

    protected override async Task<ConnectionTestResult> TestConnectionImplementationAsync()
    {
        try
        {
            var account = await client!.GetCurrentUser();
            if (account != null)
            {
                return new ConnectionTestResult(this, true);
            }
            else
            {
                return new ConnectionTestResult(this, false, "Could not read current user account");
            }
        }
        catch (Exception e)
        {
            return new ConnectionTestResult(this, false, e.Message, e);
        }
    }

    private string NetworkUrl => $"https://{NetworkName}";

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message)
    {
        var text = PostVariant.Compose(message);
        var mediaIds = new List<string>();
        var status = await client!.PublishStatus(text, Visibility.Public, null, mediaIds, false, null, null, null, null);
        return new PostResult(this, message, status != null, status == null ? "Status null" : null);
    }
}