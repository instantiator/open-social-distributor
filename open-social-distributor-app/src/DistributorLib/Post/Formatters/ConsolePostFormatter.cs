using DistributorLib.Network;

namespace DistributorLib.Post.Formatters
{
    public class ConsolePostFormatter : AbstractPostFormatter
    {
        public ConsolePostFormatter() : base(NetworkType.Console)
        {
        }

        public override IEnumerable<string> FormatText(ISocialMessage message)
        {
            // a single message, containing a description of all the parts
            return new[] { 
                string.Join("\n", message.Parts.Select(part => $"{part.Part}: {part.ToStringFor(NetworkType.Console)}")) 
            };
        }
    }
}
