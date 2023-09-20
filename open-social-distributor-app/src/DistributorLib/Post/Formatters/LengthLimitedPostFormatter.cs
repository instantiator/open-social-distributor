using System.Text;
using System.Text.RegularExpressions;
using DistributorLib.Network;

namespace DistributorLib.Post.Formatters;

public class LengthLimitedPostFormatter : AbstractPostFormatter
{
    public const string BREAK_CODE = "$$";
    public const decimal DEFAULT_TAG_COVERAGE = 0.5m;
    public enum BreakBehaviour { None, NewParagraph, NewPost }
    public enum DecorationBehaviour { FirstPost, AllPosts, Inline, None }
    public enum IndexBehaviour { Slash, Parentheses, SquareBrackets, None }

    protected int messageLengthLimit;
    protected int subsequentLimits;
    protected bool indices;
    protected decimal acceptableTagCoverage;
    protected BreakBehaviour breakBehaviour;
    protected DecorationBehaviour tagBehaviour;
    protected DecorationBehaviour linkBehaviour;
    protected IndexBehaviour indexBehaviour;

    public LengthLimitedPostFormatter(NetworkType network, 
        int limit, int subsequentLimits,
        bool indices,
        DecorationBehaviour linkBehaviour = DecorationBehaviour.FirstPost, 
        BreakBehaviour breakBehaviour = BreakBehaviour.NewPost,
        DecorationBehaviour tagBehaviour = DecorationBehaviour.FirstPost,
        IndexBehaviour indexBehaviour = IndexBehaviour.Slash,
        decimal acceptableTagCoverage = DEFAULT_TAG_COVERAGE) : base(network)
    {
        this.messageLengthLimit = limit;
        this.subsequentLimits = subsequentLimits;
        this.indices = indices;
        this.linkBehaviour = linkBehaviour;
        this.breakBehaviour = breakBehaviour;
        this.tagBehaviour = tagBehaviour;
        this.indexBehaviour = indexBehaviour;
        this.acceptableTagCoverage = acceptableTagCoverage;
    }

    public override IEnumerable<string> FormatText(ISocialMessage message)
    {
        return new WordWrapFormatter()
            .WithNetwork(Network)
            .WithLimit(messageLengthLimit)
            .WithSubsequentLimits(subsequentLimits)
            .WithBreakBehaviour(breakBehaviour, BREAK_CODE)
            .WithTagBehaviour(tagBehaviour)
            .WithLinkBehaviour(linkBehaviour)
            .WithIndexBehaviour(indexBehaviour)
            .WithAcceptableTagCoverage(acceptableTagCoverage)
            .WithIndices(indices)
            .Build(message);
    }

    public class WordWrapFormatter
    {
        public List<string> Posts { get; private set; } = new List<string>();

        private int currentMessageIndex = 1;
        private StringBuilder currentMessage = new StringBuilder();
        private int currentLimit = 100;
        private NetworkType network = NetworkType.Any;
        private int limit = 100;
        private int subsequentLimits = 100;
        private decimal acceptableTagCoverage = DEFAULT_TAG_COVERAGE;
        private string msgBreak = BREAK_CODE;
        private bool indices = true;
        private BreakBehaviour msgBreakBehaviour = BreakBehaviour.NewPost;
        private DecorationBehaviour tagBehaviour = DecorationBehaviour.AllPosts;
        private DecorationBehaviour linkBehaviour = DecorationBehaviour.FirstPost;
        private IndexBehaviour indexBehaviour = IndexBehaviour.Slash;

        private IEnumerable<string> words = new List<string>();
        private IEnumerable<string> tagWords = new List<string>();
        private string? link = null;

        public WordWrapFormatter WithNetwork(NetworkType network) { this.network = network; return this; }
        public WordWrapFormatter WithLimit(int limit) { this.limit = limit; return this; }
        public WordWrapFormatter WithSubsequentLimits(int subsequentLimits) { this.subsequentLimits = subsequentLimits; return this; }
        public WordWrapFormatter WithAcceptableTagCoverage(decimal coverage) { this.acceptableTagCoverage = coverage; return this; }
        public WordWrapFormatter WithLinkBehaviour(DecorationBehaviour behaviour) { this.linkBehaviour = behaviour; return this; }
        public WordWrapFormatter WithBreakBehaviour(BreakBehaviour behaviour, string msgBreak) { this.msgBreak = msgBreak; this.msgBreakBehaviour = behaviour; return this; }
        public WordWrapFormatter WithTagBehaviour(DecorationBehaviour behaviour) { this.tagBehaviour = behaviour; return this; }
        public WordWrapFormatter WithIndexBehaviour(IndexBehaviour behaviour) { this.indexBehaviour = behaviour; return this; }
        public WordWrapFormatter WithIndices(bool indices) { this.indices = indices; return this; }

        public void Reset(IEnumerable<string> words, string? link, IEnumerable<string> tagWords)
        {
            this.words = words;
            this.link = link;
            this.tagWords = tagWords;

            Posts.Clear();
            currentMessageIndex = 1;
            currentMessage = new StringBuilder();

            currentLimit = CalculateCurrentLimit(tagWords, link);
        }

