using DistributorLib;
using DistributorLib.Network;
using DistributorLib.Post;
using Newtonsoft.Json;

namespace Integration.Tests;

public abstract class AbstractNetworkTests
{
    protected Config config;
    protected IEnumerable<ISocialNetwork> networks;

    protected AbstractNetworkTests()
    {
        var configPath = Environment.GetEnvironmentVariable("CONFIG_PATH");
        if (string.IsNullOrWhiteSpace(configPath)) throw new ArgumentNullException("CONFIG_PATH");
        
        var json = File.ReadAllText(configPath);
        config = JsonConvert.DeserializeObject<Config>(json)!;
        networks = NetworkFactory.FromConfig(config);
    }

    protected async Task<ConnectionTestResult> TestNetworkInit(ISocialNetwork network)
    {
        await network.InitAsync();
        Assert.True(network.Initialised);

        var testResult = await network.TestConnectionAsync();
        Assert.NotNull(testResult);
        Assert.True(testResult.Success);
        return testResult;
    }

    protected async Task<PostResult> TestNetworkPost(ISocialNetwork network, ISocialMessage message, int expectedPosts)
    {
        var result = await network.PostAsync(message);
        Assert.NotNull(result);
        Assert.True(result.Success, $"{result.Exception?.GetType().Name}: {result.Error}");
        Assert.True(result.Message == message);
        Assert.Equal(expectedPosts, result.PostIds!.Count());
        return result;
    }
}