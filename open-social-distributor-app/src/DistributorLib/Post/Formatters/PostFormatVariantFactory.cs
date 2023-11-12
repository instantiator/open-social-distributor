using DistributorLib.Network;
using DistributorLib.Post.Formatters;

namespace DistributorLib.Post.Formatters
{
    public class PostFormatVariantFactory
    {
        public static IPostFormatter For(NetworkType network)
        {
            return network switch
            {
                NetworkType.Console => Console,
                NetworkType.Mastodon => Mastodon,
                NetworkType.Facebook => Facebook,
                NetworkType.LinkedIn => LinkedIn,
                NetworkType.Discord => Discord,
                NetworkType.Twitter => throw new NotImplementedException(network.ToString()),
                _ => throw new NotImplementedException(network.ToString())
            };
        }

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

        // LinkedIn requires that reserved characters be escaped
        // See: https://learn.microsoft.com/en-us/linkedin/marketing/integrations/community-management/shares/little-text-format?view=li-lms-2023-10#text
        public static IPostFormatter LinkedIn => new LengthLimitedPostFormatter(
            network: Network.NetworkType.LinkedIn, 
            limit: 3000, subsequentLimits: 1250, indices: false,
            linkBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost,
            breakBehaviour: LengthLimitedPostFormatter.BreakBehaviour.NewParagraph, 
            tagBehaviour: LengthLimitedPostFormatter.DecorationBehaviour.FirstPost,
            indexBehaviour: LengthLimitedPostFormatter.IndexBehaviour.None,
            escapeCharacters: "\\|{}@[]()<>#*_~".ToCharArray());

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
