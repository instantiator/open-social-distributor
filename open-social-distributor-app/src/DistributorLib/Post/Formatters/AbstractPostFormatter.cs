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
    }
}