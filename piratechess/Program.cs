using piratechess_lib;
using System.Text.Json;

string rawDir = Path.GetFullPath("rawresponses");
string pgnDir = Path.GetFullPath("pgn");

IEnumerable<string> files = args.Length > 0
    ? args.Select(Path.GetFullPath)
    : Directory.GetFiles(rawDir, "*.restResponse");

if (!files.Any())
{
    Console.WriteLine($"No .restResponse files found in {rawDir}");
    return;
}

foreach (string file in files)
{
    Console.WriteLine($"Processing: {Path.GetFileName(file)}");

    string json = File.ReadAllText(file);
    var course = JsonSerializer.Deserialize<RestResponseCourse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    if (course == null)
    {
        Console.WriteLine("  ERROR: Could not deserialize file.");
        continue;
    }

    var lib = new PirateChessLib();
    lib.restResponseCourse = course;
    lib.SetChapterCounterEvent(c => Console.Write($"\r  Chapter {c}   "));
    lib.SetLineCounterEvent(l => Console.Write($"\r  Line {l}        "));
    lib.SetCumulativeLinesEvent(t => Console.Write($"\r  Total lines: {t}   "));

    var (pgn, coursename) = lib.GetCourse("", useLocalData: true);
    Console.WriteLine();

    if (string.IsNullOrEmpty(pgn))
    {
        Console.WriteLine("  WARNING: Empty PGN generated.");
        continue;
    }

    string safeName = string.Concat(coursename.Split(Path.GetInvalidFileNameChars()));
    if (string.IsNullOrWhiteSpace(safeName))
        safeName = Path.GetFileNameWithoutExtension(file);

    string outPath = Path.Combine(pgnDir, safeName + ".pgn");
    try
    {
        File.WriteAllText(outPath, pgn);
        Console.WriteLine($"  Saved: {outPath}");
    }
    catch (IOException ex)
    {
        Console.WriteLine($"  ERROR: Could not write file (is it open in another program?): {ex.Message}");
    }
}
