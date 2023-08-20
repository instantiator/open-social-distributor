using DistributorLib.Post;

namespace DistributorLib.Network.Implementations;

public class ConsoleNetwork : AbstractNetwork
{
    public ConsoleNetwork() : base(NetworkType.Console, "Console", "Console.Network", PostVariantFactory.Console)
    {
    }

    protected override async Task DisposeClientAsync()
    {
        Console.WriteLine($"{ShortCode} disposed.");
    }

    protected override async Task InitClientAsync()
    {
        Console.WriteLine($"{ShortCode} ready.");
    }

    protected override async Task<bool> TestConnectionImplementationAsync()
    {
        return true;
    }

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message)
    {
        var text = PostVariant.Compose(message);
        Console.WriteLine(text);
        foreach (var image in message.Images ?? Array.Empty<ISocialImage>())
        {
            Console.WriteLine($" - Image Uri:   {image.Uri}");
            Console.WriteLine($" - Description: {image.Description}");
        }
        return new PostResult(this, message, true);
    }
}