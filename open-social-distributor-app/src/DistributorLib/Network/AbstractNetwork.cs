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


    public bool Initialised { get; private set; } = false;

    public async Task InitAsync()
    {
        Console.WriteLine($"Initialising {ShortCode} ({NetworkType})...");
        await InitClientAsync();
        Initialised = true;
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine($"Disposing {ShortCode} ({NetworkType})...");
        await DisposeClientAsync();
        Initialised = false;
    }

    protected abstract Task InitClientAsync();

    protected abstract Task DisposeClientAsync();

    public async Task<bool> TestConnectionAsync()
    {
        Console.WriteLine($"Testing {ShortCode} ({NetworkType}) connection...");
        if (!Initialised) return false;
        return await TestConnectionImplementationAsync();
    }

    protected abstract Task<bool> TestConnectionImplementationAsync();

    public async Task<PostResult> PostAsync(ISocialMessage message)
    {
        Console.WriteLine($"Posting to {ShortCode} ({NetworkType})...");
        return await PostImplementationAsync(message);
    }

    protected abstract Task<PostResult> PostImplementationAsync(ISocialMessage message);

}