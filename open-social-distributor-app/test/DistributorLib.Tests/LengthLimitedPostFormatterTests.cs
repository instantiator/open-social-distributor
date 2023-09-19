using DistributorLib.Network;
using DistributorLib.Post;
using DistributorLib.Post.Formatters;

namespace DistributorLib.Tests;

public class LengthLimitedPostFormatterTests
{
    [Fact]
    public void LengthLimitedPostFormatter_FormatText_SplitsOnWords()
    {
        var formatter = new LengthLimitedPostFormatter(
            network: NetworkType.Any, 
            limit: 10, 
            subsequentLimits: 10, 
            indices: true,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.Inline,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewPost,
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost);
            
        var message = new SimpleSocialMessage("This is a test message indeed so");
        var result = formatter.FormatText(message);
        Assert.Equal(5, result.Count());
        Assert.Equal("This is /1", result.ElementAt(0));
        Assert.Equal("a test /2", result.ElementAt(1));
        Assert.Equal("message /3", result.ElementAt(2));
        Assert.Equal("indeed /4", result.ElementAt(3));
        Assert.Equal("so /5", result.ElementAt(4));
    }

    [Fact]
    public void LengthLimitedPostFormatter_FormatText_RecognisesFirstAndSubsequentLimits()
    {
        var formatter = new LengthLimitedPostFormatter(
            network: NetworkType.Any, 
            limit: 20, 
            subsequentLimits: 10, 
            indices: true,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.Inline,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewPost,
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost);

        var message = new SimpleSocialMessage("This is a test message indeed so");
        var result = formatter.FormatText(message);
        Assert.Equal(4, result.Count());
        Assert.Equal("This is a test /1", result.ElementAt(0));
        Assert.Equal("message /2", result.ElementAt(1));
        Assert.Equal("indeed /3", result.ElementAt(2));
        Assert.Equal("so /4", result.ElementAt(3));
    }

    [Fact]
    public void LengthLimitedPostFormatter_FormatText_HandlesAwkwardEdgeCase()
    {
        var formatter = new LengthLimitedPostFormatter(
            network: NetworkType.Any, 
            limit: 10, 
            subsequentLimits: 10, 
            indices: true,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.Inline,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewPost,
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost);

        var message = new SimpleSocialMessage("This is a test messageX indeed");
        var result = formatter.FormatText(message);
        // Console.WriteLine(string.Join("\n", result.Select(s => $"*** {s}")));

        Assert.Equal(5, result.Count());
        Assert.Equal("This is /1", result.ElementAt(0));
        Assert.Equal("a test /2", result.ElementAt(1));
        Assert.Equal("message /3", result.ElementAt(2)); // couldn't quite squeeze in the index without breaking the word
        Assert.Equal("X /4", result.ElementAt(3));
        Assert.Equal("indeed /5", result.ElementAt(4));
    }

    [Fact]
    public void LengthLimitedPostFormatter_FormatText_HandlesVeryLongWords()
    {
        var formatter = new LengthLimitedPostFormatter(
            network: NetworkType.Any, 
            limit: 10, 
            subsequentLimits: 10, 
            indices: true,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.Inline,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewPost,
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost);

        var message = new SimpleSocialMessage("This is a test message WithAVeryLongWordThatRunsOn");
        var result = formatter.FormatText(message);
        Assert.Equal(7, result.Count());
        Assert.Equal("This is /1", result.ElementAt(0));
        Assert.Equal("a test /2", result.ElementAt(1));
        Assert.Equal("message /3", result.ElementAt(2));
        Assert.Equal("WithAVe /4", result.ElementAt(3));
        Assert.Equal("ryLongW /5", result.ElementAt(4));
        Assert.Equal("ordThat /6", result.ElementAt(5));
        Assert.Equal("RunsOn /7", result.ElementAt(6));
    }

    [Fact]
    public void LengthLimitedPostFormatter_FormatText_HandlesVeryLongWordsAndSubsequentWords()
    {
        var formatter = new LengthLimitedPostFormatter(
            network: NetworkType.Any, 
            limit: 10, 
            subsequentLimits: 10, 
            indices: true,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.Inline,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewPost,
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost);

        var message = new SimpleSocialMessage("This is a test message WithAVeryLongWordThatRuns and some words after it");
        var result = formatter.FormatText(message);
        // Console.WriteLine(string.Join("\n", result.Select(s => $"*** {s}")));

        Assert.Equal(12, result.Count());
        Assert.Equal("This is /1", result.ElementAt(0));
        Assert.Equal("a test /2", result.ElementAt(1));
        Assert.Equal("message /3", result.ElementAt(2));
        Assert.Equal("WithAVe /4", result.ElementAt(3));
        Assert.Equal("ryLongW /5", result.ElementAt(4));
        Assert.Equal("ordThat /6", result.ElementAt(5));
        Assert.Equal("Runs /7", result.ElementAt(6));
        Assert.Equal("and /8", result.ElementAt(7));
        Assert.Equal("some /9", result.ElementAt(8));
        Assert.Equal("words /10", result.ElementAt(9));
        Assert.Equal("after /11", result.ElementAt(10));
        Assert.Equal("it /12", result.ElementAt(11));
    }

    [Fact]
    public void LengthLimitedPostFormatter_FormatText_HandlesBreakCodes()
    {
        var formatter = new LengthLimitedPostFormatter(
            network: NetworkType.Any, 
            limit: 10, 
            subsequentLimits: 10, 
            indices: true,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.Inline,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewPost,
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost);

        var message = new SimpleSocialMessage("First $$ Second $$ Third");
        var result = formatter.FormatText(message);
        Assert.Equal(3, result.Count());
        Assert.Equal("First /1", result.ElementAt(0));
        Assert.Equal("Second /2", result.ElementAt(1));
        Assert.Equal("Third /3", result.ElementAt(2));
    }

    [Fact]
    public void LengthLimitedPostFormatter_FormatText_HandlesBigTextWithAllFeatures()
    {
            var formatter = new LengthLimitedPostFormatter(
            network: NetworkType.Any, 
            limit: 100, 
            subsequentLimits: 100, 
            indices: true,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.Inline,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewPost,
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost);

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

        // Console.WriteLine(string.Join("\n", result.Select(s => $"*** {s}")));
        // *** This is a nice long test message with a whole bunch of features /1 #Formatting #LongText #Experiment
        // *** attached. It's important that there are over 100 characters across all the text, because this is /2
        // *** an experiment with long text. https://instantiator.dev /3

        Assert.Equal(3, result.Count());
        Assert.StartsWith("This is a nice long test message with a whole bunch of features /1", result.ElementAt(0));
        Assert.StartsWith("attached. It's important that there are over 100 characters across all the text, because this is /2", result.ElementAt(1));
        Assert.StartsWith("an experiment with long text. https://instantiator.dev /3", result.ElementAt(2));

        // find the tags - but they'll be in a random order
        Assert.Contains("#LongText", result.ElementAt(0));
        Assert.Contains("#Experiment", result.ElementAt(0));
        Assert.Contains("#Formatting", result.ElementAt(0));
    }
}
