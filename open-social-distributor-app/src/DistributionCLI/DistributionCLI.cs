using System.Text.RegularExpressions;
using CommandLine;
using DistributorLib;
using DistributorLib.Input;
using DistributorLib.Network;
using DistributorLib.Network.Implementations;
using DistributorLib.Post;
using DistributorLib.Post.Images;
using Mastonet.Entities;
using Newtonsoft.Json;

namespace DistributionCLI;

public class DistributionCLI
{
    public static IEnumerable<PostResult>? LastPostResults;
    public static IDictionary<ISocialNetwork, ConnectionTestResult>? LastTestResults;

    [Verb("post", HelpText = "Post a message to one or more social networks.")]
    public class PostOptions
    {
        [Option('c', "config-file", Required = true, HelpText = "Path to the config file.")]
        public string ConfigPath { get; set; } = null!;

        [Option('s', "source-file", Required = false, HelpText = "Path or url to a source file containing posts.")]
        public string? DataPath { get; set; }

        [Option('o', "offset", Required = false, HelpText = "Offset (index) of a post within the source file. (Leave blank to send all posts in the source file.)")]
        public int? Offset { get; set; }

        [Option('f', "filter", Required = false, HelpText = "Regular expression filter for network short codes.")]
        public string? Filter { get; set; } = null;

        [Option('m', "message", Required = false, HelpText = "Simple message text.")]
        public string? Message { get; set; } = string.Empty;

        [Option('l', "link", Required = false, HelpText = "Link for this message.")]
        public string? Link { get; set; }

        [Option('i', "images", Required = false, Separator = ',', HelpText = "URIs to images, semi-colon separated (;)")]
        public IEnumerable<string>? Images { get; set; }

        [Option('d', "image-descriptions", Required = false, Separator = ';', HelpText = "Image descriptions, semi-colon separated (;)")]
        public IEnumerable<string>? ImageDescriptions { get; set; }

        [Option('t', "tags", Required = false, Separator = ';', HelpText = "A list of tags (without # prefix), semi-colon separated (;)")]
        public IEnumerable<string>? Tags { get; set; }
    }

    [Verb("test", HelpText = "Test all connections in the configuration file.")]
    public class TestOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to the config file.")]
        public string ConfigPath { get; set; } = string.Empty;

        [Option('f', "filter", Required = false, HelpText = "Regular expression filter for network short codes.")]
        public string? Filter { get; set; } = null;
    }

    public static int Main(params string[] args)
    {
        return Parser.Default
            .ParseArguments<PostOptions, TestOptions>(args)
            .MapResult(
                (PostOptions opts) => ExecutePost(opts),
                (TestOptions opts) => ExecuteTest(opts),
                errs => 1);
    }

    public static IEnumerable<ISocialNetwork> FilterNetworks(IEnumerable<ISocialNetwork> networks, string regex)
    {
        var filter = new Regex(regex);
        return networks.Where(n => filter.IsMatch(n.ShortCode));
    }

    public static int ExecuteTest(TestOptions options)
    {
        var json = File.ReadAllText(options.ConfigPath!);
        var config = JsonConvert.DeserializeObject<Config>(json);
        var networks = NetworkFactory.FromConfig(config!);
        Console.WriteLine($"Identified {networks.Count()} networks.");

        var filteredNetworks = options.Filter == null ? networks : FilterNetworks(networks, options.Filter);
        Console.WriteLine($"{filteredNetworks.Count()} of {networks.Count()} networks selected for testing.");

        var distributor = new Distributor(filteredNetworks);
        var testResults = distributor.TestNetworksAsync().Result;
        LastTestResults = testResults;
        foreach (var result in testResults)
        {
            var message = result.Value.Message ?? result.Value.Exception?.Message;
            var exceptionType = result.Value.Exception != null ? result.Value.Exception?.GetType().Name + ": " : "";
            var report = $"{exceptionType}{message}";
            var id = result.Value.ActorId != null ? $"id = {result.Value.ActorId}, " : "";
            var icon = result.Value.Success ? "✅" : "❌";
            Console.WriteLine($"{icon} - {result.Key.ShortCode} ({result.Key.NetworkType}) - {id}{report}");
        }
        return testResults.All(r => r.Value.Success) ? 0 : 1;
    }

    public static int ExecutePost(PostOptions options)
    {
        var configJson = File.ReadAllText(options.ConfigPath!);
        var config = JsonConvert.DeserializeObject<Config>(configJson);
        var networks = NetworkFactory.FromConfig(config!);

        var filteredNetworks = options.Filter == null ? networks : FilterNetworks(networks, options.Filter);
        Console.WriteLine($"{filteredNetworks.Count()} of {networks.Count()} networks selected for posting.");
        
        var distributor = new Distributor(filteredNetworks);

        var text = options.Message;
        var dataPath = options.DataPath;

        if (!string.IsNullOrWhiteSpace(dataPath) && !string.IsNullOrWhiteSpace(text))
        {
            Console.WriteLine("Both data file and message were specified - these are mutually exclusive.");
            return 1;
        }

        if (!string.IsNullOrWhiteSpace(text))
        {
            var link = options.Link;
            var tags = options.Tags;
            var images = options.Images != null ? SocialImageFactory.FromUrisAndDescriptions(options.Images, options.ImageDescriptions) : null;
            var message = new SimpleSocialMessage(text!, images, link, tags);
            var results = distributor.PostAsync(message).Result;
            LastPostResults = results;
            PrintResults(results);
            Console.WriteLine();
            PrintErrors(results);
        }

        if (!string.IsNullOrWhiteSpace(dataPath))
        {
            var postListJson = UriReader.ReadStringAsync(dataPath).Result;
            var postList = new PostListReader().ReadJson(postListJson);
            var messages = options.Offset == null 
                ? postList!.ToSocialMessages()
                : new[] { postList!.ToSocialMessages().ElementAt(options.Offset.Value) }; 

            var resultSets = new List<IEnumerable<PostResult>>();
            foreach (var message in messages)
            {
                var results = distributor.PostAsync(message).Result;
                LastPostResults = results;
                resultSets.Add(results);
                PrintResults(results);
                Console.WriteLine();
                PrintErrors(results);
                Console.WriteLine();
            }
        }
        return 0;
    }

    public static void PrintResults(IEnumerable<PostResult> results)
    {
        foreach (var result in results)
        {
            Console.WriteLine($"{(result.Success ? "✅" : "❌")} {result.Network.ShortCode} ({result.Network.NetworkType})");
        }
    }

    public static void PrintErrors(IEnumerable<PostResult> results)
    {
        var errors = results.Where(r => !r.Success);
        if (errors.Count() > 0)
        {
            Console.WriteLine($"{errors.Count()} errors:");
            var i = 1;
            foreach (var error in errors)
            {
                Console.WriteLine($"{i++}. {error.Network.ShortCode} ({error.Network.NetworkType}): {error.Error} {(error.Exception != null ? error.Exception.ToString() : string.Empty)}");
            }
        }
    }

    public static void Reset()
    {
        LastPostResults = null;
        LastTestResults = null;
    }
}
