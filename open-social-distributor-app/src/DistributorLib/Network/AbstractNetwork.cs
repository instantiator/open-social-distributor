using DistributorLib.Post;

namespace DistributorLib.Network;
public abstract class AbstractNetwork : ISocialNetwork
{
    protected AbstractNetwork(string code, string network, string? accountId = null)
    {
        ShortCode = code;
        NetworkName = network;
        NetworkAccountId = accountId;
    }

    public string ShortCode { get; private set; }

    public string NetworkName { get; private set; }

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