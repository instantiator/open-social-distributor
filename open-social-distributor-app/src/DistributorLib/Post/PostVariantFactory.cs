using DistributorLib.Post.Formatters;

namespace DistributorLib.Post
{
    public class PostVariantFactory
    {
        public static IPostFormatter Console => new ConsolePostFormatter();
        public static IPostFormatter Mastodon => new ConsolePostFormatter();
        public static IPostFormatter Facebook => new ConsolePostFormatter();
    }
}