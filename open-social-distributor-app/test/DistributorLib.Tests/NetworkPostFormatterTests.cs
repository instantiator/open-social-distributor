using System.Reflection;
using DistributorLib.Input;
using DistributorLib.Network;
using DistributorLib.Post.Formatters;
using DistributorLib.Tests.Helpers;

namespace DistributorLib.Tests;

public class NetworkPostFormatterTests
{
        [Fact]
    public void ProjectPlacesTestFilesInTheRightPlace()
    {
        var runPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Assert.False(string.IsNullOrWhiteSpace(runPath));
        var testFilePath = Path.Combine(runPath, "TestData", "release-announcement-v0.1.jsonc");
        Assert.False(string.IsNullOrWhiteSpace(testFilePath));
        Assert.True(File.Exists(testFilePath), $"Test file not found at {testFilePath}");
    }

    [Theory]
    [InlineData("Hello world! #hello #lovely #world", "Hello world!")]
    public void RemoveHashTags_RemovesHashTags(string input, string expected)
    {
        var result = input.RemoveHashTags();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(NetworkType.LinkedIn, 1, new[] { 
@"🎉 Announcing release 0.1 of Open Social Distributor - a command line tool that can create posts and threads across a variety of social networks.

🆓 It's open source and free - and you're welcome to contribute to it if you fancy getting involved.

🧵 This post (and thread if you're looking at it on Mastodon!) is an early test for the Open Social Distributor... 🤞 https://instantiator.dev/open-social-distributor/" })]
    [InlineData(NetworkType.Mastodon, 3, new[] {
"🎉 Announcing release 0.1 of Open Social Distributor - a command line tool that can create posts and threads across a variety of social networks. https://instantiator.dev/open-social-distributor/ /1",
"🆓 It's open source and free - and you're welcome to contribute to it if you fancy getting involved. /2",
"🧵 This post (and thread if you're looking at it on Mastodon!) is an early test for the Open Social Distributor... 🤞 /3" })]
    [InlineData(NetworkType.Discord, 3, new[] { 
"🎉 Announcing release 0.1 of Open Social Distributor - a command line tool that can create posts and threads across a variety of social networks. https://instantiator.dev/open-social-distributor/",
"🆓 It's open source and free - and you're welcome to contribute to it if you fancy getting involved.",
"🧵 This post (and thread if you're looking at it on Mastodon!) is an early test for the Open Social Distributor... 🤞" })]
    public void LinkedInPostFormatter_CorrectlyFormatsAnnouncement(NetworkType network, int expectedMessages, IEnumerable<string> expectedText)
    {
        var runPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var json = File.ReadAllText(Path.Combine(runPath!, "TestData", "release-announcement-v0.1.jsonc"));
        var reader = new PostListReader();
        var list = reader.ReadJson(json);
        var message = list!.ToSocialMessages().Single();
        
        var formatter = PostFormatVariantFactory.For(network);
        var result = formatter.FormatText(message);

        Assert.Equal(expectedMessages, result.Count());

        var expectedMinusHashTags = expectedText.Select(m => m.RemoveHashTags().Trim());
        var resultMinusHashTags = result.Select(m => m.RemoveHashTags().Trim());

        Assert.Equal(expectedMinusHashTags, resultMinusHashTags);
    }
}