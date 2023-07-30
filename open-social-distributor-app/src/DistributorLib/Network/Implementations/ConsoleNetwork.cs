using DistributorLib.Post;

namespace DistributorLib.Network.Implementations;

public class ConsoleNetwork : AbstractNetwork
{
    public ConsoleNetwork() : base("Console", "Console.Network", PostVariantFactory.Console)
    {
    }

    public override async ValueTask DisposeAsync()
    {
        Console.WriteLine($"Disposing {ShortCode}...");
    }

    public override async Task InitClientAsync()
    {
        Console.WriteLine($"Initialising {ShortCode}...");
    }

    public override async Task<PostResult> PostAsync(ISocialMessage message)
    {
        var text = PostVariant.Compose(message);
        Console.WriteLine($"{ShortCode} posting message to {NetworkName}...");
        Console.WriteLine($" - Message: {text}");
        foreach (var image in message.Images ?? Array.Empty<ISocialImage>())
        {
            Console.WriteLine($" - Image Uri:   {image.Uri}");
            Console.WriteLine($" - Description: {image.Description}");
        }
        return new PostResult(this, message, true);
    }
}