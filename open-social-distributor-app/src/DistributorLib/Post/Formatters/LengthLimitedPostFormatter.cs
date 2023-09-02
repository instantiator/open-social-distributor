using System.Text;
using DistributorLib.Network;

namespace DistributorLib.Post.Formatters;

public class LengthLimitedPostFormatter : AbstractPostFormatter
{
    public const string BREAK_CODE = "$$";
    public const decimal DEFAULT_TAG_COVERAGE = 0.5m;
    public enum BreakBehaviour { None, NewParagraph, NewPost }
    public enum TagBehaviour { FirstPost, AllPosts, Inline, None }

    protected bool linkInText;
    protected int messageLengthLimit;
    protected int subsequentLimits;
    protected decimal acceptableTagCoverage;
    protected BreakBehaviour breakBehaviour;
    protected TagBehaviour tagBehaviour;

    public LengthLimitedPostFormatter(NetworkType network, 
        int limit, int subsequentLimits,
        bool linkInText, 
        BreakBehaviour breakBehaviour = BreakBehaviour.NewPost,
        TagBehaviour tagBehaviour = TagBehaviour.FirstPost,
        decimal acceptableTagCoverage = DEFAULT_TAG_COVERAGE) : base(network)
    {
        this.messageLengthLimit = limit;
        this.subsequentLimits = subsequentLimits;
        this.acceptableTagCoverage = acceptableTagCoverage;
        this.linkInText = linkInText;
        this.breakBehaviour = breakBehaviour;
        this.tagBehaviour = tagBehaviour;
    }

    public override IEnumerable<string> FormatText(ISocialMessage message)
    {
        return new WordWrapFormatter()
            .WithNetwork(Network)
            .WithLimit(messageLengthLimit)
            .WithSubsequentLimits(subsequentLimits)
            .WithLinkInText(linkInText)
            .WithBreakBehaviour(breakBehaviour, BREAK_CODE)
            .WithTagBehaviour(tagBehaviour)
            .WithAcceptableTagCoverage(acceptableTagCoverage)
            .WithMessage(message)
            .Build();
    }

    public class WordWrapFormatter
    {
        public List<string>? Messages { get; private set; }

        private int currentMessageIndex;
        private StringBuilder? currentMessage;
        private ISocialMessage? message;
        private NetworkType network;
        private int limit;
        private int subsequentLimits;
        private decimal acceptableTagCoverage = DEFAULT_TAG_COVERAGE;
        private bool link;
        private string msgBreak = BREAK_CODE;
        private BreakBehaviour msgBreakBehaviour;
        private TagBehaviour tagBehaviour;

        public WordWrapFormatter WithNetwork(NetworkType network) { this.network = network; return this; }
        public WordWrapFormatter WithLimit(int limit) { this.limit = limit; return this; }
        public WordWrapFormatter WithSubsequentLimits(int subsequentLimits) { this.subsequentLimits = subsequentLimits; return this; }
        public WordWrapFormatter WithAcceptableTagCoverage(decimal coverage) { this.acceptableTagCoverage = coverage; return this; }
        public WordWrapFormatter WithMessage(ISocialMessage message) { this.message = message; return this; }
        public WordWrapFormatter WithLinkInText(bool link) { this.link = link; return this; }
        public WordWrapFormatter WithBreakBehaviour(BreakBehaviour behaviour, string msgBreak) { this.msgBreak = msgBreak; this.msgBreakBehaviour = behaviour; return this; }
        public WordWrapFormatter WithTagBehaviour(TagBehaviour behaviour) { this.tagBehaviour = behaviour; return this; }

        public IEnumerable<string> Build()
        {
            var included = message!.GetMessageParts(link, tagBehaviour == TagBehaviour.Inline);
            var strings = included.Select(part => part.ToStringFor(network)).Where(x => x != null).ToList();
            var words = string.Join(' ', strings).Split(' ');
            var tagWords = message!.Tags.Select(tag => tag.ToStringFor(network)).Where(x => x != null).Select(x => x!);
            
            Messages = new List<string>();
            currentMessageIndex = 1;
            currentMessage = new StringBuilder();

            for (int i = 0; i < words.Length; i++)
            {
                var currentLimit = Messages.Count() == 0 ? limit : subsequentLimits;
                if ((tagBehaviour == TagBehaviour.FirstPost && currentMessageIndex == 1) || tagBehaviour == TagBehaviour.AllPosts)
                {
                    currentLimit -= GetMaxTagsetLength(currentLimit, acceptableTagCoverage, tagWords!); // squeeze out some tag space
                }

                PerformAction(words, i, currentLimit);
            }
            
            // finish last message
            CompleteMessage();

            // apply tags
            for (var m = 0; m < Messages.Count(); m++)
            {
                var currentLimit = m == 0 ? limit : subsequentLimits;
                var reshuffled = tagWords.OrderBy(t => Guid.NewGuid());
                if ((m == 0 && tagBehaviour == TagBehaviour.FirstPost) || tagBehaviour == TagBehaviour.AllPosts)
                {
                    foreach (var tagWord in reshuffled)
                    {
                        if (Messages[m].Length + tagWord!.Length + 1 <= currentLimit)
                        {
                            Messages[m] = $"{Messages[m]} {tagWord}";
                        }
                    }
                }
            }

            return Messages;
        }

