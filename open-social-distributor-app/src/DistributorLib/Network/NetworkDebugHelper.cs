using RestSharp;

namespace DistributorLib.Network;

public class NetworkDebugHelper
{
    public static string Summarise(RestResponse response)
    {
        return "## Response\n\n"
            + "| Key | Value |\n|-|-|\n"
            + $"| Code | `{response.StatusCode}` ({(int)response.StatusCode}) |\n"
            + (string.IsNullOrWhiteSpace(response.ErrorMessage) ? "" : $"| Error | {response.ErrorMessage} |\n")
            + (string.IsNullOrWhiteSpace(response.Content) ? "" : $"| Content | `{response.Content}` |\n")
            + (response.Headers != null && response.Headers.Count() > 0 
                ? string.Join('\n', response.Headers!.Select(h => $"| {h.Name} | `{h.Value}` |")) : "")
            + "\n";
    }

    public static string Summarise(RestRequest request, string? body)
    {
        return "## Request\n\n"
            + "| Key | Value |\n|-|-|\n"
            + $"| Method | `{request.Method}` |\n"
            + $"| Resource | `{request.Resource}` |\n"
            + (string.IsNullOrWhiteSpace(body) ? "" : $"| Body | `{body}` |\n")
            + (request.Parameters != null && request.Parameters.Count() > 0 
                ? string.Join('\n', request.Parameters!.Select(p => $"| {p.Name} | {p.Value} |")) : "")
            + "\n";
    }

    public static string Summarise(HttpResponseMessage response)
    {
        return "## Response\n\n"
            + "| Key | Value |\n|-|-|\n"
            + $"| Code | `{response.StatusCode}` ({(int)response.StatusCode}) |\n"
            + (string.IsNullOrWhiteSpace(response.ReasonPhrase) ? "" : $"| Reason | {response.ReasonPhrase} |\n")
            + (response.Headers != null && response.Headers.Count() > 0 
                ? string.Join('\n', response.Headers!.Select(h => $"| {h.Key} | `{h.Value}` |")) : "")
            + "\n";
    }

}