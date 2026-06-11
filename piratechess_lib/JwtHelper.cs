using System;
using System.Text;
using System.Text.Json;

namespace piratechess_lib;
public static class JwtHelper
{
    public static int ExtractUidFromToken(string jwtToken)
    {
        if (string.IsNullOrWhiteSpace(jwtToken))
            throw new ArgumentException("Token darf nicht leer sein.", nameof(jwtToken));

        var parts = jwtToken.Split('.');
        if (parts.Length < 2)
            throw new ArgumentException("Ungültiges JWT-Format.");

        string payload = parts[1];
        string json = DecodeBase64Url(payload);

        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("user", out JsonElement userElement) &&
            userElement.TryGetProperty("uid", out JsonElement uidElement))
        {
            return uidElement.GetInt32();
        }

        throw new InvalidOperationException("UID konnte im Token nicht gefunden werden.");
    }

    public static DateTimeOffset? GetExpiration(string jwtToken)
    {
        if (string.IsNullOrWhiteSpace(jwtToken)) return null;
        var parts = jwtToken.Split('.');
        if (parts.Length < 2) return null;

        try
        {
            string json = DecodeBase64Url(parts[1]);
            using JsonDocument doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("exp", out JsonElement expEl) && expEl.TryGetInt64(out long exp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(exp);
            }
        }
        catch { }
        return null;
    }

    public static bool IsExpired(string jwtToken)
    {
        var exp = GetExpiration(jwtToken);
        return exp.HasValue && exp.Value <= DateTimeOffset.UtcNow;
    }

    private static string DecodeBase64Url(string base64Url)
    {
        string padded = base64Url.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
            case 1: padded += "="; break;
        }

        byte[] data = Convert.FromBase64String(padded);
        return Encoding.UTF8.GetString(data);
    }
}
