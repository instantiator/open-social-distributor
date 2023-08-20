using DistributorLib.Post.Variants;

namespace DistributorLib.Post
{
    public class PostVariantFactory
    {
        public static IPostVariant Console => new ConsolePostVariant();
        public static IPostVariant Mastodon => throw new NotImplementedException();
        public static IPostVariant LinkedIn => throw new NotImplementedException();
    }
}