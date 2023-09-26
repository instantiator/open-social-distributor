namespace Integration.Tests;

public class DiscordIntegrationTests : AbstractNetworkTests
{
    [Fact]
    public async Task DiscordNetworkCanHandleTextAndImages()
    {
        var network = networks.Single(n => n.NetworkType == DistributorLib.Network.NetworkType.Discord);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateSimplestMessage(), 1);
    }

    [Fact]
    public async Task DiscordNetworkCanHandleLongText()
    {
        var network = networks.Single(n => n.NetworkType == DistributorLib.Network.NetworkType.Discord);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateLongTextMessage(), 3);
    }

    [Fact]
    public async Task DiscordNetworkCanHandleBreakWordsAndMultipleImages()
    {
        var network = networks.Single(n => n.NetworkType == DistributorLib.Network.NetworkType.Discord);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateComplexMessageWithBreakWords(), 4);
    }
}