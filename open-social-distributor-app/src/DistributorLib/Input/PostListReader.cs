using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DistributorLib.Input;

public class PostListReader
{
    public PostListReader()
    {
    }

    private JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = {
            new JsonStringEnumConverter() // already case insensitive
        }
    };

    public PostListFormat? ReadJson(string json)
    {
        return JsonSerializer.Deserialize<PostListFormat>(json, options);
    }

    public PostListFormat? ReadFile(string filename)
    {
        return ReadJson(File.ReadAllText(filename));
    }

    public async Task<PostListFormat?> ReadUriAsync(Uri uri)
    {
        using (var client = new HttpClient())
        {
            return ReadJson(await client.GetStringAsync(uri));
        }
    }
}