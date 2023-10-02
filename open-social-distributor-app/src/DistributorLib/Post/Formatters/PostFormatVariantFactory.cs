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
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.AllPosts,
            indexBehaviour: LengthLimitedPostFormatter.IndexBehaviour.Slash);

        public static IPostFormatter Facebook => new LengthLimitedPostFormatter(
            network: Network.NetworkType.Facebook, 
            limit: 63206, subsequentLimits: 8000, indices: false,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.None,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewParagraph, 
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost,
            indexBehaviour: LengthLimitedPostFormatter.IndexBehaviour.None);

        public static IPostFormatter LinkedIn => new LengthLimitedPostFormatter(
            network: Network.NetworkType.LinkedIn, 
            limit: 3000, subsequentLimits: 1250, indices: false,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewParagraph, 
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost,
            indexBehaviour: LengthLimitedPostFormatter.IndexBehaviour.None);

        public static IPostFormatter Discord => new LengthLimitedPostFormatter(
            network: Network.NetworkType.Discord, 
            limit: 2000, subsequentLimits: 2000, indices: true,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewPost, 
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost,
            indexBehaviour: LengthLimitedPostFormatter.IndexBehaviour.None);

        [Obsolete("Twitter is not a safe platform")]
        public static IPostFormatter Twitter => new LengthLimitedPostFormatter(
            network: Network.NetworkType.Twitter, 
            limit: 280, subsequentLimits: 280, indices: true,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewPost, 
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.AllPosts,
            indexBehaviour: LengthLimitedPostFormatter.IndexBehaviour.Slash);
    }
}
