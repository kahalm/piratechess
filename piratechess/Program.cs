using piratechess_lib;
using System.Text.Json;

string exportBase = "";
bool addMoveToEmptyChapters = false;
string iniPath = Path.Combine(AppContext.BaseDirectory, "settings.ini");
if (File.Exists(iniPath))
{
    foreach (var line in File.ReadAllLines(iniPath))
    {
        var parts = line.Split('=', 2);
        if (parts.Length != 2) continue;
        string key = parts[0].Trim();
        string val = parts[1].Trim();
        if (key == "exportFolder") exportBase = val;
        else if (key == "addMoveToEmptyChapters") addMoveToEmptyChapters = val == "1";
    }
}
if (string.IsNullOrEmpty(exportBase) || !Directory.Exists(exportBase))
    exportBase = Directory.GetCurrentDirectory();

string rawDir = Path.Combine(exportBase, "rawresponses");
string pgnDir = Path.Combine(exportBase, "pgn");
Directory.CreateDirectory(rawDir);
Directory.CreateDirectory(pgnDir);

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

    string safeName = "";

    foreach (var (label, allKeys, noTraining) in new (string, bool, bool)[] {
        ("firstkey",   false, false),
        ("allkeys",    true,  false),
        ("notraining", false, true)
    })
    {
        Console.Write($"  [{label}] ");

        var lib = new PirateChessLib();
        lib.restResponseCourse = course;
        lib.AllKeyMovesTraining = allKeys;
        lib.NoTrainingMove = noTraining;
        lib.AddMoveToEmptyChapters = addMoveToEmptyChapters;
        lib.SetChapterCounterEvent(c => Console.Write($"\r  [{label}] Chapter {c}   "));
        lib.SetLineCounterEvent(l => Console.Write($"\r  [{label}] Line {l}        "));
        lib.SetCumulativeLinesEvent(t => Console.Write($"\r  [{label}] Total lines: {t}   "));

        var (pgn, coursename) = lib.GetCourse("", useLocalData: true);
        Console.WriteLine();

        if (string.IsNullOrEmpty(pgn))
        {
            Console.WriteLine($"  [{label}] WARNING: Empty PGN generated.");
            continue;
        }

        if (string.IsNullOrEmpty(safeName))
        {
            safeName = string.Concat(coursename.Split(Path.GetInvalidFileNameChars()));
            if (string.IsNullOrWhiteSpace(safeName))
                safeName = Path.GetFileNameWithoutExtension(file);
        }

        string outPath = Path.Combine(pgnDir, $"{safeName}_{label}.pgn");
        try
        {
            File.WriteAllText(outPath, pgn);
            Console.WriteLine($"  [{label}] Saved: {outPath}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"  [{label}] ERROR: Could not write file (is it open in another program?): {ex.Message}");
        }
    }
}
