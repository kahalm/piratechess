﻿using System.Text.Json;

namespace piratechess_lib
{
    internal class Options
    {

        public static JsonSerializerOptions GetOptions() => new()
        {
            PropertyNameCaseInsensitive = true
        };
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
