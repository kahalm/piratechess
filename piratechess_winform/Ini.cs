using System.Text;

namespace piratechess_Winform
{

    class INIFileHandler
    {
        // Method to write string values to an INI file
        public static void WriteToINI(string filePath, string section, IDictionary<string, string> values)
        {
            StringBuilder iniContent = new();

            // Add the section header
            iniContent.AppendLine($"[{section}]");

            // Add key-value pairs
            foreach (var entry in values)
            {
                if (string.IsNullOrEmpty(entry.Key))
                    continue;
                iniContent.AppendLine($"{entry.Key}={entry.Value}");
            }

            // Write to the file
            File.WriteAllText(filePath, iniContent.ToString());
        }

        // Method to read all key-value pairs of a section from an INI file
        public static Dictionary<string, string> ReadFromINI(string filePath, string section)
        {
            Dictionary<string, string> values = [];

            if (!File.Exists(filePath))
            {
                Console.WriteLine("INI file does not exist.");
                return values;
            }

            string currentSection = "";

            foreach (var line in File.ReadAllLines(filePath))
            {
                // Ignore comments and empty lines
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';'))
                    continue;

                // Check for section headers
                if (line.StartsWith('[') && line.EndsWith(']'))
                {
                    currentSection = line.Trim('[', ']');
                    continue;
                }

                // If we're in the right section, collect key-value pairs
                if (currentSection != section)
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length < 2) continue;
                values[parts[0]] = parts[1];
            }

            return values;
        }
    }
}
