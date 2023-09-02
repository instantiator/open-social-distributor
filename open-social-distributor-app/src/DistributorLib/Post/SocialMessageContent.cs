using DistributorLib.Network;

namespace DistributorLib.Post;

public class SocialMessageContent
{
    public SocialMessageContent(string value, NetworkType type = NetworkType.Any, SocialMessagePart part = SocialMessagePart.Text)
    {
        Content.Add(type, value);
        Part = part;
    }

    public SocialMessageContent(Dictionary<NetworkType, string> content, SocialMessagePart part = SocialMessagePart.Text)
    {
        Content = content;
        Part = part;
    }

    public SocialMessagePart Part {get;set;} = SocialMessagePart.Text;
    public Dictionary<NetworkType,string> Content { get; set; } = new Dictionary<NetworkType, string>();

    public string? ToStringFor(NetworkType type)
    {
        var text = Content.Count == 0
            ? null
            : type == NetworkType.Any
                ? Content.First().Value
                : Content.ContainsKey(type) 
                    ? Content[type] 
                    : Content.ContainsKey(NetworkType.Any) 
                        ? Content[NetworkType.Any] 
                        : null;

        return Part == SocialMessagePart.Tag ? $"#{text}" : text;
    }    
}
