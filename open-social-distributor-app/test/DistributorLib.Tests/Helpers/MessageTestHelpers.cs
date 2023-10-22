namespace DistributorLib.Tests.Helpers;

public static class MessageTestHelpers
{
    public static string RemoveHashTags(this string message)
        => string.Join(' ', message.Split(' ').Where(w => !w.StartsWith('#')));
}