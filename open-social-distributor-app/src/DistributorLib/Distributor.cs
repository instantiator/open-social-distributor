using DistributorLib.Network;
using DistributorLib.Post;

namespace DistributorLib;

public class Distributor : IAsyncDisposable
{
    private List<ISocialNetwork> networks = new List<ISocialNetwork>();
    private List<PostResult> history = new List<PostResult>();

    public Distributor()
    {
    }

    public IEnumerable<ISocialNetwork> Networks => networks;

    public IEnumerable<PostResult> History => history;

    public async Task AddNetworkAsync(ISocialNetwork network, bool init = false)
    {
        networks.Add(network);
        if (init && !network.Initialised) await network.InitAsync();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var network in networks)
        {
            await network.DisposeAsync();
        }
        networks.Clear();
    }

    public async Task<IEnumerable<PostResult>> PostAsync(ISocialMessage message)
    {
        var results = new List<PostResult>();
        foreach (var network in Networks)
        {
            try
            {
                if (!network.Initialised) await network.InitAsync();
                var result = await network.PostAsync(message);
                results.Add(result);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error posting to {network.NetworkName}, {e.GetType().Name}: {e.Message}");
                var result = new PostResult(network, message, false, e.Message, exception: e);
                results.Add(result);
            }
        }
        history.AddRange(results);
        return results;
    }


}