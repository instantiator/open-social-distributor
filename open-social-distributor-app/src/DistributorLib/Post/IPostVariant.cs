namespace DistributorLib.Post;

public interface IPostVariant
{
    public int MessageLengthLimit { get; }

    public string Compose(ISocialMessage message);

}