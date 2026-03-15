using System.Text.Json;

namespace piratechess_lib
{
    internal class Options
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static JsonSerializerOptions GetOptions() => _options;
    }

    internal class PgnInfo
    {
        public string Event = string.Empty;
        public int Round;
        public int Subround;
        public string White = string.Empty;
        public string Black = string.Empty;
        public string FEN = string.Empty;
    }
}