        public IEnumerable<string> Build(ISocialMessage message)
        {
            var included = message.GetMessageParts(linkBehaviour == DecorationBehaviour.Inline, tagBehaviour == DecorationBehaviour.Inline);
            var strings = included.Select(part => part.ToStringFor(network)).Where(x => x != null).ToList();
            
            var words = string.Join(' ', strings)
                .Split(new[] { ' ' })
                .SelectMany(s => Regex.Split(s, @"(\n|\r|\r\n)"));
            
            var tagWords = message.Tags.Select(tag => tag.ToStringFor(network)).Where(x => x != null).Select(x => x!);
            var link = message.Link?.ToStringFor(network);

            // init
            Reset(words, link, tagWords);

            // add words 1 at a time
            foreach (var word in words)
            {
                PerformAction(word);
            }            

            // finish last message
            CompleteMessage();

            // apply all decorations
            if (Posts.Count < 2) { indices = false; }
            for (var m = 0; m < Posts.Count(); m++)
            {
                var tags = tagWords.OrderBy(t => Guid.NewGuid()).ToList();
                AddWordIfFits(link, m, linkBehaviour);
                AddWordIfFits(IndexWord(m + 1), m, indices ? DecorationBehaviour.AllPosts : DecorationBehaviour.None);
                AddWordsIfFit(tags, m, tagBehaviour);
            }
            return Posts;
        }

        private void AddWordsIfFit(IEnumerable<string> set, int index, DecorationBehaviour behaviour)
        {
            foreach (var word in set)
            {
                AddWordIfFits(word, index, behaviour);
            }
        }

        private void AddWordIfFits(string? word, int index, DecorationBehaviour behaviour)
        {
            if (word == null) { return; }
            var first = index == 0;
            if ((behaviour == DecorationBehaviour.FirstPost && first) || behaviour == DecorationBehaviour.AllPosts)
            {
                var mLimit = first ? limit : subsequentLimits;
                var gap = Posts[index].Length == 0 ? "" : " ";
                if (Posts[index].Length + word.Length + gap.Length <= mLimit)
                {
                    Posts[index] = $"{Posts[index]}{gap}{word}";
                }
            }
        }

        private int GetMaxTagsetLength(int maxLimit, decimal acceptableCoverage, IEnumerable<string> tagWords)
            => Math.Min(
                string.Join(' ', tagWords).Length, 
                (int)(maxLimit * acceptableCoverage));

        private int CalculateCurrentLimit(IEnumerable<string> tagWords, string? link)
        {
            var postLimit = currentMessageIndex == 1 ? limit : subsequentLimits;

            if (tagBehaviour == DecorationBehaviour.AllPosts ||
                (tagBehaviour == DecorationBehaviour.FirstPost && currentMessageIndex == 1))
            {
                postLimit -= GetMaxTagsetLength(currentLimit, acceptableTagCoverage, tagWords!);
            }
            if (linkBehaviour == DecorationBehaviour.AllPosts ||
                (linkBehaviour == DecorationBehaviour.FirstPost && currentMessageIndex == 1))
            {
                postLimit -= link == null ? 0 : 1 + link!.Length;
            }
            if (indices)
            {
                postLimit -= 1 + IndexWord(currentMessageIndex).Length;
            }
            return postLimit;
        }

        private void PerformAction(string word)
        {
            var blankChars = new List<char>{'\n','\r',' '};
            var isBlank = word.All(c => blankChars.Contains(c));
            var isFirstWord = currentMessage.Length == 0;
            var lastCharIsNewline = currentMessage.Length > 0 && currentMessage[currentMessage.Length - 1] == '\n';
            var gap = isBlank || isFirstWord || lastCharIsNewline ? "" : " ";

            var wordNoWhitespace = word.Trim(blankChars.ToArray());
            if (wordNoWhitespace == msgBreak && msgBreakBehaviour == BreakBehaviour.NewPost)
            {
                CompleteMessage();
                return;
            }

            var fits = currentMessage.Length + gap.Length + word.Trim(' ').Length <= currentLimit;
            if (fits)
            {
                if (wordNoWhitespace == msgBreak && msgBreakBehaviour == BreakBehaviour.NewParagraph) 
                { 
                    currentMessage.Append("\n\n"); 
                }
                else
                {
                    currentMessage.Append($"{gap}{word.Trim(' ')}");
                }
            }
            else
            {
                CompleteMessage();

                // if the word is too long, split it up
                if (word.Length > currentLimit)
                {
                    var chunks = word.Chunk(currentLimit).Select(s => new string(s)).ToList();
                    var chunk = word.Substring(0, currentLimit);
                    var remainder = word.Substring(chunk.Length);
                    currentMessage.Append(chunk);
                    CompleteMessage();
                    PerformAction(remainder);
                }
                else
                {
                    // regular word - goes into the next message
                    PerformAction(word);
                }
            }
        }

        private string IndexWord(int index)
        {
            switch (indexBehaviour)
            {
                case IndexBehaviour.Slash:
                    return $"/{index}";
            
                case IndexBehaviour.Parentheses:
                    return $"({index})";

                case IndexBehaviour.SquareBrackets:
                    return $"[{index}]";

                case IndexBehaviour.None:
                    return "";

                default:
                    throw new Exception($"Unknown index behaviour: {indexBehaviour}");
            }
        } 

        private void CompleteMessage()
        {
            if (currentMessage!.Length > 0) 
            {
                Posts!.Add(currentMessage!.ToString());
                currentMessageIndex++;
            }
            currentMessage = new StringBuilder();
            currentLimit = CalculateCurrentLimit(tagWords, link);
        }
    }
}