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

    [Fact]
    public async Task MastodonNetworkCanHandleLongText()
    {
        var network = networks.Single(n => n.NetworkType == DistributorLib.Network.NetworkType.Mastodon);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateLongTextMessage(), 12);
    }

    [Fact]
    public async Task MastodonNetworkCanHandleBreakWordsAndMultipleImages()
    {
        var network = networks.Single(n => n.NetworkType == DistributorLib.Network.NetworkType.Mastodon);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateComplexMessageWithBreakWords(), 4);
    }
}
