using DistributorLib.Network;

namespace DistributorLib.Post.Formatters;

public interface IPostFormatter
{
    IEnumerable<string> FormatText(ISocialMessage message);
}