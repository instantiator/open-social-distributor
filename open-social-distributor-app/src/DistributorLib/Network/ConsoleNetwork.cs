using DistributorLib.Post;

namespace DistributorLib.Network;

public class ConsoleNetwork : AbstractNetwork
{
    public ConsoleNetwork() : base("Console", "Console.Network")
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
        Console.WriteLine($"{ShortCode} posting message to {NetworkName}...");
        Console.WriteLine($" - Message: {message.Message}");
        foreach (var image in message.Images ?? Array.Empty<ISocialImage>())
        {
            Console.WriteLine($" - Image Uri:   {image.Uri}");
            Console.WriteLine($" - Description: {image.Description}");
        }
        return new PostResult(this, message, true);
    }
}