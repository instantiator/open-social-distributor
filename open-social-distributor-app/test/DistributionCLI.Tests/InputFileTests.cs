using DistributorLib.Input;
using DistributorLib.Network;
using DistributorLib.Post;

namespace DistributionCLI.Tests;

public class InputFileTests
{
    private static string CreateTempFile(string data)
    {
        var tempPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), Path.GetTempFileName()));
        Console.WriteLine("Creating temporary file...");
        Console.WriteLine($"Data: {data}");
        Console.WriteLine($"Path: {tempPath}");
        File.WriteAllText(tempPath, data);
        return tempPath;
    }

    private static string CreateSimpleConfigFile()
        => CreateTempFile(@"{ ""networks"": { ""enabled"": [ ""type=console"" ], ""disabled"": [] } }");

    private static string CreateEmptyPostList()
        => CreateTempFile(@"{ ""posts"": [] }");

    private static string CreateSimplePostListFile()
        => CreateTempFile(@"{ ""posts"": [ { ""parts"": [ { ""part"": ""Text"", ""content"" : { ""any"": ""Hello"" } } ] } ] }");

    [Fact]
    public void RejectsNoParameters()
    {
        DistributionCLI.Reset();
        int status = DistributionCLI.Main();
        Assert.Equal(1, status);        
    }

    [Fact]
    public void CanTestSimpleConfigFile()
    {
        DistributionCLI.Reset();
        var configPath = CreateSimpleConfigFile();
        int status = DistributionCLI.Main("test", "-c", configPath);
        Assert.Equal(0, status);
    }

    [Fact]
    public void CanPostSimpleMessage()
    {
        DistributionCLI.Reset();
        var configPath = CreateSimpleConfigFile();
        int status = DistributionCLI.Main("post", "-c", configPath, "-m", "Hello, world!");
        Assert.Equal(0, status);
        Assert.Equal("Hello, world!", DistributionCLI.LastPostResults!.Single().Message.Parts.Single().Content[NetworkType.Any]);
    }

    [Fact]
    public void AcceptsEmptyPostList()
    {
        DistributionCLI.Reset();
        var configPath = CreateSimpleConfigFile();
        var postsPath = CreateEmptyPostList();

        var inflated = new PostListReader().ReadFile(postsPath);
        Assert.NotNull(inflated);
        Assert.Empty(inflated!.Posts);

        int status = DistributionCLI.Main("post", "-c", configPath, "-s", postsPath);
        Assert.Equal(0, status);        
    }

    [Fact]
    public void AcceptsSimplePostList()
    {
        DistributionCLI.Reset();
        var configPath = CreateSimpleConfigFile();
        var postsPath = CreateSimplePostListFile();

        var inflated = new PostListReader().ReadFile(postsPath);
        Assert.NotNull(inflated);
        Assert.Single(inflated!.Posts);
        Assert.Single(inflated.Posts.Single().Parts);
        Assert.Equal(SocialMessagePart.Text, inflated.Posts.Single().Parts.Single().Part);
        Assert.Equal("Hello", inflated.Posts.Single().Parts.Single().Content[NetworkType.Any]);
        Assert.Empty(inflated!.Posts.Single().Images);

        int status = DistributionCLI.Main("post", "-c", configPath, "-s", postsPath);
        Assert.Equal(0, status);
        Assert.Single(DistributionCLI.LastPostResults!);
        Assert.True(DistributionCLI.LastPostResults!.Single().Success);
        Assert.Equal("Hello", DistributionCLI.LastPostResults!.Single().Message.Parts.Single().Content[NetworkType.Any]);
    }
    
}