using DistributorLib.Network.Implementations;

namespace Integration.Tests;

public class MastodonIntegrationTests : AbstractNetworkTests
{
    [Fact]
    public async Task MastodonNetworkCanHandleTextAndImages()
    {
        var network = networks.Single(n => n.NetworkType == DistributorLib.Network.NetworkType.Mastodon);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateSimplestMessage(), 1);
    }

}
