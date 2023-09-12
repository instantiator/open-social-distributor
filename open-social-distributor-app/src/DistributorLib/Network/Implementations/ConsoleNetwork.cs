using DistributorLib.Post;
using DistributorLib.Post.Formatters;
using DistributorLib.Post.Images;

namespace DistributorLib.Network.Implementations;

public class ConsoleNetwork : AbstractNetwork
{
    public ConsoleNetwork() : base(NetworkType.Console, "Console", "Console.Network", PostFormatVariantFactory.Console)
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

    protected override async Task<ConnectionTestResult> TestConnectionImplementationAsync()
    {
        return new ConnectionTestResult(this, true, $"{Environment.MachineName}", "Console network is always connected.");
    }

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message)
    {
        var text = Formatter.FormatText(message);
        Console.WriteLine(string.Join("\n", text));

        foreach (var image in message.Images ?? Array.Empty<ISocialImage>())
        {
            Console.WriteLine($"Image uri: {image.SourceUri}");
            Console.WriteLine($"Image description: {image.Description}");
            
            using (var stream = await image.GetStreamAsync())
            using (var reader = new StreamReader(stream))
            {
                var len = (await reader.ReadToEndAsync()).Length;
                Console.WriteLine($"Image size: {len} bytes");
            }
        }
        var counter = 0;
        var ids = Enumerable.Repeat($"console-post-{counter++}", text.Count()).ToList();
        return new PostResult(this, message, true, ids);
    }
}