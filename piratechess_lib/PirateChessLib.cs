using System.Text;
using System.Text.Json;
using RestSharp;

namespace piratechess_lib
{
    public class PirateChessLib(string uid, string bearer)
    {
        private int _cumLines = 0;
        private Action<string>? _chapterCounterEvent;
        private Action<string>? _lineCounterEvent;
        private readonly StringBuilder _pgn = new();
        private readonly string _bearer = bearer;
        private readonly string _uid = uid;

        public (string, string) GetCourse(string bid, int lines = 10000)
        {
            _cumLines = 0;
            string coursename = string.Empty;
            JsonSerializerOptions caseInvariant = Options.GetOptions();

            string url = $"https://www.chessable.com/api/v1/getCourse?uid={_uid}&bid={bid}";
            RestClient client = new(url);

            RestRequest request = GenerateRequest(_bearer);

            RestResponse response = client.Execute(request);

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
                    return ("", "");
                }

                int chapterCounter = 0;
                foreach (Chapter item in course.Course.Data)
                {
                    chapterCounter++;

                    _chapterCounterEvent?.Invoke($"{chapterCounter} / {course.Course.Data.Count}");
                    coursename = GetChapter(Options.GetOptions(), lines, chapterCounter, bid, item.Id.ToString());
                    Random rand = new();
                    System.Threading.Thread.Sleep(rand.Next(500, 1500));

                    if (lines <= _cumLines)
                    {
                        break;
                    }
                }
            }
            return (_pgn.ToString(), coursename);
        }

        private string GetChapter(JsonSerializerOptions caseInvariant, int lines, int chapter, string bid, string lid)
        {
            RestClient client = new($"https://www.chessable.com/api/v1/getList?uid={_uid}&bid={bid}&lid={lid}");

            RestRequest request = GenerateRequest(_bearer);

            RestResponse response = client.Execute(request);


            string content = response.Content ?? "";
            string coursename = "";
            if (content != null)
            {
                ResponseChapter responseChapter = JsonSerializer.Deserialize<ResponseChapter>(content, options: caseInvariant) ?? new ResponseChapter();
                coursename = responseChapter.List.Name;
                int count = 0;

                foreach (Line line in responseChapter.List.Data)
                {
                    count++;
                    _lineCounterEvent?.Invoke($"{count} / {responseChapter.List.Data.Count}");

                    PgnInfo pgnHeader = new()
                    {
                        Event = responseChapter.List.Name,
                        Round = chapter + 1,
                        Subround = count + 1,
                        White = line.Name,
                        Black = responseChapter.List.Title
                    };

                    GetLine(Options.GetOptions(), pgnHeader, line.Id.ToString());

                    if (lines < _cumLines)
                    {
                        break;
                    }

                }
            }
            return coursename;
        }

        private void GetLine(JsonSerializerOptions caseInvariant, PgnInfo pgnHeader, string oid, string json = "")
        {
            string content = "";
            if (json == "")
            {
                RestClient client = new($"https://www.chessable.com/api/v1/getGame?lng=en&uid={_uid}&oid={oid}");

                RestRequest request = GenerateRequest(_bearer);

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

                _ = (_pgn?.Append($"""
                        
                        [Event "{pgnHeader.Event}"]
                        [Round "{pgnHeader.Round:000}.{pgnHeader.Subround:000}"]
                        [White "{pgnHeader.White}"]
                        [Black "{pgnHeader.Black}"]
                        [FEN "{pgnHeader.FEN}"]
                        [Result "*"]

                        {pgn}


                        """));
                /*
                Invoke(new Action(() =>
                {
                    textBoxCumulativeLines.Text = _cumLines.ToString();
                }));*/
            }

        }

        private static RestRequest GenerateRequest(string bearer)
        {
            RestRequest request = new("", Method.Get);
            _ = request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:138.0) Gecko/20100101 Firefox/138.0");
            _ = request.AddHeader("accept", "application/json, text/plain, */*");
            _ = request.AddHeader("accept-language", "en");
            _ = request.AddHeader("accept-encoding", "gzip, deflate, br, zstd");
            _ = request.AddHeader("platform", "Web");
            _ = request.AddHeader("x-os-name", "Firefox");
            _ = request.AddHeader("x-os-version", "138");
            _ = request.AddHeader("x-device-model", "Windows");
            _ = request.AddHeader("authorization", $"Bearer {bearer}");
            _ = request.AddHeader("alt-used", "www.chessable.com");
            _ = request.AddHeader("connection", "keep-alive");
            _ = request.AddHeader("sec-fetch-dest", "empty");
            _ = request.AddHeader("sec-fetch-mode", "cors");
            _ = request.AddHeader("sec-fetch-site", "same-origin");
            _ = request.AddHeader("priority", "u=0");
            _ = request.AddHeader("te", "trailers");
            _ = request.AddHeader("pragma", "no-cache");
            _ = request.AddHeader("cache-control", "no-cache");

            return request;
        }

        public void SetChapterCounterEvent(Action<string> setChapterCounter)
        {
            _chapterCounterEvent = setChapterCounter;
        }

        public void SetLineCounterEvent(Action<string> setLineCounter)
        {
            _lineCounterEvent = setLineCounter;
        }
    }
}
