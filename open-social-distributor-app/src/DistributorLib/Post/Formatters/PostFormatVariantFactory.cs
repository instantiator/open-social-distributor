using DistributorLib.Post.Formatters;

namespace DistributorLib.Post.Formatters
{
    public class PostFormatVariantFactory
    {
        public static IPostFormatter Console => new ConsolePostFormatter();
        public static IPostFormatter Mastodon => new LengthLimitedPostFormatter(Network.NetworkType.Mastodon, 500, 500, true, LengthLimitedPostFormatter.BreakBehaviour.NewPost);
        public static IPostFormatter Facebook => new LengthLimitedPostFormatter(Network.NetworkType.Facebook, 63206, 8000, false, LengthLimitedPostFormatter.BreakBehaviour.NewParagraph);
    }
}
