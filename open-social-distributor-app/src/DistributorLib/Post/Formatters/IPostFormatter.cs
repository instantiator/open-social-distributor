using DistributorLib.Network;

namespace DistributorLib.Post.Formatters;

public interface IPostFormatter
{
    string? GetLink(ISocialMessage message);
    IEnumerable<string> FormatText(ISocialMessage message);
}