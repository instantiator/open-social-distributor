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

    public override async ValueTask DisposeAsync()
    {
    }

    public override async Task InitClientAsync()
    {
        client = new MastodonClient(NetworkName, token);
    }

    public override async Task<PostResult> PostAsync(ISocialMessage message)
    {
        var text = PostVariant.Compose(message);
        var mediaIds = new List<string>();
        var status = await client!.PublishStatus(text, Visibility.Public, null, mediaIds, false, null, null, null, null);
        return new PostResult((ISocialNetwork)this, message, status != null, status == null ? "Status null" : null);
    }
}