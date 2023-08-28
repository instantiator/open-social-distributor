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
        var included = message.Parts.Where(part => linkInText || part.Part != SocialMessagePart.Link);
        var strings = included.Select(part => part.ToStringFor(Network)).Where(x => x != null).ToList();
        var words = string.Join(' ', strings).Split(' ');

        var messages = new List<string>();
        var currentMessageIndex = 1;
        var currentMessage = new StringBuilder();
        var firstInMessage = true;

        for (int w = 0; w < words.Count(); w++)
        {
            var currentWord = words[w];
            var currentWordLen = currentWord.Length;

            var nextWord = w + 1 < words.Count() ? words[w+1] : null;
            var indexWord = $"/{currentMessageIndex}";
            var followingWordLen = Math.Min(nextWord?.Length ?? 0, indexWord.Length);

            // if we cannot fit the word, append the index word, and start a new message
            if (currentMessage.Length + currentWordLen + 1 >= messageLengthLimit)
            {
                if (!firstInMessage) { currentMessage.Append(" "); }
                currentMessage.Append(indexWord);
                messages.Add(currentMessage.ToString());
                currentMessage = new StringBuilder();
                currentMessageIndex++;
                firstInMessage = true;
            }

            // if the current word is longer than the message length limit, split it up
            if (currentWordLen > messageLengthLimit)
            {
                messages.AddRange(currentWord.Chunk(messageLengthLimit).Select(s => new string(s)).ToList());
            }

            // if we can fit the word, and the next word OR the index word, append the word
            if (currentMessage.Length + currentWordLen + followingWordLen + 2 <= messageLengthLimit)
            {
                if (!firstInMessage) { currentMessage.Append(" "); }
                currentMessage.Append(currentWord);
                firstInMessage = false;
            }
        }

        return messages;
    }
}