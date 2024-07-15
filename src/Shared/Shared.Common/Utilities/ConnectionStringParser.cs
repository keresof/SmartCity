using System.Text.RegularExpressions;
using System.Web;

namespace Shared.Common.Utilities;
public class ConnectionStringParser
{
    public static string ConvertToNpgsqlFormat(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
        }

        if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) || 
            connectionString.StartsWith("psql://", StringComparison.OrdinalIgnoreCase))
        {
            return ConvertUriFormatToNpgsql(connectionString);
        }
        else if (connectionString.Contains("="))
        {
            return NormalizeKeyValueFormat(connectionString);
        }
        else
        {
            throw new ArgumentException("Invalid connection string format.", nameof(connectionString));
        }
    }

    private static string ConvertUriFormatToNpgsql(string uriString)
    {
        var regex = new Regex(@"(?:postgresql|psql)://(?<username>[^:]+)(:(?<password>[^@]+))?@(?<host>[^:/]+)(:(?<port>\d+))?/(?<database>[^?]+)(\?(?<query>.+))?");
        var match = regex.Match(uriString);

        if (!match.Success)
        {
            throw new ArgumentException("Invalid URI connection string format.", nameof(uriString));
        }

        var parts = new Dictionary<string, string>
        {
            ["Host"] = match.Groups["host"].Value,
            ["Database"] = match.Groups["database"].Value,
            ["Username"] = match.Groups["username"].Value
        };

        if (match.Groups["port"].Success)
        {
            parts["Port"] = match.Groups["port"].Value;
        }

        if (match.Groups["password"].Success)
        {
            parts["Password"] = match.Groups["password"].Value;
        }

        if (match.Groups["query"].Success)
        {
            var queryParts = match.Groups["query"].Value.Split('&');
            foreach (var queryPart in queryParts)
            {
                var keyValue = queryPart.Split('=');
                var key = NormalizeKey(keyValue[0]);
                var value = keyValue.Length > 1 ? keyValue[1] : "true";  // Assign "true" if no value is provided
                parts[key] = value;
            }
        }

        return string.Join(";", parts.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }

    private static string NormalizeKeyValueFormat(string keyValueString)
    {
        var parts = keyValueString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Trim().Split(new[] { '=' }, 2))
            .Where(kvp => kvp.Length == 2)
            .Select(kvp => new KeyValuePair<string, string>(NormalizeKey(kvp[0].Trim()), kvp[1].Trim()))
            .Distinct(new KeyValuePairComparer())
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return string.Join(";", parts.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }

    private static string NormalizeKey(string key)
    {
        var normalizedKey = key.ToLowerInvariant();
        switch (normalizedKey)
        {
            case "server":
            case "data source":
                return "Host";
            case "user id":
            case "uid":
                return "Username";
            case "pwd":
                return "Password";
            case "ssl mode":
            case "sslmode":
                return "SSL Mode";
            default:
                return char.ToUpperInvariant(key[0]) + key.Substring(1);
        }
    }

    private class KeyValuePairComparer : IEqualityComparer<KeyValuePair<string, string>>
    {
        public bool Equals(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
        {
            return string.Equals(x.Key, y.Key, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(KeyValuePair<string, string> obj)
        {
            return obj.Key.ToLowerInvariant().GetHashCode();
        }
    }
}