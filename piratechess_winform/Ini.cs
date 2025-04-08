using System.Text;

namespace piratechess_Winform
{

    class INIFileHandler
    {
        // Method to write string values to an INI file
        public static void WriteToINI(string filePath, string section, string key1, string value1, string key2, string value2, string key3, string value3, string key4, string value4, string key5, string value5)
        {
            StringBuilder iniContent = new();

            // Add the section header
            iniContent.AppendLine($"[{section}]");

            // Add key-value pairs
            iniContent.AppendLine($"{key1}={value1}");
            iniContent.AppendLine($"{key2}={value2}");
            iniContent.AppendLine($"{key3}={value3}");
            iniContent.AppendLine($"{key4}={value4}");
            iniContent.AppendLine($"{key5}={value5}");

            // Write to the file
            File.WriteAllText(filePath, iniContent.ToString());
        }

        // Method to read string values from an INI file
        public static Dictionary<string, string> ReadFromINI(string filePath, string section, string key1, string key2, string key3, string key4, string key5)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("INI file does not exist.");
                return [];
            }

            string[] lines = File.ReadAllLines(filePath);
            string currentSection = "";
            string value1 = "", value2 = "", value3 = "", value4 = "", value5 = "";

            foreach (var line in lines)
            {
                // Ignore comments and empty lines
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';'))
                    continue;

                // Check for section headers
                if (line.StartsWith('[') && line.EndsWith(']'))
                {
                    currentSection = line.Trim('[', ']');
                }

                // If we're in the right section, check for key-value pairs
                if (currentSection == section)
                {
                    if (line.StartsWith(key1))
                        value1 = line.Split('=')[1];
                    else if (line.StartsWith(key2))
                        value2 = line.Split('=')[1];
                    else if (line.StartsWith(key3))
                        value3 = line.Split('=')[1];
                    else if (line.StartsWith(key4))
                        value4 = line.Split('=')[1];
                    else if (line.StartsWith(key5))
                        value5 = line.Split('=')[1];
                }
            }

            // Display read values
            Dictionary<string, string> dict = new()
            {
                { key1, value1 },
                { key2, value2 },
                { key3, value3 },
                { key4, value4 },
                { key5, value5 }
            };

            return dict;
        }
    }
}