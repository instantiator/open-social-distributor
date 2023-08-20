using DistributorLib.Post.Variants;

namespace DistributorLib.Post
{
    public class PostVariantFactory
    {
        public static IPostVariant Console => new ConsolePostVariant();
        public static IPostVariant Mastodon => new ConsolePostVariant();
        public static IPostVariant LinkedIn => throw new NotImplementedException();
    }
}