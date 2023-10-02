namespace Integration.Tests;

public class LinkedInIntegrationTests : AbstractNetworkTests
{
    [Fact]
    public async Task LinkedInkNetworkCanHandleTextAndSingleImages()
    {
        var network = networks.Single(n => n.NetworkType == DistributorLib.Network.NetworkType.LinkedIn);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateSimplestMessage(), 1);
    }

    [Fact]
    public async Task LinkedInkNetworkCanHandleTextAndMultipleImages()
    {
        var network = networks.Single(n => n.NetworkType == DistributorLib.Network.NetworkType.LinkedIn);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateSimplestMessage(2), 1);
    }

    [Fact]
    public async Task LinkedInNetworkCanHandleLongText()
    {
        var network = networks.Single(n => n.NetworkType == DistributorLib.Network.NetworkType.LinkedIn);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateLongTextMessage(), 1);
    }

    [Fact]
    public async Task LinkedInNetworkCanHandleBreakWordsAndMultipleImages()
    {
        var network = networks.Single(n => n.NetworkType == DistributorLib.Network.NetworkType.LinkedIn);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateComplexMessageWithBreakWords(), 1);
    }

}