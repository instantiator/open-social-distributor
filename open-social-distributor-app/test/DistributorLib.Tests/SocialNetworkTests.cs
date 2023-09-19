using DistributorLib.Network;
using DistributorLib.Network.Implementations;
using DistributorLib.Post;
using DistributorLib.Post.Images;

namespace DistributorLib.Tests;

public class SocialNetworkTests
{
    [Fact]
    public async Task ConsoleNetworkCanHandleTextAndImages()
    {
        var network = new ConsoleNetwork
        {
            DryRunPosting = true
        };

        var testResult = await TestNetworkInit(network);
        var postResult = await TestNetworkPost(network, CreateSimplestMessage(), 1);
    }

    private async Task<ConnectionTestResult> TestNetworkInit(ISocialNetwork network)
    {
        await network.InitAsync();
        Assert.True(network.Initialised);

        var testResult = await network.TestConnectionAsync();
        Assert.NotNull(testResult);
        Assert.True(testResult.Success);
        return testResult;
    }

    private async Task<PostResult> TestNetworkPost(ISocialNetwork network, ISocialMessage message, int expectedPosts)
    {
        var result = await network.PostAsync(message);
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.True(result.Message == message);
        Assert.Equal(expectedPosts, result.PostIds!.Count());
        return result;
    }

    private ISocialMessage CreateSimplestMessage()
    {
        var parts = new List<SocialMessageContent>
        {
            new SocialMessageContent("Hello this is a test")
        };
        var images = new List<ISocialImage>()
        {
            SocialImageFactory.FromUri("file://TestData/TestImages/mastodon-logo.png", "the Mastodon logo")
        };
        return new SimpleSocialMessage(parts, images);
    }

}