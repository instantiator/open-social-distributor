using DistributorLib.Post.Images;

namespace DistributorLib.Post.Assigners;

public interface IImageAssigner
{
    IEnumerable<IEnumerable<ISocialImage>> AssignImages(ISocialMessage message, int posts);

}