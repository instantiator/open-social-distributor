using System.Reflection;
using DistributorLib.Input;

namespace DistributorLib.Tests;

public class PostListReaderTests
{
    [Fact]
    public void ProjectPlacesTestFilesInTheRightPlace()
    {
        var runPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Assert.False(string.IsNullOrWhiteSpace(runPath));

        var testFilePath = Path.Combine(runPath, "TestData", "single-message.jsonc");
        Assert.False(string.IsNullOrWhiteSpace(testFilePath));

        Assert.True(File.Exists(testFilePath), $"Test file not found at {testFilePath}");
    }

    [Fact]
    public void CanReadSampleJsoncList()
    {
        var reader = new PostListReader();
        var runPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var json = File.ReadAllText(Path.Combine(runPath!, "TestData", "single-message.jsonc"));
        var list = reader.ReadJson(json);
        Assert.NotNull(list);
        Assert.Single(list.Posts);
        Assert.Equal(6, list.Posts.Single().Parts.Count());
        Assert.Equal(2, list.Posts.Single().Images.Count());
        Assert.All(list.Posts, p =>
        {
            Assert.All(p.Parts, pp => 
            {
                Assert.NotNull(pp.Content);
                Assert.NotEmpty(pp.Content);
                Assert.All(pp.Content.Values, v => Assert.False(string.IsNullOrWhiteSpace(v)));
           });

           Assert.All(p.Images, pi =>
           {
               Assert.False(string.IsNullOrWhiteSpace(pi.Uri));
               Assert.False(string.IsNullOrWhiteSpace(pi.Description));
           });
        });
    }

    [Fact]
    public void CanConvertPostListToSocialMessages()
    {
        var reader = new PostListReader();
        var runPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var json = File.ReadAllText(Path.Combine(runPath!, "TestData", "single-message.jsonc"));
        var list = reader.ReadJson(json);
        var messages = list!.ToSocialMessages();
        Assert.NotNull(messages);
        Assert.Single(messages);
        Assert.Equal(6, messages.Single().Parts!.Count());
        Assert.NotNull(messages.Single().Images);
        Assert.Equal(2, messages.Single().Images!.Count());
    }
}
