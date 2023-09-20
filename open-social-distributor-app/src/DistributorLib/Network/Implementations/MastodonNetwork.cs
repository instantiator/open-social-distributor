using DistributorLib;
using DistributorLib.Post;
using DistributorLib.Post.Formatters;
using DistributorLib.Post.Images;
using Mastonet;
using Mastonet.Entities;

namespace DistributorLib.Network.Implementations;
public class MastodonNetwork : AbstractNetwork
{
    private string token;
    private MastodonClient? client;

    public MastodonNetwork(string code, string instance, string token) : base(NetworkType.Mastodon, code, instance, PostFormatVariantFactory.Mastodon)
    {
        this.token = token;
    }

    protected override async Task DisposeClientAsync()
    {
    }

    protected override async Task InitClientAsync()
    {
        client = new MastodonClient(NetworkName, token);
    }

    protected override async Task<ConnectionTestResult> TestConnectionImplementationAsync()
    {
        try
        {
            var account = await client!.GetCurrentUser();
            if (account != null)
            {
                return new ConnectionTestResult(this, true, account.Id, $"Username: {account.UserName}, Display name: {account.DisplayName}");
            }
            else
            {
                return new ConnectionTestResult(this, false, null, "Could not read current user account");
            }
        }
        catch (Exception e)
        {
            return new ConnectionTestResult(this, false, null, e.Message, e);
        }
    }

    private string NetworkUrl => $"https://{NetworkName}";

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message, IEnumerable<string> texts, IEnumerable<IEnumerable<ISocialImage>> images)
    {
        var statuses = new List<Status>();
        for (int p = 0; p < texts.Count(); p++)
        {
            var text = texts.ElementAt(p);
            if (!DryRunPosting)
            {
                // TODO: image uploads - add images to the first post, figure out something for subsequent posts if necessary
                var mediaIds = new List<string>();
                foreach (var image in images.ElementAt(p))
                {
                    var media = await client!.UploadMedia(await image.GetStreamAsync(), description: image.Description);
                    if (media == null) throw new Exception($"Could not upload image for post {p}");
                    mediaIds.Add(media.Id);
                }
                var previousStatus = statuses.LastOrDefault();
                var status = await client!.PublishStatus(text, Visibility.Public, previousStatus?.Id, mediaIds, false, null, null, null, null);
                if (status == null) throw new Exception($"Could not post status {p}");
                statuses.Add(status);
            }
        }
        var aok = statuses.All(s => s != null && !string.IsNullOrWhiteSpace(s.Id));
        var ids = statuses.Where(s => s != null && !string.IsNullOrWhiteSpace(s.Id)).Select(s => s!.Id!);
        return new PostResult(this, message, aok, ids, aok ? null : "Unable to post all statuses");
    }

    protected override IEnumerable<IEnumerable<ISocialImage>> AssignImages(ISocialMessage message, int posts)
    {
        // TODO: make throwIfTooManyImages configurable
        return FrontLoadImages(message, posts, maxImagesPerPost: 4, throwIfTooManyImages: true);
    }

}