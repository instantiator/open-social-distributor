using DistributorLib.Post;

namespace DistributorLib.Network;
public abstract class AbstractNetwork : ISocialNetwork
{
    protected AbstractNetwork(NetworkType type, string shortcode, string networkName, IPostVariant postVariant)
    {
        NetworkType = type;
        ShortCode = shortcode;
        NetworkName = networkName;
        PostVariant = postVariant;
    }

    public NetworkType NetworkType { get; private set; }
    
    public string ShortCode { get; private set; }

    public string NetworkName { get; private set; }

    public IPostVariant PostVariant { get; private set; }

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

    public async Task<ConnectionTestResult> TestConnectionAsync()
    {
        Console.WriteLine($"Testing {ShortCode} ({NetworkType}) connection...");
        if (!Initialised) return new ConnectionTestResult(this, false, "Network not initialised");
        return await TestConnectionImplementationAsync();
    }

    protected abstract Task<ConnectionTestResult> TestConnectionImplementationAsync();

    public async Task<PostResult> PostAsync(ISocialMessage message)
    {
        try
        {
            Console.WriteLine($"Posting to {ShortCode} ({NetworkType})...");
            return await PostImplementationAsync(message);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.ToString());
            return new PostResult(this, message, false, e.Message, e);
        }
    }

    protected abstract Task<PostResult> PostImplementationAsync(ISocialMessage message);

}