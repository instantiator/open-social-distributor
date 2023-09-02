using DistributorLib.Network;
using DistributorLib.Post.Images;

namespace DistributorLib.Post;

public class SimpleSocialMessage : ISocialMessage
{
    public SimpleSocialMessage(string message, IEnumerable<ISocialImage>? images = null, string? link = null, IEnumerable<string>? tags = null)
    {
        this.parts = new List<SocialMessageContent>() { new SocialMessageContent(message, NetworkType.Any, SocialMessagePart.Text) };
        if (link != null) { this.parts.Add(new SocialMessageContent(link, NetworkType.Any, SocialMessagePart.Link)); }
        if (tags != null) { this.parts.AddRange(tags.Select(t => new SocialMessageContent(t, NetworkType.Any, SocialMessagePart.Tag))); }
        this.images = images?.ToList() ?? new List<ISocialImage>();
    }

    public SimpleSocialMessage(IEnumerable<SocialMessageContent> parts, IEnumerable<ISocialImage>? images = null)
    {
        this.parts.AddRange(parts);
        if (images != null) { this.images.AddRange(images); }
    }

    private List<SocialMessageContent> parts { get; set; } = new List<SocialMessageContent>();
    private List<ISocialImage>? images {get; set; } = new List<ISocialImage>();

    public IEnumerable<SocialMessageContent> Parts => parts;
    public IEnumerable<ISocialImage>? Images => images;
}