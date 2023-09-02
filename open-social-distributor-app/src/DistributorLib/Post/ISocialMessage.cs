using DistributorLib.Network;
using DistributorLib.Post.Images;

namespace DistributorLib.Post;
public interface ISocialMessage
{
    IEnumerable<SocialMessageContent> Parts { get; }
    IEnumerable<ISocialImage>? Images { get; }
    IEnumerable<SocialMessageContent> Tags { get; }
    SocialMessageContent? Link { get; }
    IEnumerable<SocialMessageContent> GetMessageParts(bool includeLink, bool includeTags);
}
