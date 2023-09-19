using DistributorLib.Network;
using DistributorLib.Post;

namespace Integration.Tests;

public abstract class AbstractNetworkTests
{
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
        Assert.True(result.Success);
        Assert.True(result.Message == message);
        Assert.Equal(expectedPosts, result.PostIds!.Count());
        return result;
    }
}