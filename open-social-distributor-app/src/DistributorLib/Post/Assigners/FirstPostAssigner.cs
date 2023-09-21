using DistributorLib.Post.Images;

namespace DistributorLib.Post.Assigners;

public class FirstPostAssigner : AbstractImageAssigner
{
    public FirstPostAssigner(int? maxImagesPerPost, bool throwIfTooManyImages) : base(maxImagesPerPost, throwIfTooManyImages)
    {
    }

    public override IEnumerable<IEnumerable<ISocialImage>> AssignImages(ISocialMessage message, int posts)
    {
        if (ThrowIfTooManyImages && message.Images?.Count() > MaxImagesPerPost)
        {
            throw new Exception($"Unable to assign {message.Images?.Count()} images across {posts} posts.");
        }
        var images = MaxImagesPerPost != null ? message.Images?.Take(MaxImagesPerPost.Value) : message.Images;
        return new List<IEnumerable<ISocialImage>>()
        {
            images ?? new List<ISocialImage>() // all images in the first post
        };
    }

}
