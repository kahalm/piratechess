using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RestSharp;

namespace piratechess_lib
{
    public class PirateChessLib
    {
        private int _cumLines = 0;
        private Action<string>? _chapterCounterEvent;
        private Action<string>? _lineCounterEvent;
        private readonly StringBuilder _pgn = new();
        private string _bearer = string.Empty;
        private string _uid = string.Empty;

        public PirateChessLib()
        {

        }

        public PirateChessLib(string uid, string bearer)
        {
            _uid = uid;
            _bearer = bearer;
        }

        public (string, string) GetCourse(string bid, int lines = 10000)
        {
            _cumLines = 0;
            string coursename = string.Empty;
            JsonSerializerOptions caseInvariant = Options.GetOptions();

            string url = $"https://www.chessable.com/api/v1/getCourse?uid={_uid}&bid={bid}";
            RestClient client = new(url);

            RestRequest request = GenerateRequest(_bearer, Method.Get);

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

            RestRequest request = GenerateRequest(_bearer, Method.Get);

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

                RestRequest request = GenerateRequest(_bearer, Method.Get);

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
            }

        }

        public Dictionary<string, string> GetChapters()
        {
            var chapters = new Dictionary<string, string>();
            var client = new RestClient($"https://www.chessable.com/api/v1/getHomeData?uid={_uid}&sortBookRowsBy=alphabetically&userLanguageShort=en");

            RestRequest request = GenerateRequest(_bearer, Method.Get);

            RestResponse response = client.Execute(request);
            var content = response.Content ?? "";

            if (content != null)
            {
                ResponseChapterList responseChapterList = JsonSerializer.Deserialize<ResponseChapterList>(content, options: Options.GetOptions()) ?? new ResponseChapterList();

                foreach (var item in responseChapterList.HomeData.BooksList)
                {
                    chapters.Add(item.Bid.ToString(), item.Name);
                }
            }
            return chapters;
        }

        public string Login(string emailInput, string pwdInput)
        {
            if (string.IsNullOrEmpty(emailInput))
            {
                return "please fill out email.";
            }
            if (string.IsNullOrEmpty(pwdInput))
            {
                return "please fill out password.";
            }
            var hash = ComputeSha512Hash(pwdInput);

            RestClient client = new($"https://www.chessable.com/api/v1/authenticate");

            var requestBody = new
            {
                method = "email",
                credentials = new
                {
                    email = emailInput,
                    password = hash
                },
                providerData = (object?)null,
                mode = "login",
                checkoutData = (object?)null,
                preferredLanguage = "en",
                newsletterChecked = false
            };
            string json = JsonSerializer.Serialize(requestBody);


            RestRequest request = GenerateRequestLogin(json);

            RestResponse response = client.Execute(request);
            var content = response.Content ?? "";

            if (content != null)
            {
                try
                {
                    if (!response.IsSuccessful)
                    {
                        return content;
                    }
                    //--ActivityStatusCode: Uauthorized
                    ResponseLogin? responseLogin = JsonSerializer.Deserialize<ResponseLogin>(content, options: Options.GetOptions());

                    if (responseLogin != null)
                    {
                        _bearer = responseLogin.Jwt;
                        _uid = responseLogin.Uid.ToString();
                    }
                }
                catch (Exception e)
                {
                    return (e.Message);
                }
            }
            else
            {
                return "Antwort war leer - irgendwas ist falsch.";
            }

            return "";
        }
        private static RestRequest GenerateRequest(string bearer, Method method)
        {
            RestRequest request = new("", method);
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

        private static RestRequest GenerateRequestLogin(string json)
        {
            var request = new RestRequest("", Method.Post);
            request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:137.0) Gecko/20100101 Firefox/137.0");
            request.AddHeader("accept", "application/json, text/plain, */*");
            request.AddHeader("accept-language", "en");
            request.AddHeader("accept-encoding", "gzip, deflate, br, zstd");
            request.AddHeader("referer", "https://www.chessable.com/login/");
            request.AddHeader("content-type", "application/json;charset=utf-8");
            request.AddHeader("platform", "Web");
            request.AddHeader("x-os-name", "Firefox");
            request.AddHeader("x-os-version", "137");
            request.AddHeader("x-device-model", "Windows");
            request.AddHeader("origin", "https://www.chessable.com");
            request.AddHeader("alt-used", "www.chessable.com");
            request.AddHeader("connection", "keep-alive");
            request.AddHeader("sec-fetch-dest", "empty");
            request.AddHeader("sec-fetch-mode", "cors");
            request.AddHeader("sec-fetch-site", "same-origin");
            request.AddHeader("dnt", "1");
            request.AddHeader("sec-gpc", "1");
            request.AddHeader("priority", "u=0");

            request.AddJsonBody(json);

            return request;
        }

        static string ComputeSha512Hash(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = SHA512.HashData(bytes);
            StringBuilder builder = new();

            foreach (byte b in hashBytes)
            {
                builder.Append(b.ToString("x2")); // hex format
            }

            return builder.ToString();
        }

        private static int ExtractUidFromToken(string jwtToken)
        {
            if (string.IsNullOrWhiteSpace(jwtToken))
                throw new ArgumentException("Token darf nicht leer sein.", nameof(jwtToken));

            var parts = jwtToken.Split('.');
            if (parts.Length < 2)
                throw new ArgumentException("Ungültiges JWT-Format.");

            string payload = parts[1];
            string json = DecodeBase64Url(payload);

            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("user", out JsonElement userElement) &&
                userElement.TryGetProperty("uid", out JsonElement uidElement))
            {
                return uidElement.GetInt32();
            }

            throw new InvalidOperationException("UID konnte im Token nicht gefunden werden.");
        }

        private static string DecodeBase64Url(string base64Url)
        {
            string padded = base64Url.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
                case 1: padded += "="; break;
            }

            byte[] data = Convert.FromBase64String(padded);
            return Encoding.UTF8.GetString(data);
        }

        public void SetChapterCounterEvent(Action<string> setChapterCounter)
        {
            _chapterCounterEvent = setChapterCounter;
        }

        public void SetLineCounterEvent(Action<string> setLineCounter)
        {
            _lineCounterEvent = setLineCounter;
        }

        public string ExtractUid(string jwt)
        {
            var resp = new ResponseLogin
            {
                Jwt = jwt
            };
            _bearer = jwt;
            try
            {
                _uid = resp.Uid.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string LoginWithBearer(string text)
        {
            _bearer = text;
            _uid = JwtHelper.ExtractUidFromToken(text).ToString();

            return "";
        }
    }
}
    