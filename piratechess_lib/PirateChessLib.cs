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
        private Action<string>? _errorDiagEvent;
        private readonly List<string> _errorDetails = new();
        private const int MaxErrorDetails = 100;
        private readonly StringBuilder _pgn = new();
        private string _bearer = string.Empty;
        private string _uid = string.Empty;

        public RestResponseCourse? restResponseCourse { get; set; }
        public int ErrorCount => _errorCount;
        /// <summary>Details, including the full stack trace, of every line or chapter that was
        /// skipped while parsing. These exceptions are swallowed on purpose (one broken line must
        /// not abort the whole course), so without this list they would vanish without a trace.
        /// Reset at the start of each <see cref="GetCourse"/> run.</summary>
        public IReadOnlyList<string> ErrorDetails => _errorDetails;
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
            _errorDetails.Clear();
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
            // Guard against SYSTEMATIC failure: single corrupt lines are skipped on purpose (see
            // GetLine), but when far more lines fail than arrive, that is a parser bug or a broken
            // cache, not a data problem. Fail loudly instead of returning a stub course as success.
            // Only for cached data: in the live fetch _errorCount also counts retries, so it would
            // not be a fair measure there.
            if (useLocalData && _errorCount > 10 && _errorCount > _cumLines)
                throw new InvalidOperationException(
                    $"Course export aborted: {_errorCount} lines/chapters skipped but only {_cumLines} exported. " +
                    "That points to a systematic parser problem rather than single corrupt lines (see ErrorDetails).");
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
                ResponseChapter responseChapter;
                try
                {
                    responseChapter = JsonSerializer.Deserialize<ResponseChapter>(content, options: caseInvariant) ?? new ResponseChapter();
                }
                catch (JsonException ex)
                {
                    // A truncated or corrupt chapter body (e.g. a download that broke off mid-stream)
                    // is not empty, so it slips past the check above. Skip it like an empty one
                    // instead of letting the whole course export crash.
                    _errorCount++;
                    RecordError($"[{chapter + 1:000}] Chapter JSON skipped (corrupt or truncated)", ex, content);
                    return coursename;
                }
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
                string lineRef = $"{pgnHeader.Round:000}.{pgnHeader.Subround:000}";
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
                ResponseLine? game;
                try
                {
                    game = JsonSerializer.Deserialize<ResponseLine>(content, options: caseInvariant);
                }
                catch (JsonException ex)
                {
                    // Same as for the chapter: one truncated line must not kill the whole course.
                    _errorCount++;
                    RecordError($"[{lineRef}] Line JSON skipped (corrupt or truncated)", ex, content);
                    return;
                }
                string? pgn;
                try
                {
                    pgn = game?.Game?.GeneratePGN(AllKeyMovesTraining, NoTrainingMove);
                }
                catch (Exception ex)
                {
                    // Corrupt move/variation data must not take down the whole course export (in the
                    // WinForm app an unhandled exception here kills the process). Skip just this line,
                    // like an empty/broken line JSON above; RecordError keeps the full stack trace.
                    _errorCount++;
                    RecordError($"[{lineRef}] GeneratePGN skipped (corrupt move or variation data)", ex, content);
                    return;
                }
                // GeneratePGN tolerates duplicate move ids (last one wins). The line stays in the
                // export, but the possible loss of a move must not go unnoticed.
                int dupIds = game?.Game?.DuplicateMoveIds ?? 0;
                if (dupIds > 0)
                {
                    _errorCount++;
                    RecordError($"[{lineRef}] Duplicate move ids in line ({dupIds}), last move per id kept, PGN may be incomplete", null, content);
                }

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

        /// <summary>Diagnostics callback for skipped lines and chapters. Fires once per swallowed
        /// parser exception with context and the full stack trace.</summary>
        public void SetErrorDiagEvent(Action<string> errorDiagEvent)
        {
            _errorDiagEvent = errorDiagEvent;
        }

        private void RecordError(string context, Exception? ex = null, string? snippet = null)
        {
            // Short form for the GUI log (the retry event is the log channel of both GUIs).
            _retryEvent?.Invoke(ex == null ? context : $"{context}: {ex.GetType().Name}: {ex.Message}");

            var sb = new StringBuilder(context);
            if (!string.IsNullOrEmpty(snippet))
            {
                var trimmed = snippet.Length > 300 ? snippet.Substring(0, 300) + "…" : snippet;
                sb.Append(" | snippet: ").Append(trimmed.Replace('\n', ' ').Replace('\r', ' '));
            }
            if (ex != null)
            {
                sb.Append(" | ").Append(ex.GetType().Name).Append(": ").Append(ex.Message);
                sb.Append('\n').Append(ex.StackTrace);
            }
            var detail = sb.ToString();
            if (_errorDetails.Count < MaxErrorDetails)
            {
                _errorDetails.Add(detail);
            }
            _errorDiagEvent?.Invoke(detail);
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
    