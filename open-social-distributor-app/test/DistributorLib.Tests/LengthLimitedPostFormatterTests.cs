using DistributorLib.Network;
using DistributorLib.Post;
using DistributorLib.Post.Formatters;

namespace DistributorLib.Tests;

public class LengthLimitedPostFormatterTests
{
    [Fact]
    public void LengthLimitedPostFormatter_FormatText_SplitsOnWords()
    {
        var formatter = new LengthLimitedPostFormatter(NetworkType.Any, 10, 10, false);
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
    public void LengthLimitedPostFormatter_FormatText_RecognisesFirstAndSubsequentLimits()
    {
        var formatter = new LengthLimitedPostFormatter(NetworkType.Any, 20, 10, false);
        var message = new SimpleSocialMessage("This is a test message indeed so");
        var result = formatter.FormatText(message);
        Assert.Equal(4, result.Count());
        Assert.Equal("This is a test /1", result.ElementAt(0));
        Assert.Equal("message /2", result.ElementAt(1));
        Assert.Equal("indeed /3", result.ElementAt(2));
        Assert.Equal("so", result.ElementAt(3));
    }

    [Fact]
    public void LengthLimitedPostFormatter_FormatText_HandlesAwkwardEdgeCase()
    {
        var formatter = new LengthLimitedPostFormatter(NetworkType.Any, 10, 10, false);
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
        var formatter = new LengthLimitedPostFormatter(NetworkType.Any, 10, 10, false);
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
        var formatter = new LengthLimitedPostFormatter(NetworkType.Any, 10, 10, false);
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

    [Fact]
    public void LengthLimitedPostFormatter_FormatText_HandlesBreakCodes()
    {
        var formatter = new LengthLimitedPostFormatter(NetworkType.Any, 10, 10, false, LengthLimitedPostFormatter.BreakBehaviour.NewPost);
        var message = new SimpleSocialMessage("First $$ Second $$ Third");
        var result = formatter.FormatText(message);
        Assert.Equal(3, result.Count());
        Assert.Equal("First /1", result.ElementAt(0));
        Assert.Equal("Second /2", result.ElementAt(1));
        Assert.Equal("Third", result.ElementAt(2));
    }

    [Fact]
    public void LengthLimitedPostFormatter_FormatText_HandlesBigTextWithAllFeatures()
    {
        var formatter = new LengthLimitedPostFormatter(
            NetworkType.Any, 
            100, 100,
            true, 
            LengthLimitedPostFormatter.BreakBehaviour.NewPost,
            LengthLimitedPostFormatter.TagBehaviour.AllPosts);
    
        var parts = new List<SocialMessageContent>()
        {
            new SocialMessageContent("This is a nice long test message with a whole bunch of features attached."),
            new SocialMessageContent("It's important that there are over 100 characters across all the text, because this is an experiment with long text."),
            new SocialMessageContent("LongText", NetworkType.Any, SocialMessagePart.Tag),
            new SocialMessageContent("Experiment", NetworkType.Any, SocialMessagePart.Tag),
            new SocialMessageContent("Formatting", NetworkType.Any, SocialMessagePart.Tag),
            new SocialMessageContent("https://instantiator.dev", NetworkType.Any, SocialMessagePart.Link)
        };

        var message = new SimpleSocialMessage(parts, null);
        var result = formatter.FormatText(message);

        // Console.WriteLine(string.Join("\n", result));
        // This is a nice long test message with a whole bunch of features /1 #LongText #Formatting #Experiment
        // attached. It's important that there are over 100 characters /2 #Formatting #Experiment #LongText
        // across all the text, because this is an experiment with long /3 #Experiment #Formatting #LongText
        // text. https://instantiator.dev #Formatting #Experiment #LongText

        Assert.Equal(4, result.Count());
        Assert.StartsWith("This is a nice long test message with a whole bunch of features /1", result.ElementAt(0));
        Assert.StartsWith("attached. It's important that there are over 100 characters /2", result.ElementAt(1));
        Assert.StartsWith("across all the text, because this is an experiment with long /3", result.ElementAt(2));
        Assert.StartsWith("text. https://instantiator.dev", result.ElementAt(3));

        // find the tags - but they'll be in a random order
        foreach (var msg in result)
        {
            Assert.True(msg.Contains("#LongText") && msg.Contains("#Experiment") && msg.Contains("#Formatting"));
        }
    }


}