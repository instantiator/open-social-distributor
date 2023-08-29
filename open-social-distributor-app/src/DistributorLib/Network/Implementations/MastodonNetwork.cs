using DistributorLib;
using DistributorLib.Post;
using DistributorLib.Post.Formatters;
using Mastonet;
using Mastonet.Entities;

namespace DistributorLib.Network.Implementations;
public class MastodonNetwork : AbstractNetwork
{
    private string token;
    private MastodonClient? client;

    public MastodonNetwork(string code, string instance, string token) : base(NetworkType.Mastodon, code, instance, PostFormatVariantFactory.Mastodon)
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
        var texts = Formatter.FormatText(message);
        var statuses = new List<Status>();
        foreach (var text in texts)
        {
            // TODO: image uploads - add images to the first post, figure out something for subsequent posts if necessary
            var mediaIds = new List<string>();
            var previousStatus = statuses.LastOrDefault();
            var status = await client!.PublishStatus(text, Visibility.Public, previousStatus?.Id, mediaIds, false, null, null, null, null);
            statuses.Add(status);
        }
        var aok = statuses.All(s => s != null && !string.IsNullOrWhiteSpace(s.Id));
        return new PostResult(this, message, aok, aok ? null : "Unable to post all statuses");
    }
}