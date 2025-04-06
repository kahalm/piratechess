using System.Text.Json;
using System.Text.RegularExpressions;
using RestSharp;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace piratechess
{
    public partial class PirateChess : Form
    {
        private int _cumLines = 0;
        public PirateChess()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                GetCourse(Options.GetOptions());
            }).Start();
        }

        private void ButtonFirstTenLines_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                GetCourse(Options.GetOptions(), lines: 10);
            }).Start();
        }

        private void GetLine(JsonSerializerOptions caseInvariant, string json = "")
        {
            string content = "";
            if (json == "")
            {
                RestClient client = new($"https://www.chessable.com/api/v1/getGame?lng=en&uid={textBoxUid.Text}&oid={textBoxOid.Text}");
                RestRequest request = new("", Method.Get);
                _ = request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:138.0) Gecko/20100101 Firefox/138.0");
                _ = request.AddHeader("accept", "application/json, text/plain, */*");
                _ = request.AddHeader("accept-language", "en");
                _ = request.AddHeader("accept-encoding", "gzip, deflate, br, zstd");
                _ = request.AddHeader("referer", "https://www.chessable.com/variation/36729850/");
                _ = request.AddHeader("platform", "Web");
                _ = request.AddHeader("x-os-name", "Firefox");
                _ = request.AddHeader("x-os-version", "138");
                _ = request.AddHeader("x-device-model", "Windows");
                _ = request.AddHeader("x-device-id", "S5vQCG2VcFArYKfC9p97YI");
                _ = request.AddHeader("authorization", $"Bearer {textBoxBearer.Text}");
                _ = request.AddHeader("alt-used", "www.chessable.com");
                _ = request.AddHeader("connection", "keep-alive");
                _ = request.AddHeader("cookie", "amp_dfb317=S5vQCG2VcFArYKfC9p97YI.NzkwOTI3..1io2lfd40.1io2lg5uj.3.2.5; _gcl_au=1.1.91946173.1743846357; _ga_SM6G6M7B8T=GS1.1.1743846356.1.1.1743846445.49.0.0; tms_VisitorID=9v7h213gor; tms_wsip=1; _jst_profileid_fb5e9043b672481791ed69e733d9ac1e=9lKei4s9p; _fbp=fb.1.1743846357753.637610792166494840; osano_consentmanager_uuid=bedb2331-845d-4f17-890b-2ec38669c1a0; osano_consentmanager=DsNAdPUtyWA6SAPArZniSu4sUEywMC-Hq-OBJxhYsuc9i-LlZ7IyFWEwomBrPmfzgfw5pJRCaWFEceQg-o4e4m6jXtNCSnSDmi4aCrBhXNGXFEWf_zeyXvhKg5YLwoyb3IARux6pr15tafzSXLgGtlXXhfPWsfbdTo77BA3KSr8JfkU-YYAesASvQuCaf1cxznEo_vl-JEzgJrfI4-uqWlboZPMJDfSL9HgzFnCJhnOQSBe1edMsnNqxyDNlWO-owcvMVfEKVTDCgPINebbMD5GSyQ9In4ai01mu7MfgEuRktMKI4Hyqn2tQJQn6dPXzSraxvw==; osano_consentmanager_expdate=1777023958895; _ga=GA1.2.1818527451.1743846358; _ga=GA1.1.1818527451.1743846358; _ga=GA1.1.1818527451.1743846358; _gid=GA1.2.1395078794.1743846358; _gid=GA1.1.1395078794.1743846358; intercom-id-qzot1t7g=0559c744-8324-434e-a78d-f892cca5923a; intercom-session-qzot1t7g=; intercom-device-id-qzot1t7g=b5e29a6e-43b1-4fc4-989c-8b4677b25246; _ga_Z6ZD3CB4HN=GS1.2.1743846376.1.1.1743846444.60.0.0; sec_session_id=aa338c456c1a7de9ddf209741a604371; uidsessid=790927; unamesessid=kahalm; loginstringsessid=d3d8186535da6fde%3A2282e897b33ce4c28234178ae1aa706a; _gat=1");
                _ = request.AddHeader("sec-fetch-dest", "empty");
                _ = request.AddHeader("sec-fetch-mode", "cors");
                _ = request.AddHeader("sec-fetch-site", "same-origin");
                _ = request.AddHeader("priority", "u=0");
                _ = request.AddHeader("te", "trailers");
                _ = request.AddHeader("pragma", "no-cache");
                _ = request.AddHeader("cache-control", "no-cache");
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

                _cumLines++;
                Invoke(new Action(() =>
                {
                    textBoxPGN.Text += pgn;
                    textBoxCumulativeLines.Text = _cumLines.ToString();
                }));
            }

        }


        private void GetCourse(JsonSerializerOptions caseInvariant, int lines = 10000)
        {
            RestClient client = new($"https://www.chessable.com/api/v1/getCourse?uid={textBoxUid.Text}&bid={textBoxBid.Text}");
            RestRequest request = new("", Method.Get);
            _ = request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:138.0) Gecko/20100101 Firefox/138.0");
            _ = request.AddHeader("accept", "application/json, text/plain, */*");
            _ = request.AddHeader("accept-language", "en");
            _ = request.AddHeader("accept-encoding", "gzip, deflate, br, zstd");
            _ = request.AddHeader("referer", "https://www.chessable.com/variation/36729850/");
            _ = request.AddHeader("platform", "Web");
            _ = request.AddHeader("x-os-name", "Firefox");
            _ = request.AddHeader("x-os-version", "138");
            _ = request.AddHeader("x-device-model", "Windows");
            _ = request.AddHeader("x-device-id", "S5vQCG2VcFArYKfC9p97YI");
            _ = request.AddHeader("authorization", $"Bearer {textBoxBearer.Text}");
            _ = request.AddHeader("alt-used", "www.chessable.com");
            _ = request.AddHeader("connection", "keep-alive");
            _ = request.AddHeader("cookie", "amp_dfb317=S5vQCG2VcFArYKfC9p97YI.NzkwOTI3..1io2lfd40.1io2lg5uj.3.2.5; _gcl_au=1.1.91946173.1743846357; _ga_SM6G6M7B8T=GS1.1.1743846356.1.1.1743846445.49.0.0; tms_VisitorID=9v7h213gor; tms_wsip=1; _jst_profileid_fb5e9043b672481791ed69e733d9ac1e=9lKei4s9p; _fbp=fb.1.1743846357753.637610792166494840; osano_consentmanager_uuid=bedb2331-845d-4f17-890b-2ec38669c1a0; osano_consentmanager=DsNAdPUtyWA6SAPArZniSu4sUEywMC-Hq-OBJxhYsuc9i-LlZ7IyFWEwomBrPmfzgfw5pJRCaWFEceQg-o4e4m6jXtNCSnSDmi4aCrBhXNGXFEWf_zeyXvhKg5YLwoyb3IARux6pr15tafzSXLgGtlXXhfPWsfbdTo77BA3KSr8JfkU-YYAesASvQuCaf1cxznEo_vl-JEzgJrfI4-uqWlboZPMJDfSL9HgzFnCJhnOQSBe1edMsnNqxyDNlWO-owcvMVfEKVTDCgPINebbMD5GSyQ9In4ai01mu7MfgEuRktMKI4Hyqn2tQJQn6dPXzSraxvw==; osano_consentmanager_expdate=1777023958895; _ga=GA1.2.1818527451.1743846358; _ga=GA1.1.1818527451.1743846358; _ga=GA1.1.1818527451.1743846358; _gid=GA1.2.1395078794.1743846358; _gid=GA1.1.1395078794.1743846358; intercom-id-qzot1t7g=0559c744-8324-434e-a78d-f892cca5923a; intercom-session-qzot1t7g=; intercom-device-id-qzot1t7g=b5e29a6e-43b1-4fc4-989c-8b4677b25246; _ga_Z6ZD3CB4HN=GS1.2.1743846376.1.1.1743846444.60.0.0; sec_session_id=aa338c456c1a7de9ddf209741a604371; uidsessid=790927; unamesessid=kahalm; loginstringsessid=d3d8186535da6fde%3A2282e897b33ce4c28234178ae1aa706a; _gat=1");
            _ = request.AddHeader("sec-fetch-dest", "empty");
            _ = request.AddHeader("sec-fetch-mode", "cors");
            _ = request.AddHeader("sec-fetch-site", "same-origin");
            _ = request.AddHeader("priority", "u=0");
            _ = request.AddHeader("te", "trailers");
            _ = request.AddHeader("pragma", "no-cache");
            _ = request.AddHeader("cache-control", "no-cache");
            RestResponse response = client.Execute(request);


            Invoke(new Action(() =>
            {
                textBoxPGN.Text = "";
                textBoxCumulativeLines.Text = _cumLines.ToString();
            }));
            string content = response.Content ?? "";
            if (content != null)
            {
                ResponseCourse? course = JsonSerializer.Deserialize<ResponseCourse>(content, options: caseInvariant);

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

                    if (lines < _cumLines)
                    {
                        break;
                    }
                }
            }
        }


        private void GetChapter(JsonSerializerOptions caseInvariant, int lines, int chapter)
        {
            RestClient client = new($"https://www.chessable.com/api/v1/getList?uid={textBoxUid.Text}&bid={textBoxBid.Text}&lid={textBoxLid.Text}");
            RestRequest request = new("", Method.Get);
            _ = request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:138.0) Gecko/20100101 Firefox/138.0");
            _ = request.AddHeader("accept", "application/json, text/plain, */*");
            _ = request.AddHeader("accept-language", "en");
            _ = request.AddHeader("accept-encoding", "gzip, deflate, br, zstd");
            _ = request.AddHeader("referer", "https://www.chessable.com/variation/36729850/");
            _ = request.AddHeader("platform", "Web");
            _ = request.AddHeader("x-os-name", "Firefox");
            _ = request.AddHeader("x-os-version", "138");
            _ = request.AddHeader("x-device-model", "Windows");
            _ = request.AddHeader("x-device-id", "S5vQCG2VcFArYKfC9p97YI");
            _ = request.AddHeader("authorization", $"Bearer {textBoxBearer.Text}");
            _ = request.AddHeader("alt-used", "www.chessable.com");
            _ = request.AddHeader("connection", "keep-alive");
            _ = request.AddHeader("cookie", "amp_dfb317=S5vQCG2VcFArYKfC9p97YI.NzkwOTI3..1io2lfd40.1io2lg5uj.3.2.5; _gcl_au=1.1.91946173.1743846357; _ga_SM6G6M7B8T=GS1.1.1743846356.1.1.1743846445.49.0.0; tms_VisitorID=9v7h213gor; tms_wsip=1; _jst_profileid_fb5e9043b672481791ed69e733d9ac1e=9lKei4s9p; _fbp=fb.1.1743846357753.637610792166494840; osano_consentmanager_uuid=bedb2331-845d-4f17-890b-2ec38669c1a0; osano_consentmanager=DsNAdPUtyWA6SAPArZniSu4sUEywMC-Hq-OBJxhYsuc9i-LlZ7IyFWEwomBrPmfzgfw5pJRCaWFEceQg-o4e4m6jXtNCSnSDmi4aCrBhXNGXFEWf_zeyXvhKg5YLwoyb3IARux6pr15tafzSXLgGtlXXhfPWsfbdTo77BA3KSr8JfkU-YYAesASvQuCaf1cxznEo_vl-JEzgJrfI4-uqWlboZPMJDfSL9HgzFnCJhnOQSBe1edMsnNqxyDNlWO-owcvMVfEKVTDCgPINebbMD5GSyQ9In4ai01mu7MfgEuRktMKI4Hyqn2tQJQn6dPXzSraxvw==; osano_consentmanager_expdate=1777023958895; _ga=GA1.2.1818527451.1743846358; _ga=GA1.1.1818527451.1743846358; _ga=GA1.1.1818527451.1743846358; _gid=GA1.2.1395078794.1743846358; _gid=GA1.1.1395078794.1743846358; intercom-id-qzot1t7g=0559c744-8324-434e-a78d-f892cca5923a; intercom-session-qzot1t7g=; intercom-device-id-qzot1t7g=b5e29a6e-43b1-4fc4-989c-8b4677b25246; _ga_Z6ZD3CB4HN=GS1.2.1743846376.1.1.1743846444.60.0.0; sec_session_id=aa338c456c1a7de9ddf209741a604371; uidsessid=790927; unamesessid=kahalm; loginstringsessid=d3d8186535da6fde%3A2282e897b33ce4c28234178ae1aa706a; _gat=1");
            _ = request.AddHeader("sec-fetch-dest", "empty");
            _ = request.AddHeader("sec-fetch-mode", "cors");
            _ = request.AddHeader("sec-fetch-site", "same-origin");
            _ = request.AddHeader("priority", "u=0");
            _ = request.AddHeader("te", "trailers");
            _ = request.AddHeader("pragma", "no-cache");
            _ = request.AddHeader("cache-control", "no-cache");
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
                        textBoxPGN.Text += $"""
                        
                        [Event "{responseChapter.List.Name}"]
                        [Round "{chapter:000}.{count:000}"]
                        [White "{line.Name}"]
                        [Black "{responseChapter.List.Title}"]
                        [Result "*"]


                        """;
                        textBoxOid.Text = line.Id.ToString();

                    }));
                    GetLine(Options.GetOptions());
                    if (lines < _cumLines)
                    {
                        break;
                    }
                }
            }
        }

        private void ButtonTestdaten_Click(object sender, EventArgs e)
        {
            GetLine(Options.GetOptions(), Testdata.Testjson);
        }


        private void buttonFirstTenLines_Click_1(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                _cumLines = 0;
                GetCourse(Options.GetOptions(), 10);
            }).Start();
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
        public string GeneratePGN()
        {
            // 1. Parse äußeres JSON


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
