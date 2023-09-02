using DistributorLib.Post.Formatters;

namespace DistributorLib.Post.Formatters
{
    public class PostFormatVariantFactory
    {
        public static IPostFormatter Console => new ConsolePostFormatter();

        public static IPostFormatter Mastodon => new LengthLimitedPostFormatter(
            network: Network.NetworkType.Mastodon, 
            limit: 500, subsequentLimits: 500, indices: true,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewPost,
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.AllPosts);

        public static IPostFormatter Facebook => new LengthLimitedPostFormatter(
            network: Network.NetworkType.Facebook, 
            limit: 63206, subsequentLimits: 8000, indices: false,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.None,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewParagraph, 
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost);
    }
}
