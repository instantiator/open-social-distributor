using DistributorLib.Post;
using DistributorLib.Post.Formatters;
using DistributorLib.Post.Images;

namespace DistributorLib.Network;
public abstract class AbstractNetwork : ISocialNetwork
{
    protected AbstractNetwork(NetworkType type, string shortcode, string networkName, IPostFormatter formatter)
    {
        NetworkType = type;
        ShortCode = shortcode;
        NetworkName = networkName;
        Formatter = formatter;
    }

    public NetworkType NetworkType { get; private set; }
    
    public bool DryRunPosting { get; set; } = false;

    public string ShortCode { get; private set; }

    public string NetworkName { get; private set; }

    public IPostFormatter Formatter { get; private set; }

    public bool Initialised { get; private set; } = false;

    public async Task InitAsync()
    {
        Console.WriteLine($"Initialising {ShortCode} ({NetworkType})...");
        await InitClientAsync();
        Initialised = true;
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine($"Disposing {ShortCode} ({NetworkType})...");
        await DisposeClientAsync();
        Initialised = false;
    }

    protected abstract Task InitClientAsync();

    protected abstract Task DisposeClientAsync();

    public async Task<ConnectionTestResult> TestConnectionAsync()
    {
        Console.WriteLine($"Testing {ShortCode} ({NetworkType}) connection...");
        if (!Initialised) return new ConnectionTestResult(this, false, "Network not initialised");
        return await TestConnectionImplementationAsync();
    }

    protected abstract Task<ConnectionTestResult> TestConnectionImplementationAsync();

    public async Task<PostResult> PostAsync(ISocialMessage message)
    {
        try
        {
            Console.WriteLine($"Posting to {ShortCode} ({NetworkType})...");
            var texts = Formatter.FormatText(message);
            var images = AssignImages(message, texts.Count());
            return await PostImplementationAsync(message, texts, images);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.ToString());
            return new PostResult(this, message, false, null, e.Message, e);
        }
    }

    protected abstract Task<PostResult> PostImplementationAsync(ISocialMessage message, IEnumerable<string> texts, IEnumerable<IEnumerable<ISocialImage>> images);

    protected abstract IEnumerable<IEnumerable<ISocialImage>> AssignImages(ISocialMessage message, int posts);

    protected IEnumerable<IEnumerable<ISocialImage>> AssignImagesToFirstPost(ISocialMessage message, int posts, int? max, bool throwIfTooManyImages)
    {
        if (throwIfTooManyImages && message.Images?.Count() > max)
        {
            throw new Exception($"Unable to assign {message.Images?.Count()} images across {posts} posts.");
        }
        var images = max != null ? message.Images?.Take(max.Value) : message.Images;
        return new List<IEnumerable<ISocialImage>>()
        {
            images ?? new List<ISocialImage>() // all images in the first post
        };
    }

    protected IEnumerable<IEnumerable<ISocialImage>> FrontLoadImages(ISocialMessage message, int posts, int maxImagesPerPost, bool throwIfTooManyImages)
    {
        var images = message.Images ?? new List<ISocialImage>();
        var result = new List<List<ISocialImage>>();

        if (maxImagesPerPost < 1) return result;
        var currentGroup = new List<ISocialImage>();
        foreach (var image in images)
        {
            if (currentGroup.Count >= maxImagesPerPost)
            {
                result.Add(currentGroup);
                currentGroup = new List<ISocialImage>();
            }
            currentGroup.Add(image);
        }
        if (currentGroup.Count > 0)
        {
            result.Add(currentGroup);
        }
        if (throwIfTooManyImages && result.Count() > posts)
        {
            throw new Exception($"Unable to assign {images.Count()} images across {posts} posts.");
        }
        return result.Take(posts); // cap at the number of available posts
    }

}