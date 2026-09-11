using System.Text;
using System.Text.Json;
using RestSharp;

namespace piratechess_lib
{
    public class PirateChessLib
    {
        private int _cumLines = 0;
        private int _errorCount = 0;
        private Action<string>? _chapterCounterEvent;
        private Action<string>? _lineCounterEvent;
        private Action<string>? _cumulativeLinesEvent;
        private Action<string>? _retryEvent;
        private readonly StringBuilder _pgn = new();
        private string _bearer = string.Empty;
        private string _uid = string.Empty;

        public RestResponseCourse? restResponseCourse { get; set; }
        public int ErrorCount => _errorCount;
        public bool AllKeyMovesTraining { get; set; } = false;
        public bool NoTrainingMove { get; set; } = false;
        public bool AddMoveToEmptyChapters { get; set; } = false;

        private int _extraDelayMinMs = 0;
        private int _extraDelayMaxMs = 0;

        /// <summary>
        /// Lower bound of the extra wait in ms that is added on top of the built-in
        /// 500-1500 ms delay between two Chessable calls. Negative values become 0.
        /// </summary>
        public int ExtraDelayMinMs
        {
            get => _extraDelayMinMs;
            set => _extraDelayMinMs = value < 0 ? 0 : value;
        }

        /// <summary>
        /// Upper bound of the extra wait in ms. Negative values become 0; if the value is
        /// below <see cref="ExtraDelayMinMs"/>, both bounds are swapped when waiting.
        /// </summary>
        public int ExtraDelayMaxMs
        {
            get => _extraDelayMaxMs;
            set => _extraDelayMaxMs = value < 0 ? 0 : value;
        }

        public PirateChessLib()
        {

        }


        public PirateChessLib(string uid, string bearer)
        {
            _uid = uid;
            _bearer = bearer;
        }

        public (string, string) GetCourse(string bid, int lines = 10000, bool useLocalData = false)
        {
            _cumLines = 0;
            _errorCount = 0;
            string? content = null;
            string coursename = string.Empty;

            JsonSerializerOptions caseInvariant = Options.GetOptions();

            if (useLocalData)
            {
                content = restResponseCourse?.CourseJsonContent;
            
            } else
            {
                string url = $"https://www.chessable.com/api/v1/getCourse?uid={_uid}&bid={bid}";
                RestClient client = new(url);

                RestRequest request = GenerateRequest(_bearer, Method.Get);

                RestResponse response = client.Execute(request);

                content = response.Content;
            }

            if (content != null)
            {
                if (!useLocalData)
                {
                    restResponseCourse = new()
                    {
                        CourseJsonContent = content
                    };
                }
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
                    var chapterName = GetChapter(Options.GetOptions(), lines, chapterCounter, bid, item.Id.ToString(), useLocalData);
                    if (!string.IsNullOrEmpty(chapterName))
                        coursename = chapterName; // skipped/empty chapters must not overwrite the course name
                    if (!useLocalData)
                    {
                        SleepBetweenCalls();
                    }
                    if (lines <= _cumLines)
                    {
                        break;
                    }
                }
            }
            return (_pgn.ToString(), coursename);
        }

        /// <summary>
        /// Waits between two Chessable calls: the built-in 500-1500 ms baseline plus the
        /// extra delay set in the GUI via <see cref="ExtraDelayMinMs"/> and
        /// <see cref="ExtraDelayMaxMs"/>.
        /// </summary>
        private void SleepBetweenCalls()
        {
            int extraMin = _extraDelayMinMs;
            int extraMax = _extraDelayMaxMs;
            if (extraMax < extraMin)
            {
                (extraMin, extraMax) = (extraMax, extraMin);
            }

            int extra = extraMax > extraMin ? Random.Shared.Next(extraMin, extraMax + 1) : extraMin;
            System.Threading.Thread.Sleep(Random.Shared.Next(500, 1500) + extra);
        }

