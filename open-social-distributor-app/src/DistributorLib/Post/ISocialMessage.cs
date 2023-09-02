using DistributorLib.Post.Images;

namespace DistributorLib.Post;
public interface ISocialMessage
{
    IEnumerable<SocialMessageContent> Parts { get; }
    IEnumerable<ISocialImage>? Images { get; }
}
