using System.Text.RegularExpressions;
using CommandLine;
using DistributorLib;
using DistributorLib.Network;
using DistributorLib.Network.Implementations;
using DistributorLib.Post;
using DistributorLib.Post.Images;
using Mastonet.Entities;
using Newtonsoft.Json;

namespace DistributionCLI;

public class DistributionCLI
{
    [Verb("post", HelpText = "Post a message to one or more social networks.")]
    public class PostOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to the config file.")]
        public string ConfigPath { get; set; } = string.Empty;

        [Option('f', "filter", Required = false, HelpText = "Regular expression filter for network short codes.")]
        public string? Filter { get; set; } = null;

        [Option('m', "message", Required = true, HelpText = "Simple message text.")]
        public string Message { get; set; } = string.Empty;

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

    public static void Main(params string[] args)
    {
        Parser.Default
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
        var i = 0;
        foreach (var result in testResults)
        {
            Console.WriteLine($"{++i} - {result.Key.ShortCode} ({result.Key.NetworkType}): {(result.Value.Success ? "OK" : "fail")}");
        }

        var fails = testResults.Where(r => !r.Value.Success);
        if (fails.Count() > 0)
        {
            Console.WriteLine($"{fails.Count()} issues found:");
            foreach (var result in fails)
            {
                var message = result.Value.Message;
                var exceptionType = result.Value.Exception?.GetType().Name;
                var report = "";
                if (!string.IsNullOrWhiteSpace(message)) {
                    report += message;
                }
                if (!string.IsNullOrWhiteSpace(exceptionType)) {
                    if (!string.IsNullOrWhiteSpace(report)) report += " ";
                    report += $"({exceptionType})";
                }
                Console.WriteLine($"❌ - {result.Key.ShortCode} ({result.Key.NetworkType}): {report}");
                if (result.Value.Network is FacebookNetwork)
                {
                    Console.WriteLine($"Facebook token: {(result.Value.Network as FacebookNetwork)!.pageToken}");
                }
            }
        }
        return testResults.All(r => r.Value.Success) ? 0 : 1;
    }

    public static int ExecutePost(PostOptions options)
    {
        var json = File.ReadAllText(options.ConfigPath!);
        var config = JsonConvert.DeserializeObject<Config>(json);
        var networks = NetworkFactory.FromConfig(config!);

        var filteredNetworks = options.Filter == null ? networks : FilterNetworks(networks, options.Filter);
        Console.WriteLine($"{filteredNetworks.Count()} of {networks.Count()} networks selected for posting.");
        
        var distributor = new Distributor(filteredNetworks);

        var text = options.Message;
        var link = options.Link;
        var tags = options.Tags;
        var images = options.Images != null ? SocialImageFactory.FromUrisAndDescriptions(options.Images, options.ImageDescriptions) : null;
        var message = new SimpleSocialMessage(text, images, link, tags);
        var results = distributor.PostAsync(message).Result;
        foreach (var result in results)
        {
            Console.WriteLine($"{(result.Success ? "✅" : "❌")} {result.Network.ShortCode} ({result.Network.NetworkType})");
        }
        Console.WriteLine();
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

        return 0;
    }
}
