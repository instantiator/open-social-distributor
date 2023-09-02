using DistributorLib.Post.Formatters;

namespace DistributorLib.Post.Formatters
{
    public class PostFormatVariantFactory
    {
        public static IPostFormatter Console => new ConsolePostFormatter();

        public static IPostFormatter Mastodon => new LengthLimitedPostFormatter(
            network: Network.NetworkType.Mastodon, 
            limit: 500, subsequentLimits: 500,
            linkInText: true,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewPost,
            tagBehaviour: LengthLimitedPostFormatter.TagBehaviour.FirstPost);

        public static IPostFormatter Facebook => new LengthLimitedPostFormatter(
            network: Network.NetworkType.Facebook, 
            limit: 63206, subsequentLimits: 8000, 
            linkInText: false, 
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewParagraph, 
            tagBehaviour: LengthLimitedPostFormatter.TagBehaviour.FirstPost);
    }
}
