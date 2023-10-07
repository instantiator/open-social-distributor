using DistributorLib.Network;
using DistributorLib.Post;

namespace DistributorLib.Input;

public class PostListFormat
{
    public IEnumerable<PostFormat> Posts { get; set; } = new List<PostFormat>();

    public class PostFormat
    {
        public IEnumerable<PostPartFormat> Parts { get; set; } = new List<PostPartFormat>();
        public IEnumerable<PostImageFormat> Images { get; set; } = new List<PostImageFormat>();
    }

    public class PostPartFormat
    {
        public SocialMessagePart Part { get; set; } = SocialMessagePart.Text;
        public PostContentFormat Content { get; set; } = new PostContentFormat();
    }

    public class PostContentFormat : Dictionary<NetworkType, string>
    {
    }

    public class PostImageFormat
    {
        public string Uri { get; set; } = null!;
        public string? Description { get; set; }
    }
}
