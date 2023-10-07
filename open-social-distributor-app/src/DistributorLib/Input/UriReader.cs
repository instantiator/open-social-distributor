namespace DistributorLib.Input;

public class UriReader
{
    public static async Task<Stream> GetStreamAsync(string uriOrPath)
    {
        var uri = uriOrPath.AsUri();
        return uri != null 
            ? await GetStreamAsync(uri) 
            : File.OpenRead(uriOrPath);
    }

    public static async Task<byte[]> ReadByteArrayAsync(string uriOrPath)
    {
        var uri = uriOrPath.AsUri();
        return uri != null 
            ? await ReadByteArrayAsync(uri) 
            : await File.ReadAllBytesAsync(uriOrPath);
    }

    public static async Task<string> ReadStringAsync(string uriOrPath)
    {
        var uri = uriOrPath.AsUri();
        return uri != null 
            ? await ReadStringAsync(uri) 
            : await File.ReadAllTextAsync(uriOrPath);
    }

    public static async Task<Stream> GetStreamAsync(Uri uri)
        => uri.IsFile 
            ? File.OpenRead(AbsoluteLocalPath(uri)!) 
            : await new HttpClient().GetStreamAsync(uri);

    public static async Task<byte[]> ReadByteArrayAsync(Uri uri)
        => uri.IsFile 
            ? await File.ReadAllBytesAsync(AbsoluteLocalPath(uri)!) 
            : await new HttpClient().GetByteArrayAsync(uri);

    public static async Task<string> ReadStringAsync(Uri uri)
        => uri.IsFile 
            ? await File.ReadAllTextAsync(AbsoluteLocalPath(uri)!) 
            : await new HttpClient().GetStringAsync(uri);

    public static string? LocalPath(string pathOrUri)
    {
        var uri = pathOrUri.AsUri();
        return uri != null 
            ? AbsoluteLocalPath(uri) 
            : pathOrUri;
    }

    public static string? AbsoluteLocalPath(Uri uri) 
        => uri.IsFile 
            ? Path.GetFullPath(uri.ToString().Substring("file://".Length)) 
            : null;

}