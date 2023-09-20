using DistributorLib.Network.Implementations;

namespace Integration.Tests;

public class MastodonIntegrationTests : AbstractNetworkTests
{
    private static string code = "mastodon-integration-test";
    private static string instance = "botsin.space";
    private static string accountName = "open_social_distributor_test";
    private static string token = "OLGIVWgg1dMtbOt3udkFamiGAbdiPcEJNw6KoB8nuA4"; // not too secret, it's a test account only
    private static string connectionString => $"type=mastodon;code={code};instance={instance};token={token}";

    [Fact]
    public async Task MastodonNetworkCanHandleTextAndImages()
    {
        var network = new MastodonNetwork(code, instance, token);
        
        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, MessageHelper.CreateSimplestMessage(), 1);
    }

}
