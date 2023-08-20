using DistributorLib.Post;

namespace DistributorLib.Network;
public abstract class AbstractNetwork : ISocialNetwork
{
    protected AbstractNetwork(NetworkType type, string shortcode, string networkName, IPostVariant postVariant, string? accountId = null)
    {
        NetworkType = type;
        ShortCode = shortcode;
        NetworkName = networkName;
        NetworkAccountId = accountId;
        PostVariant = postVariant;
    }

    public NetworkType NetworkType { get; private set; }
    
    public string ShortCode { get; private set; }

    public string NetworkName { get; private set; }

    public IPostVariant PostVariant { get; private set; }

    public string? NetworkAccountId { get; private set; }

    public abstract ValueTask DisposeAsync();

    public bool Initialised { get; private set; } = false;

    public async Task InitAsync()
    {
        await InitClientAsync();
        Initialised = true;
    }

    public abstract Task InitClientAsync();

    public abstract Task<PostResult> PostAsync(ISocialMessage message);

}