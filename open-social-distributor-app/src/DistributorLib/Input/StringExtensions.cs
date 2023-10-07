namespace DistributorLib.Input;

public static class StringExtensions
{
    public static Uri? AsUri(this string candidate)
    {
        Uri? uri = null;
        var options = new UriCreationOptions() { DangerousDisablePathAndQueryCanonicalization = false };
        var ok = Uri.TryCreate(candidate, options, out uri);
        return uri;
    } 

}