        private int GetMaxTagsetLength(int maxLimit, decimal acceptableCoverage, IEnumerable<string> tagWords)
            => Math.Min(
                string.Join(' ', tagWords).Length, 
                (int)(maxLimit * acceptableCoverage));

        private void PerformAction(IEnumerable<string> words, int index, int currentLimit)
        {
            var word = words.ElementAt(index);
            var nextWord = index + 1 < words.Count() ? words.ElementAt(index+1) : null;

            var decision = DetermineAction(currentMessage!, currentLimit, word, nextWord, IndexWord(currentMessageIndex));
            // Console.WriteLine($"Decision: {decision} for: {word}");
            switch (decision)
            {
                case Decision.AddWord:
                    currentMessage!.Append($"{(currentMessage!.Length == 0 ? "" : " ")}{word}");
                    break;
                case Decision.AddIndex:
                    currentMessage!.Append($"{(currentMessage!.Length == 0 ? "" : " ")}{IndexWord(currentMessageIndex)}");
                    CompleteMessage();
                    PerformAction(words, index, currentLimit);
                    break;
                case Decision.AddNothing:
                    CompleteMessage();
                    PerformAction(words, index, currentLimit);
                    break;
                case Decision.NewParagraph:
                    currentMessage!.Append("\n\n");
                    break;
                case Decision.NewPost:
                    currentMessage!.Append($"{(currentMessage!.Length == 0 ? "" : " ")}{IndexWord(currentMessageIndex)}");
                    CompleteMessage();
                    break;
                case Decision.VeryLongWord:
                    CompleteMessage();
                    var chunks = word.Chunk(currentLimit).Select(s => new string(s)).ToList();
                    foreach (var chunk in chunks)
                    {
                        currentMessage!.Append(chunk);
                        CompleteMessage();
                    }
                    break;
                case Decision.Ignore:
                    break;
                case Decision.Finish:
                    CompleteMessage();
                    break;
            }
        }

        private string IndexWord(int index) => $"/{index}";

        private void CompleteMessage()
        {
            if (currentMessage!.Length > 0) 
            {
                Messages!.Add(currentMessage!.ToString());
                currentMessageIndex++;
            }
            currentMessage = new StringBuilder();
        }

        private enum Decision { Ignore, NewParagraph, NewPost, AddWord, AddIndex, VeryLongWord, AddNothing, Finish }

        private Decision DetermineAction(StringBuilder message, int currentLimit, string? thisWord, string? nextWord, string indexWord)
        {
            if (thisWord == null) return Decision.Finish;

            if (thisWord == msgBreak)
            {
                switch (msgBreakBehaviour)
                {
                    case BreakBehaviour.None:
                        return Decision.Ignore;
                    case BreakBehaviour.NewParagraph:
                        return Decision.NewParagraph;
                    case BreakBehaviour.NewPost:
                        return Decision.NewPost;
                    default:
                        throw new NotImplementedException($"Break behaviour not implemented: {msgBreakBehaviour}");
                }
            }

            bool firstWordInMessage = message.Length == 0;
            int prefixChars = firstWordInMessage ? 0 : 1;
            bool canFitThisWord = message.Length + thisWord!.Length + prefixChars <= currentLimit;
            bool canFitIndexWord = message.Length + indexWord.Length + prefixChars <= currentLimit;
            bool canFitThisAndNextWord = nextWord == null ? false : message.Length + thisWord!.Length + nextWord!.Length + prefixChars + 1 <= currentLimit;
            bool canFitThisAndIndexWord = message.Length + thisWord!.Length + indexWord.Length + prefixChars + 1 <= currentLimit;
            bool isVeryLongWord = thisWord.Length > currentLimit;

            if (canFitThisAndNextWord || canFitThisAndIndexWord) return Decision.AddWord;
            if (message.Length == 0 && canFitThisWord) return Decision.AddWord;
            if (message.Length == 0 && isVeryLongWord) return Decision.VeryLongWord;

            return canFitIndexWord
                ? Decision.AddIndex
                : Decision.AddNothing; // shouldn't happen, might happen I guess
        }

    }
}