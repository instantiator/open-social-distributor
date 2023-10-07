using System.Reflection;
using DistributorLib.Post.Images;

namespace DistributorLib.Tests;

public class SocialImageTests
{
    [Fact]
    public void ProjectPlacesTestFilesInTheRightPlace()
    {
        var runPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Assert.False(string.IsNullOrWhiteSpace(runPath));

        var testFilePath = Path.Combine(runPath, "TestData", "TestImages", "mastodon-logo.png");
        Assert.False(string.IsNullOrWhiteSpace(testFilePath));

        Assert.True(File.Exists(testFilePath), $"Test file not found at {testFilePath}");
    }

    [Fact]
    public void SocialImageCorrectlyIdentifiesFilename()
    {
        
    }

    [Fact]
    public async Task ResolvesFullPathFileUrls()
    {
        var runPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        await TestSocialImageUri($"file://{runPath}/TestData/TestImages/mastodon-logo.png", true, "TestData/TestImages/mastodon-logo.png");
    }

    [Fact]
    public async Task ResolvesRelativePathFileUrls()
    {
        await TestSocialImageUri($"file://TestData/TestImages/mastodon-logo.png", true, "TestData/TestImages/mastodon-logo.png");
    }

    [Fact]
    public async Task ResolvesHttpsUrls()
    {
        await TestSocialImageUri("https://github.com/instantiator/consensus-chess-engine/blob/main/docs/images/cog-icon.png?raw=true", false, "cog-icon.png");
    }

    private async Task TestSocialImageUri(string testUri, bool isFilePath, string testFilename)
    {
        var image = new SocialImage(testUri, null);
        
        if (isFilePath)
        {
            var correctRunPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assert.Equal($"{correctRunPath}/{testFilename}".ToLowerInvariant(), image.LocalPath?.ToLowerInvariant());
            Assert.Equal(isFilePath, File.Exists(image.LocalPath));
        }
        
        using (var stream = await image.GetStreamAsync())
        {
            Assert.NotNull(stream);
            using (var reader = new StreamReader(stream))
            {
                var len = (await reader.ReadToEndAsync()).Length;
                Assert.True(len > 0);
            }
        }
    }

}