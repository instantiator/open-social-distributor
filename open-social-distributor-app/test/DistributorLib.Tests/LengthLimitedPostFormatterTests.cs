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
        var message = new SimpleSocialMessage("This is a test message indeed so");
        var result = formatter.FormatText(message);
        Assert.Equal(5, result.Count());
        Assert.Equal("This is /1", result.ElementAt(0));
        Assert.Equal("a test /2", result.ElementAt(1));
        Assert.Equal("message /3", result.ElementAt(2));
        Assert.Equal("indeed /4", result.ElementAt(3));
        Assert.Equal("so", result.ElementAt(4));
    }

    [Fact]
    public void LengthLimitedPostFormatter_FormatText_HandlesAwkwardEdgeCase()
    {
        var formatter = new LengthLimitedPostFormatter(NetworkType.Any, 10, false);
        var message = new SimpleSocialMessage("This is a test messageX indeed");
        var result = formatter.FormatText(message);
        Assert.Equal(4, result.Count());
        Assert.Equal("This is /1", result.ElementAt(0));
        Assert.Equal("a test /2", result.ElementAt(1));
        Assert.Equal("messageX", result.ElementAt(2)); // couldn't quite squeeze in the index without breaking the word
        Assert.Equal("indeed", result.ElementAt(3));
    }

    [Fact]
    public void LengthLimitedPostFormatter_FormatText_HandlesVeryLongWords()
    {
        var formatter = new LengthLimitedPostFormatter(NetworkType.Any, 10, false);
        var message = new SimpleSocialMessage("This is a test message WithAVeryLongWordThatRunsOn");
        var result = formatter.FormatText(message);
        Assert.Equal(6, result.Count());
        Assert.Equal("This is /1", result.ElementAt(0));
        Assert.Equal("a test /2", result.ElementAt(1));
        Assert.Equal("message /3", result.ElementAt(2));
        Assert.Equal("WithAVeryL", result.ElementAt(3));
        Assert.Equal("ongWordTha", result.ElementAt(4));
        Assert.Equal("tRunsOn", result.ElementAt(5));
    }

    [Fact]
    public void LengthLimitedPostFormatter_FormatText_HandlesVeryLongWordsAndSubsequentWords()
    {
        var formatter = new LengthLimitedPostFormatter(NetworkType.Any, 10, false);
        var message = new SimpleSocialMessage("This is a test message WithAVeryLongWordThatRuns and some words after it");
        var result = formatter.FormatText(message);
        Assert.Equal(11, result.Count());
        Assert.Equal("This is /1", result.ElementAt(0));
        Assert.Equal("a test /2", result.ElementAt(1));
        Assert.Equal("message /3", result.ElementAt(2));
        Assert.Equal("WithAVeryL", result.ElementAt(3));
        Assert.Equal("ongWordTha", result.ElementAt(4));
        Assert.Equal("tRuns", result.ElementAt(5));
        Assert.Equal("and /7", result.ElementAt(6));
        Assert.Equal("some /8", result.ElementAt(7));
        Assert.Equal("words /9", result.ElementAt(8));
        Assert.Equal("after /10", result.ElementAt(9));
        Assert.Equal("it", result.ElementAt(10));
    }

}