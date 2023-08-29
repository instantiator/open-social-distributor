using System.Text;
using DistributorLib.Network;

namespace DistributorLib.Post.Formatters;

public class LengthLimitedPostFormatter : AbstractPostFormatter
{
    protected bool linkInText;
    protected int messageLengthLimit;

    public LengthLimitedPostFormatter(NetworkType network, int limit, bool linkInText) : base(network)
    {
        this.messageLengthLimit = limit;
        this.linkInText = linkInText;
    }

    public override IEnumerable<string> FormatText(ISocialMessage message)
    {
        return new WordWrapFormatter()
            .WithNetwork(Network)
            .WithLimit(messageLengthLimit)
            .WithMessage(message)
            .WithLinkInText(linkInText)
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
        private bool link;

        public WordWrapFormatter WithNetwork(NetworkType network) { this.network = network; return this; }
        public WordWrapFormatter WithLimit(int limit) { this.limit = limit; return this; }
        public WordWrapFormatter WithMessage(ISocialMessage message) { this.message = message; return this; }
        public WordWrapFormatter WithLinkInText(bool link) { this.link = link; return this; }

        public IEnumerable<string> Build()
        {
            var included = message!.Parts.Where(part => link || part.Part != SocialMessagePart.Link);
            var strings = included.Select(part => part.ToStringFor(network)).Where(x => x != null).ToList();
            var words = string.Join(' ', strings).Split(' ');

            Messages = new List<string>();
            currentMessageIndex = 1;
            currentMessage = new StringBuilder();

            for (int i = 0; i < words.Length; i++)
            {
                PerformAction(words, i);
            }
            CompleteMessage();
            return Messages;
        }

        private void PerformAction(IEnumerable<string> words, int index)
        {
            var word = words.ElementAt(index);
            var nextWord = index + 1 < words.Count() ? words.ElementAt(index+1) : null;

            var decision = DetermineAction(currentMessage!, word, nextWord, IndexWord(currentMessageIndex));
            // Console.WriteLine($"Decision: {decision} for: {word}");
            switch (decision)
            {
                case Decision.AddWord:
                    currentMessage!.Append($"{(currentMessage!.Length == 0 ? "" : " ")}{word}");
                    break;
                case Decision.AddIndex:
                    currentMessage!.Append($"{(currentMessage!.Length == 0 ? "" : " ")}{IndexWord(currentMessageIndex)}");
                    CompleteMessage();
                    PerformAction(words, index);
                    break;
                case Decision.AddNothing:
                    CompleteMessage();
                    PerformAction(words, index);
                    break;
                case Decision.VeryLongWord:
                    CompleteMessage();
                    var chunks = word.Chunk(limit).Select(s => new string(s)).ToList();
                    foreach (var chunk in chunks)
                    {
                        currentMessage!.Append(chunk);
                        CompleteMessage();
                    }
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

        private enum Decision { AddWord, AddIndex, VeryLongWord, AddNothing, Finish }

        private Decision DetermineAction(StringBuilder message, string? thisWord, string? nextWord, string indexWord)
        {
            if (thisWord == null) return Decision.Finish;
            bool firstWordInMessage = message.Length == 0;
            int prefixChars = firstWordInMessage ? 0 : 1;
            bool canFitThisWord = message.Length + thisWord!.Length + prefixChars <= limit;
            bool canFitIndexWord = message.Length + indexWord.Length + prefixChars <= limit;
            bool canFitThisAndNextWord = nextWord == null ? false : message.Length + thisWord!.Length + nextWord!.Length + prefixChars + 1 <= limit;
            bool canFitThisAndIndexWord = message.Length + thisWord!.Length + indexWord.Length + prefixChars + 1 <= limit;
            bool isVeryLongWord = thisWord.Length > limit;

            if (canFitThisAndNextWord || canFitThisAndIndexWord) return Decision.AddWord;
            if (message.Length == 0 && canFitThisWord) return Decision.AddWord;
            if (message.Length == 0 && isVeryLongWord) return Decision.VeryLongWord;

            return canFitIndexWord
                ? Decision.AddIndex
                : Decision.AddNothing; // shouldn't happen, might happen I guess
        }

    }
}