using DistributorLib.Network;

namespace DistributorLib.Post.Variants
{
    public class ConsolePostVariant : AbstractPostVariant
    {
        public ConsolePostVariant() : base(int.MaxValue)
        {
        }

        public override string Compose(ISocialMessage message)
        {
            return string.Join("\n", message.Parts.Select(part => $"{part.Part}: {part.ToStringFor(NetworkType.Console)}"));
        }
    }
}
