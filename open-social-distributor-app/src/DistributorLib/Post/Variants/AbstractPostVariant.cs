namespace DistributorLib.Post.Variants
{
    public abstract class AbstractPostVariant : IPostVariant
    {
        protected AbstractPostVariant(int limit)
        {
            this.MessageLengthLimit = limit;
        }

        public int MessageLengthLimit { get; private set; }

        public abstract string Compose(ISocialMessage message);
    }
}