using DistributorLib.Post.Images;

namespace DistributorLib.Post.Assigners;

public class FrontLoadAssigner : AbstractImageAssigner
{
    public FrontLoadAssigner(int? maxImagesPerPost, bool throwIfTooManyImages) : base(maxImagesPerPost, throwIfTooManyImages)
    {
    }

    public override IEnumerable<IEnumerable<ISocialImage>> AssignImages(ISocialMessage message, int posts)
    {
        var images = message.Images ?? new List<ISocialImage>();
        var result = new List<List<ISocialImage>>();

        if (MaxImagesPerPost < 1) return result;
        var currentGroup = new List<ISocialImage>();
        foreach (var image in images)
        {
            if (currentGroup.Count >= MaxImagesPerPost)
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
        if (ThrowIfTooManyImages && result.Count() > posts)
        {
            throw new Exception($"Unable to assign {images.Count()} images across {posts} posts.");
        }
        return result.Take(posts); // cap at the number of available posts
    }
}
