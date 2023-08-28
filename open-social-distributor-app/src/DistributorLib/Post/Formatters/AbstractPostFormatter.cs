using DistributorLib.Network;

namespace DistributorLib.Post.Formatters
{
    public abstract class AbstractPostFormatter : IPostFormatter
    {
        protected AbstractPostFormatter(NetworkType network)
        {
            this.Network = network;
        }

        public NetworkType Network { get; private set; }

        public abstract IEnumerable<string> FormatText(ISocialMessage message);

        public IEnumerable<string> GetTags(ISocialMessage message)
        {
            return message.Parts
                .Where(part => part.Part == SocialMessagePart.Tag)
                .Select(part => part.ToStringFor(Network)!);
        }

        public string? GetLink(ISocialMessage message)
        {
            return message.Parts.FirstOrDefault(part => part.Part == SocialMessagePart.Link)?.ToStringFor(Network);
        }
    }
}