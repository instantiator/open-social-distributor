namespace Integration.Tests;

public class FacebookIntegrationTests : AbstractNetworkTests
{
    [Fact]
    public async Task FacebookNetworkCanHandleTextAndImages()
    {
        var network = networks.Single(n => n.NetworkType == DistributorLib.Network.NetworkType.Facebook);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateSimplestMessage(), 1);
    }

    [Fact]
    public async Task FacebookNetworkCanHandleLongText()
    {
        var network = networks.Single(n => n.NetworkType == DistributorLib.Network.NetworkType.Facebook);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateLongTextMessage(), 1);
    }

    [Fact]
    public async Task FacebookNetworkCanHandleBreakWordsAndMultipleImages()
    {
        var network = networks.Single(n => n.NetworkType == DistributorLib.Network.NetworkType.Facebook);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateComplexMessageWithBreakWords(), 1);
    }

}