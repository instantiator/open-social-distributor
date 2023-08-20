using DistributorLib.Network;

namespace DistributorLib.Post;

public class SimpleSocialMessage : ISocialMessage
{
    public SimpleSocialMessage(string message, IEnumerable<ISocialImage>? images = null)
    {
        parts = new List<SocialMessageContent>() { new SocialMessageContent(message, NetworkType.Any, SocialMessagePart.Text) };
        images = images ?? new List<ISocialImage>();
    }

    public SimpleSocialMessage(IEnumerable<SocialMessageContent> parts, IEnumerable<ISocialImage>? images = null)
    {
        this.parts.AddRange(parts);
        this.images.AddRange(images ?? new List<ISocialImage>());
    }

    private List<SocialMessageContent> parts { get; set; } = new List<SocialMessageContent>();
    private List<ISocialImage> images {get; set; } = new List<ISocialImage>();

    public IEnumerable<SocialMessageContent> Parts => parts;
    public IEnumerable<ISocialImage> Images => images;
}