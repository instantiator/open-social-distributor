using DistributorLib.Network;
using DistributorLib.Post;
using DistributorLib.Post.Formatters;

namespace DistributorLib.Tests;

public class LengthLimitedPostFormatterTests
{
    [Fact]
    public void LengthLimitedPostFormatter_FormatText_SplitsOnWords()
    {
        var formatter = new LengthLimitedPostFormatter(NetworkType.Any, 10, false);
        var message = new SimpleSocialMessage("This is a test message");
        var result = formatter.FormatText(message);
        Console.WriteLine(string.Join('\n', result));
        Assert.Equal(3, result.Count());
        Assert.Equal("This is /1", result.ElementAt(0));
        Assert.Equal("a test /2", result.ElementAt(1));
        Assert.Equal("message", result.ElementAt(2));
    }
}