using System.Text.Json;

namespace piratechess
{
    internal class Options
    {
        public static JsonSerializerOptions GetOptions() => new()
        {
            PropertyNameCaseInsensitive = true
        };



        public static string filePath = "settings.ini";
        public static string section = "Settings";
        public static string key1 = "bearer";
        public static string key2 = "uid";
        public static string key3 = "bid";

    }

    internal class pgnInfo
    {
        public string Event = string.Empty;
        public int Round;
        public int Subround;
        public string White = string.Empty;
        public string Black = string.Empty;
        public string FEN = string.Empty;
    }
}
