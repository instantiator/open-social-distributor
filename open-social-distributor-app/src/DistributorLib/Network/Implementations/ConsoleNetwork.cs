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

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message, IEnumerable<string> texts, IEnumerable<IEnumerable<ISocialImage>> images)
    {
        for (int t = 0; t < texts.Count(); t++)
        {
            Console.WriteLine($"Post {t}: Text: {texts.ElementAt(t)}");
            if (images.Count() > t && images.ElementAt(t) != null && images.ElementAt(t).Count() > 0)
            {
                var postImages = images.ElementAt(t);
                for (int i = 0; i < postImages.Count(); i++)
                {
                    var image = postImages.ElementAt(i);
                    Console.WriteLine($"Post {t}: Image {i} uri: {image.Source}");
                    Console.WriteLine($"Post {t}: Image {i} description: {image.Description}");
                    using (var stream = await image.GetStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        var len = (await reader.ReadToEndAsync()).Length;
                        Console.WriteLine($"Post {t}: Image {i} size: {len} bytes");
                    }
                }
            }
        }
        var counter = 0; var ids = Enumerable.Repeat($"console-post-{counter++}", texts.Count()).ToList();
        return new PostResult(this, message, true, ids);
    }

    protected override IEnumerable<IEnumerable<ISocialImage>> AssignImages(ISocialMessage message, int posts)
    {
        return AssignImagesToFirstPost(message, posts, null, false);
    }
}