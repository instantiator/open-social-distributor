using DistributorLib.Post;
using DistributorLib.Post.Formatters;

namespace DistributorLib.Network;
public abstract class AbstractNetwork : ISocialNetwork
{
    protected AbstractNetwork(NetworkType type, string shortcode, string networkName, IPostFormatter formatter)
    {
        NetworkType = type;
        ShortCode = shortcode;
        NetworkName = networkName;
        Formatter = formatter;
    }

    public NetworkType NetworkType { get; private set; }
    
    public string ShortCode { get; private set; }

    public string NetworkName { get; private set; }

    public IPostFormatter Formatter { get; private set; }

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
            return new PostResult(this, message, false, null, e.Message, e);
        }
    }

    protected abstract Task<PostResult> PostImplementationAsync(ISocialMessage message);

}