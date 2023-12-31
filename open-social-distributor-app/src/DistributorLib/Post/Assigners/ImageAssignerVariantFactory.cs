namespace DistributorLib.Post.Assigners;

public class ImageAssignerVariantFactory
{
    public static IImageAssigner Console => new FirstPostAssigner(null, false);

    public static IImageAssigner Discord => new FirstPostAssigner(10, true);

    public static IImageAssigner Facebook => new FirstPostAssigner(null, false);

    public static IImageAssigner LinkedIn => new FirstPostAssigner(20, true);

    public static IImageAssigner Mastodon => new FrontLoadAssigner(4, true);

    [Obsolete("Twitter is not a safe platform")]
    public static IImageAssigner Twitter => new FrontLoadAssigner(4, true);

}