using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace piratechess
{
    internal class Options
    {
        public static JsonSerializerOptions GetOptions() => new()
        {
            PropertyNameCaseInsensitive = true
        };

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
