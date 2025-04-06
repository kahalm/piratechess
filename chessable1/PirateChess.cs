using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using RestSharp;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace piratechess
{
    public partial class PirateChess : Form, IDisposable
    {
        private int _cumLines = 0;
        private StringBuilder _pgn = new();
        private string _coursename = "";
        public PirateChess()
        {
            InitializeComponent();

            string? value1, value2, value3;

            // Read values from INI
            var settings = INIFileHandler.ReadFromINI(Options.filePath, Options.section, Options.key1, Options.key2, Options.key3);
            if (!settings.TryGetValue(Options.key1, out value1))
            {
                value1 = "";
            }
            if (!settings.TryGetValue(Options.key2, out value2))
            {
                value2 = "";
            }
            if (!settings.TryGetValue(Options.key3, out value3))
            {
                value3 = "";
            }


            textBoxBearer.Text = value1;
            textBoxUid.Text = value2;
            textBoxBid.Text = value3;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Write values to INI
            INIFileHandler.WriteToINI(Options.filePath, Options.section, Options.key1, textBoxBearer.Text,
                Options.key2, textBoxUid.Text, Options.key3, textBoxBid.Text);

            base.OnFormClosed(e);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _pgn = new StringBuilder();
            new Thread(() =>
            {
                GetCourse(Options.GetOptions());


                Invoke(new Action(() =>
                {
                    textBoxPGN.Text = _pgn.ToString();
                }));
            }).Start();
        }

        private void ButtonTestdaten_Click(object sender, EventArgs e)
        {
            _pgn = new StringBuilder();
            var pgnHeader = new pgnInfo();
            GetLine(Options.GetOptions(), pgnHeader, Testdata.Testjson);
        }


        private void buttonFirstTenLines_Click_1(object sender, EventArgs e)
        {
            _pgn = new StringBuilder();
            new Thread(() =>
            {
                _cumLines = 0;
                GetCourse(Options.GetOptions(), 10);


                Invoke(new Action(() =>
                {
                    textBoxPGN.Text = _pgn.ToString();
                }));
            }).Start();
        }

        private void GetLine(JsonSerializerOptions caseInvariant, pgnInfo pgnHeader, string json = "")
        {
            string content = "";
            if (json == "")
            {
                RestClient client = new($"https://www.chessable.com/api/v1/getGame?lng=en&uid={textBoxUid.Text}&oid={textBoxOid.Text}");


                var bearer = textBoxBearer.Text;
                var request = generateRequest(bearer);

                RestResponse response = client.Execute(request);
                content = response.Content ?? "";
            }
            else
            {
                content = json;
            }

            if (content != null)
            {
                ResponseLine? game = JsonSerializer.Deserialize<ResponseLine>(content, options: caseInvariant);
                string? pgn = game?.Game?.GeneratePGN();

                pgnHeader.FEN = game?.Game.Initial ?? "";
                _cumLines++;

                _pgn?.Append($"""
                        
                        [Event "{pgnHeader.Event}"]
                        [Round "{pgnHeader.Event:000}.{pgnHeader.Subround:000}"]
                        [White "{pgnHeader.White}"]
                        [Black "{pgnHeader.Black}"]
                        [FEN "{pgnHeader.FEN}"]
                        [Result "*"]

                        {pgn}


                        """);

                Invoke(new Action(() =>
                {
                    textBoxCumulativeLines.Text = _cumLines.ToString();
                }));
            }

        }

        private RestRequest generateRequest(string bearer)
        {
            RestRequest request = new("", Method.Get);
            request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:138.0) Gecko/20100101 Firefox/138.0");
            request.AddHeader("accept", "application/json, text/plain, */*");
            request.AddHeader("accept-language", "en");
            request.AddHeader("accept-encoding", "gzip, deflate, br, zstd");
            request.AddHeader("platform", "Web");
            request.AddHeader("x-os-name", "Firefox");
            request.AddHeader("x-os-version", "138");
            request.AddHeader("x-device-model", "Windows");
            request.AddHeader("authorization", $"Bearer {bearer}");
            request.AddHeader("alt-used", "www.chessable.com");
            request.AddHeader("connection", "keep-alive");
            request.AddHeader("sec-fetch-dest", "empty");
            request.AddHeader("sec-fetch-mode", "cors");
            request.AddHeader("sec-fetch-site", "same-origin");
            request.AddHeader("priority", "u=0");
            request.AddHeader("te", "trailers");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("cache-control", "no-cache");

            return request;
        }
        private void GetCourse(JsonSerializerOptions caseInvariant, int lines = 10000)
        {
            if (textBoxBearer.Text.Length < 3000)
            {
                MessageBox.Show("bearer missing");
                return;
            }

            if (textBoxUid.Text.Length == 0)
            {
                MessageBox.Show("uid (userid) missing");
                return;
            }

            if (textBoxBid.Text.Length == 0)
            {
                MessageBox.Show("bid (Courseid) missing");
                return;
            }

            _cumLines = 0;

            var url = $"https://www.chessable.com/api/v1/getCourse?uid={textBoxUid.Text}&bid={textBoxBid.Text}";
            RestClient client = new(url);

            var bearer = textBoxBearer.Text;
            var request = generateRequest(bearer);

            var response = client.Execute(request);
            Invoke(new Action(() =>
            {
                textBoxPGN.Text = "";
                textBoxCumulativeLines.Text = _cumLines.ToString();
            }));
            string content = response.Content ?? "";
            if (content != null)
            {
                ResponseCourse? course = null;
                try
                {
                    course = JsonSerializer.Deserialize<ResponseCourse>(content, options: caseInvariant);
                }
                catch { }

                if (course == null || course.Course == null)
                {
                    return;
                }

                var durchlauf = 0;
                foreach (Chapter item in course.Course.Data)
                {
                    durchlauf++;
                    Invoke(new Action(() =>
                    {
                        textBoxDurchlauf.Text = $"{durchlauf} / {course.Course.Data.Count}";
                        textBoxLid.Text = item.Id.ToString();
                    }));

                    GetChapter(Options.GetOptions(), lines, durchlauf);
                    var rand = new Random();
                    System.Threading.Thread.Sleep(rand.Next(500, 1500));

                    if (lines <= _cumLines)
                    {
                        break;
                    }
                }
            }

            MessageBox.Show("Finished.", "Finished", MessageBoxButtons.OK, MessageBoxIcon.None,
     MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }


        private void GetChapter(JsonSerializerOptions caseInvariant, int lines, int chapter)
        {
            RestClient client = new($"https://www.chessable.com/api/v1/getList?uid={textBoxUid.Text}&bid={textBoxBid.Text}&lid={textBoxLid.Text}");

            var bearer = textBoxBearer.Text;
            var request = generateRequest(bearer);

            RestResponse response = client.Execute(request);


            string content = response.Content ?? "";
            if (content != null)
            {
                ResponseChapter responseChapter = JsonSerializer.Deserialize<ResponseChapter>(content, options: caseInvariant) ?? new ResponseChapter();

                var count = 0;

                foreach (Line line in responseChapter.List.Data)
                {
                    count++;
                    Invoke(new Action(() =>
                    {
                        textBoxCurLines.Text = $"{count} / {responseChapter.List.Data.Count}";
                        textBoxOid.Text = line.Id.ToString();

                    }));

                    var pgnHeader = new pgnInfo
                    {
                        Event = responseChapter.List.Name,
                        Round = chapter,
                        Subround = count,
                        White = line.Name,
                        Black = responseChapter.List.Title
                    };

                    _coursename = responseChapter.List.Name;

                    GetLine(Options.GetOptions(), pgnHeader);

                    if (lines < _cumLines)
                    {
                        break;
                    }
                }
            }
        }

        private void buttonSavePNG_Click(object sender, EventArgs e)
        {
            // Create a SaveFileDialog to allow the user to choose the save location
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                // Set the default file name

                string invalidChars = new string(Path.GetInvalidFileNameChars());
                string sanitizedFilename = string.Concat(_coursename.Split(invalidChars.ToCharArray()));

                saveFileDialog.FileName = $"{sanitizedFilename}.pgn";  // Adjust this default filename as needed
                saveFileDialog.Filter = "PGN files (*.pgn)|*.pgn|All files (*.*)|*.*"; // File type filter

                // Show the save file dialog and check if the user selected a file
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the file path chosen by the user
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        // Write the content of the TextBox to the selected file
                        File.WriteAllText(filePath, textBoxPGN.Text);
                        MessageBox.Show("File saved successfully!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while saving the file: " + ex.Message);
                    }
                }
            }
        }
    }

    public class ResponseCourse
    {
        public Course Course { get; set; } = new Course();
    }
    public class Course
    {
        public List<Chapter> Data { get; set; } = [];
    }
    public class Chapter
    {
        public int Id { get; set; }
    }
    public class ResponseLine
    {
        public Game Game { get; set; } = new Game();
    }
    public class ResponseChapter
    {
        public ResponseList List { get; set; } = new ResponseList();
    }
    public class ResponseList
    {
        public string Name { get; set; } = string.Empty;
        public List<Line> Data { get; set; } = [];
        public string Title { get; set; } = string.Empty;
    }

    public class ResponseMove
    {
        public string Before { get; set; } = string.Empty;
        public string After { get; set; } = string.Empty;
        public List<JsonMoveItemList> Data { get; set; } = [];
    }
    public class Line
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class Game
    {
        public bool Owned { get; set; }
        public List<JsonMove> Data { get; set; } = [];
        public string Initial { get; set; } = string.Empty;
        public string GeneratePGN()
        {
            string pgn = "";
            SortedList<int, JsonMove> sortedMoves = [];
            Data ??= [];
            foreach (JsonMove move in Data)
            {
                sortedMoves.Add(move.Id, move);

                if (move.After is not null and not "")
                {
                    ResponseMove? responseMove = JsonSerializer.Deserialize<ResponseMove>(move.After, options: Options.GetOptions());
                    if (responseMove == null || responseMove.Data == null)
                    {
                        continue;
                    }
                    move.Comment = string.Join(Environment.NewLine, responseMove.Data.Select(x => x.Comment).ToList());
                }
            }
            int lastMove = 0;
            foreach (JsonMove move in sortedMoves.Values)
            {
                if (lastMove < move.Move)
                {
                    pgn += $"{move.Move}. ";
                }
                pgn += move.San + " ";
                if (move.Comment != "")
                {
                    pgn += $"{{{move.Comment}}} ";
                }
                lastMove = move.Move;
            }
            return pgn;
        }
    }
    public class JsonMove
    {
        public int Id { get; set; }
        public int Move { get; set; }
        public string San { get; set; } = string.Empty;
        public string After { get; set; } = string.Empty;
        public string Comment { get; internal set; } = string.Empty;
    }

    public class JsonMoveItem
    {
        public string State { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Val { get; set; } = string.Empty;
    }

    public partial class JsonMoveItemList
    {
        public string State { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public JsonElement? Val { get; set; } //entweder eine Liste von itemList oder ein string.
        public string Comment
        {
            get
            {
                string comment = "";
                if (Val == null)
                {
                    return "";
                }
                if (Val.Value.ValueKind == JsonValueKind.String)
                {
                    comment = Val.ToString() ?? "";
                }
                else
                if (Val.Value.ValueKind == JsonValueKind.Array)
                {
                    List<string>? innerList = JsonSerializer.Deserialize<List<JsonMoveItemList>>(Val.Value, options: Options.GetOptions())?.Select(x => x.Comment).ToList();

                    comment = string.Join(Environment.NewLine, innerList ?? [""]);
                }
                else
                {
                    return "";
                }

                comment = comment.Replace("@@StartBracket@@", "(").Replace("@@EndBracket@@", ")");
                comment = comment.Replace("@@StartFEN@@", "").Replace("@@EndFEN@@", "");
                comment = comment.Replace("@@StartBlockQuote@@", "").Replace("@@EndBlockQuote@@", "");
                comment = comment.Replace("@@LinkStart@@", "").Replace("@@LinkEnd@@", "");
                comment = comment.Replace("@@SANStart@@", "").Replace("@@SANEnd@@", "");
                comment = comment.Replace("<br/>", "").Replace("<br>", "");
                comment = comment.Replace("</strong>", "").Replace("<strong>", "");
                comment = comment.Replace("</bold>", "").Replace("<bold>", "");
                comment = findHtmltags().Replace(comment, "");

                return comment;
            }
        }

        [GeneratedRegex("<[^>]*>")]
        private static partial Regex findHtmltags();

    }
}
