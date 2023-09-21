using DistributorLib.Post.Images;

namespace DistributorLib.Post.Assigners;

public abstract class AbstractImageAssigner : IImageAssigner
{
    protected int? MaxImagesPerPost { get; }
    protected bool ThrowIfTooManyImages { get; }

    protected AbstractImageAssigner(int? maxImagesPerPost, bool throwIfTooManyImages)
    {
        MaxImagesPerPost = maxImagesPerPost;
        ThrowIfTooManyImages = throwIfTooManyImages;
    }

    public abstract IEnumerable<IEnumerable<ISocialImage>> AssignImages(ISocialMessage message, int posts);

}
