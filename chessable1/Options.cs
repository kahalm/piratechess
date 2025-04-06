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
}
