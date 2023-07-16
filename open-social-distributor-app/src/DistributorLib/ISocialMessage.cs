namespace DistributorLib;
public interface ISocialMessage
{
    string Message { get; }
    IEnumerable<ISocialImage>? Images { get; }
}