        private string GetChapter(JsonSerializerOptions caseInvariant, int lines, int chapter, string bid, string lid, bool useLocalData)
        {
            string? content = null;
            string coursename = "";
            RestResponseChapter? restResponseChapter = null;

            if (useLocalData)
            {
                if (chapter - 1 < restResponseCourse?.ChapterList.Count)
                {
                    restResponseChapter = restResponseCourse?.ChapterList[chapter - 1];
                }
                content = restResponseChapter?.ChapterJsonContent;
            }
            else
            {
                RestClient client = new($"https://www.chessable.com/api/v1/getList?uid={_uid}&bid={bid}&lid={lid}");

                RestRequest request = GenerateRequest(_bearer, Method.Get);

                RestResponse response = client.Execute(request);
                content = response.Content ?? "";
            }
            if (content != null)
            {
                if (!useLocalData)
                {
                    restResponseChapter = new RestResponseChapter
                    {
                        ChapterJsonContent = content
                    };

                    restResponseCourse?.ChapterList.Add(restResponseChapter);
                }
                // Skip an empty/invalid chapter (e.g. a failed fetch stored in the cache)
                // instead of letting JsonSerializer crash.
                if (string.IsNullOrWhiteSpace(content) || content == "{}")
                {
                    _errorCount++;
                    return coursename;
                }
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

                    GetLine(Options.GetOptions(), pgnHeader, line.Id.ToString(), restResponseChapter, count, useLocalData);

                    if (!useLocalData)
                    {
                        SleepBetweenCalls();
                    }

                    if (lines < _cumLines)
                    {
                        break;
                    }

                }
            }
            return coursename;
        }

        private void GetLine(JsonSerializerOptions caseInvariant, PgnInfo pgnHeader, string oid, RestResponseChapter? restResponseChapter, int lineCounter, bool useLocalData = false, string json = "")
        {
            string? content = null;
            if (json == "" && useLocalData)
            {
                if (lineCounter - 1 < restResponseChapter?.ResponseLineList.Count())
                {
                    content = restResponseChapter?.ResponseLineList[lineCounter - 1].LineJsonContent;
                }
            } else if (json == "")
            {
                RestClient client = new($"https://www.chessable.com/api/v1/getGame?lng=en&uid={_uid}&oid={oid}");
                RestRequest request = GenerateRequest(_bearer, Method.Get);
                string round = $"{pgnHeader.Round:000}.{pgnHeader.Subround:000}";

                for (int attempt = 0; attempt < 10; attempt++)
                {
                    RestResponse response = client.Execute(request);
                    content = response.Content;

                    if (!string.IsNullOrWhiteSpace(content) && content != "{}")
                        break;

                    if (attempt < 9)
                    {
                        _errorCount++;
                        _retryEvent?.Invoke($"[{round}] Retry {attempt + 1}/10 ...");
                        System.Threading.Thread.Sleep(30000 + Random.Shared.Next(0, 5000));
                    }
                    else
                    {
                        _errorCount++;
                        _retryEvent?.Invoke($"[{round}] FAILED after 10 attempts, skipping.");
                    }
                }
            }
            else
            {
                content = json;
            }

            if (content != null)
            {
                if (!useLocalData)
                {
                    restResponseChapter?.ResponseLineList.Add(new RestResponseLine
                    {
                        LineJsonContent = content
                    });
                }
                // Skip an empty/invalid line (e.g. cached as "" after 10 failed fetch retries)
                // instead of letting JsonSerializer crash — otherwise a single line kills the
                // whole course PGN export.
                if (string.IsNullOrWhiteSpace(content) || content == "{}")
                {
                    _errorCount++;
                    return;
                }
                ResponseLine? game = JsonSerializer.Deserialize<ResponseLine>(content, options: caseInvariant);
                string? pgn = game?.Game?.GeneratePGN(AllKeyMovesTraining, NoTrainingMove);

                pgnHeader.FEN = game?.Game?.Initial ?? "";

                var nullMoveMatch = pgn != null
                    ? System.Text.RegularExpressions.Regex.Match(pgn.Trim(), @"^1\.\s*--\s*(\{.*\})?\s*$", System.Text.RegularExpressions.RegexOptions.Singleline)
                    : null;
                bool isNullMoveOnly = nullMoveMatch?.Success == true;
                if (AddMoveToEmptyChapters && (string.IsNullOrWhiteSpace(pgn) || isNullMoveOnly))
                {
                    string comment = isNullMoveOnly && nullMoveMatch!.Groups[1].Success
                        ? " " + nullMoveMatch.Groups[1].Value
                        : "";
                    pgn = "1. e4" + comment;
                    pgnHeader.FEN = "";
                }
                _cumLines++;
                _cumulativeLinesEvent?.Invoke(_cumLines.ToString());

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


        public void SetChapterCounterEvent(Action<string> setChapterCounter)
        {
            _chapterCounterEvent = setChapterCounter;
        }

        public void SetLineCounterEvent(Action<string> setLineCounter)
        {
            _lineCounterEvent = setLineCounter;
        }

        public void SetCumulativeLinesEvent(Action<string> setCumulativeLines)
        {
            _cumulativeLinesEvent = setCumulativeLines;
        }

        public void SetRetryEvent(Action<string> retryEvent)
        {
            _retryEvent = retryEvent;
        }

        public string ExtractUid(string jwt)
        {
            _bearer = jwt;
            try
            {
                _uid = JwtHelper.ExtractUidFromToken(jwt).ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        private const string RepCheckUrl = "https://github.com/kahalm/repcheck";
        private const string RookHubUrl = "https://rookhub.oberschmid.homes";

        // Hint appended to every bearer error message: the token is easiest to grab with
        // the RepCheck extension, and saving goes through RookHub.
        private const string BearerHelpText =
            "Easiest way to get a token is the RepCheck extension (" + RepCheckUrl +
            "): while logged in on chessable.com open the RepCheck popup → \"Chessable-Token\" → \"Token kopieren\". " +
            "Saving needs a RookHub account (" + RookHubUrl + ").";

        public string LoginWithBearer(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return $"Bearer token is empty. {BearerHelpText}";
            }

            text = text.Trim();
            if (text.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                text = text.Substring(7).Trim();
            }

            var parts = text.Split('.');
            if (parts.Length != 3 || parts.Any(string.IsNullOrWhiteSpace))
            {
                return $"Invalid token format: expected 3 Base64 blocks separated by dots (header.payload.signature), found {parts.Length}. {BearerHelpText}";
            }

            var exp = JwtHelper.GetExpiration(text);
            if (exp.HasValue && exp.Value <= DateTimeOffset.UtcNow)
            {
                return $"Bearer token has expired (exp: {exp.Value.UtcDateTime:yyyy-MM-dd HH:mm} UTC). Please get a new one. {BearerHelpText}";
            }

            try
            {
                _uid = JwtHelper.ExtractUidFromToken(text).ToString();
            }
            catch (Exception ex)
            {
                return $"Token could not be read: {ex.Message}. {BearerHelpText}";
            }
            _bearer = text;

            return "";
        }
    }
}
    