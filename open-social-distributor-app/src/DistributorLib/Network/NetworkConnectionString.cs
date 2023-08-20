namespace DistributorLib.Network;

public class NetworkConnectionString
{

    public NetworkConnectionString(string value) {
        this.Value = value;
        this.Parameters = GetParameters(value);
    }
    
    public string Value { get; private set; }
    public Dictionary<string,string> Parameters { get; private set; }

    private Dictionary<string,string> GetParameters(string connection) {
        var parameters = new Dictionary<string, string>();
        var parts = connection.Split(';');
        foreach (var part in parts) {
            var keyvalue = part.Split('=');
            if (keyvalue.Length != 2) throw new ArgumentException($"Invalid connection string: {this.Value}, part: \"{part}\" is not a key=value pair");
            parameters.Add(keyvalue[0], keyvalue[1]);
        }
        return parameters;
    }

    public static implicit operator string(NetworkConnectionString ncs) {
        return ncs.Value;
    }

    public static implicit operator NetworkConnectionString(string str) {
        return new NetworkConnectionString(str);
    }

}