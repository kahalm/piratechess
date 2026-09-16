using ChessDotNet;
using ChessDotNet.Pieces;
using RestSharp;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace piratechess_lib
{

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
        public string Color { get; set; } = string.Empty;
        public int IsInfo { get; set; }
        /// <summary>Number of colliding move ids in the last <see cref="GeneratePGN"/> run (corrupt
        /// Chessable data, the last move per id wins). Above 0 the PGN may be missing real moves, so
        /// PirateChessLib.GetLine reports it instead of passing it off as a clean export.</summary>
        public int DuplicateMoveIds { get; private set; }
        public string GeneratePGN(bool allKeyMovesTraining = false, bool noTrainingMove = false)
        {
            string pgn = "";
            SortedList<int, JsonMove> sortedMoves = [];
            Data ??= [];
            DuplicateMoveIds = 0;
            foreach (JsonMove move in Data)
            {
                // Indexer instead of Add: duplicate move ids (corrupt Chessable data) overwrite instead
                // of throwing an ArgumentException that aborted the whole course export. Count them,
                // so the silent loss of a move becomes visible to the caller.
                if (sortedMoves.ContainsKey(move.Id)) DuplicateMoveIds++;
                sortedMoves[move.Id] = move;

                if (move.After is not null and not "")
                {
                    ResponseMove? responseMoveAfter = JsonSerializer.Deserialize<ResponseMove>(move.After, options: Options.GetOptions());
                    if (responseMoveAfter != null && responseMoveAfter.Data != null)
                    {
                        var comments = new List<string>();
                        var variations = new List<string>();
                        foreach (var data in responseMoveAfter.Data)
                        {
                            if (data.Key == "C")
                            {
                                string c = data.CommentAfter;
                                if (c != "") comments.Add(c);
                            }
                            else if (data.Key == "V")
                            {
                                // Branch point of the variation = position BEFORE this move (an
                                // alternative to it). Chessable's "before" holds exactly that FEN.
                                string v = data.GetVariationPgn(responseMoveAfter.Before);
                                if (v != "") variations.Add(v);
                            }
                        }
                        move.CommentAfter = string.Join(" ", comments);
                        move.CommentVariations = string.Join(" ", variations);
                    }
                }

                if (move.Before is not null and not "")
                {
                    ResponseMove? responseMoveBefore = JsonSerializer.Deserialize<ResponseMove>(move.Before, options: Options.GetOptions());
                    if (responseMoveBefore != null && responseMoveBefore.Data != null)
                    {
                        move.CommentBefore = string.Join(Environment.NewLine, responseMoveBefore.Data.Select(x => x.CommentBefore).ToList());
                    }
                }
            }
            if (IsInfo == 1) noTrainingMove = true;
            if (!noTrainingMove && allKeyMovesTraining)
            {
                var allUcis = GetAllTrainingUcis(sortedMoves);
                bool prevKey = false;
                bool pastFirstKey = false;
                int moveIdx = 0;
                var fenParts = (Initial ?? "").Split(' ');
                bool currentIsWhite = fenParts.Length <= 1 || fenParts[1] != "b";
                bool? solverIsWhite = !string.IsNullOrEmpty(Color)
                    ? Color.Equals("white", StringComparison.OrdinalIgnoreCase)
                    : null;
                foreach (JsonMove move in sortedMoves.Values)
                {
                    if (move.IsKey && !prevKey)
                    {
                        pastFirstKey = true;
                        solverIsWhite ??= currentIsWhite;
                    }
                    if (pastFirstKey && move.IsKey && solverIsWhite == currentIsWhite)
                    {
                        string uci = moveIdx < allUcis.Count ? (allUcis[moveIdx] ?? "") : "";
                        string trainingComment = $"[%tqu \"En\",\"find the move\",\"\",\"\",\"{uci}\",\"\",10]";
                        move.CommentBefore = move.CommentBefore == ""
                            ? trainingComment
                            : trainingComment + "\n" + move.CommentBefore;
                    }
                    prevKey = move.IsKey;
                    currentIsWhite = !currentIsWhite;
                    moveIdx++;
                }
            }
            else if (!noTrainingMove)
            {
                bool? solverIsWhite = !string.IsNullOrEmpty(Color)
                    ? Color.Equals("white", StringComparison.OrdinalIgnoreCase)
                    : null;
                string? uci = GetFirstKeyMoveUci(sortedMoves, solverIsWhite);
                var fenParts = (Initial ?? "").Split(' ');
                bool currentIsWhite = fenParts.Length <= 1 || fenParts[1] != "b";
                bool foundKeyBlock = false;
                foreach (JsonMove move in sortedMoves.Values)
                {
                    if (move.IsKey && !foundKeyBlock)
                        foundKeyBlock = true;
                    if (foundKeyBlock && move.IsKey && (solverIsWhite == null || solverIsWhite.Value == currentIsWhite))
                    {
                        string trainingComment = $"[%tqu \"En\",\"find the move\",\"\",\"\",\"{uci ?? ""}\",\"\",10]";
                        move.CommentBefore = move.CommentBefore == ""
                            ? trainingComment
                            : trainingComment + "\n" + move.CommentBefore;
                        break;
                    }
                    currentIsWhite = !currentIsWhite;
                }
            }

            int lastMove = 0;
            // Move numbers like Chessable's own export: "N." before white, "N..." before black when black
            // starts the line or moves right after variations (otherwise a strict PGN reader cannot place it).
            var initialParts = (Initial ?? "").Split(' ');
            bool blackStarts = initialParts.Length > 1 && initialParts[1] == "b";
            bool afterVariations = false;
            foreach (JsonMove move in sortedMoves.Values)
            {
                if (move.CommentBefore != "")
                {
                    pgn += $"{{{move.CommentBefore}}} ";
                }

                if (lastMove < move.Move)
                {
                    pgn += lastMove == 0 && blackStarts ? $"{move.Move}... " : $"{move.Move}. ";
                }
                else if (afterVariations)
                {
                    pgn += $"{move.Move}... ";
                }
                pgn += move.San + " ";

                // Chessable can send "draws": null or single null entries in the list; the
                // property pattern filters null elements out as well (NullReferenceException in
                // GeneratePGN, same fix as in piratechess_docker for bid 282212).
                var arrowList = move.Draws?.Where(x => x is { Object: "arrow" }).ToList() ?? [];
                var circleList = move.Draws?.Where(x => x is { Object: "circle" }).ToList() ?? [];

                string annotation = "";

                if (arrowList.Count > 0)
                {
                    annotation += "[%cal ";
                    var firstrun = true;
                    foreach (JsonDraw draw in arrowList)
                    {
                        annotation += $"{(firstrun ? "" : ",")}{(draw.Color ?? "").ToUpper()}{draw.Start}{draw.End}";
                        firstrun = false;
                    }
                    annotation += "]";
                }

                if (circleList.Count > 0)
                {
                    annotation += "[%csl ";
                    var firstrun = true;
                    foreach (JsonDraw draw in circleList)
                    {
                        annotation += $"{(firstrun ? "" : ",")}{(draw.Color ?? "").ToUpper()}{draw.Start}";
                        firstrun = false;
                    }
                    annotation += "]";
                }

                if (move.CommentAfter != "")
                {
                    annotation += move.CommentAfter;
                }

                if (annotation != "")
                {
                    pgn += $"{{{annotation}}} ";
                }

                // Emit variations right AFTER their own move (they are alternatives to it), not after
                // the following move. Otherwise a PGN reader attaches them to the wrong move and, for
                // a move of the other colour, rejects them as illegal.
                if (move.CommentVariations != "")
                {
                    pgn += move.CommentVariations + " ";
                }
                afterVariations = move.CommentVariations != "";

                lastMove = move.Move;
            }
            return pgn;
        }

        private List<string?> GetAllTrainingUcis(SortedList<int, JsonMove> sortedMoves)
        {
            var result = new List<string?>(sortedMoves.Count);
            try
            {
                ChessGame game = string.IsNullOrEmpty(Initial)
                    ? new ChessGame()
                    : new ChessGame(Initial);

                foreach (var m in sortedMoves.Values)
                {
                    var move = SanToMove(game, m.San);
                    if (move == null) { result.Add(null); break; }

                    char ff = char.ToLower(move.OriginalPosition.File.ToString()[0]);
                    int fr = move.OriginalPosition.Rank;
                    char tf = char.ToLower(move.NewPosition.File.ToString()[0]);
                    int tr = move.NewPosition.Rank;
                    string uciStr = $"{ff}{fr}{tf}{tr}";
                    int eqIdx = m.San.IndexOf('=');
                    if (eqIdx >= 0 && eqIdx + 1 < m.San.Length)
                        uciStr += char.ToLower(m.San[eqIdx + 1]);
                    result.Add(uciStr);

                    game.MakeMove(move, false);
                }
            }
            catch { }
            while (result.Count < sortedMoves.Count)
                result.Add(null);
            return result;
        }

        private string? GetFirstKeyMoveUci(SortedList<int, JsonMove> sortedMoves, bool? solverIsWhite)
        {
            try
            {
                ChessGame game = string.IsNullOrEmpty(Initial)
                    ? new ChessGame()
                    : new ChessGame(Initial);

                var allMoves = sortedMoves.Values.ToList();
                bool foundKeyBlock = false;

                for (int i = 0; i < allMoves.Count; i++)
                {
                    if (allMoves[i].IsKey && !foundKeyBlock)
                        foundKeyBlock = true;

                    if (foundKeyBlock && allMoves[i].IsKey)
                    {
                        bool isWhiteTurn = game.WhoseTurn == Player.White;
                        if (solverIsWhite == null || solverIsWhite.Value == isWhiteTurn)
                        {
                            var move = SanToMove(game, allMoves[i].San);
                            if (move == null) return null;
                            char ff = char.ToLower(move.OriginalPosition.File.ToString()[0]);
                            int fr = move.OriginalPosition.Rank;
                            char tf = char.ToLower(move.NewPosition.File.ToString()[0]);
                            int tr = move.NewPosition.Rank;
                            string uciStr = $"{ff}{fr}{tf}{tr}";
                            int eqIdx = allMoves[i].San.IndexOf('=');
                            if (eqIdx >= 0 && eqIdx + 1 < allMoves[i].San.Length)
                                uciStr += char.ToLower(allMoves[i].San[eqIdx + 1]);
                            return uciStr;
                        }
                    }

                    // Advance the game position for all moves before the target
                    var applyMove = SanToMove(game, allMoves[i].San);
                    if (applyMove == null) return null;
                    game.MakeMove(applyMove, false);
                }
            }
            catch { }
            return null;
        }

        internal static Move? SanToMove(ChessGame game, string san)
        {
            string s = (san ?? string.Empty).TrimEnd('+', '#', '!', '?');
            int backRank = game.WhoseTurn == Player.White ? 1 : 8;

            if (s is "O-O" or "0-0")
                return new Move(new Position(ChessDotNet.File.E, backRank), new Position(ChessDotNet.File.G, backRank), game.WhoseTurn);
            if (s is "O-O-O" or "0-0-0")
                return new Move(new Position(ChessDotNet.File.E, backRank), new Position(ChessDotNet.File.C, backRank), game.WhoseTurn);

            char? promo = null;
            int eqIdx = s.IndexOf('=');
            if (eqIdx >= 0) { promo = eqIdx + 1 < s.Length ? s[eqIdx + 1] : (char?)null; s = s[..eqIdx]; }

            // Insufficient/empty notation (e.g. only a move number left): not a valid move, return
            // null instead of an IndexOutOfRangeException on s[^2] that aborted the whole course.
            if (s.Length < 2) return null;

            var destFile = (ChessDotNet.File)(char.ToLower(s[^2]) - 'a');
            int destRank = s[^1] - '0';
            var validMoves = game.GetValidMoves(game.WhoseTurn);

            bool isPawn = !char.IsUpper(s[0]);
            if (isPawn)
            {
                char? srcFile = s.Length >= 4 ? s[0] : (char?)null;
                foreach (var vm in validMoves)
                {
                    if (vm.NewPosition.File != destFile || vm.NewPosition.Rank != destRank) continue;
                    if (game.GetPieceAt(vm.OriginalPosition) is not Pawn) continue;
                    if (srcFile.HasValue && char.ToLower(vm.OriginalPosition.File.ToString()[0]) != srcFile.Value) continue;
                    return promo.HasValue
                        ? new Move(vm.OriginalPosition, vm.NewPosition, game.WhoseTurn, promo.Value)
                        : vm;
                }
                // ChessDotNet 1.0.0 does not list straight pawn-push promotions (e.g. "e8=Q") in
                // GetValidMoves, capture promotions it does. Without this the key move got no UCI and
                // the rest of the training moves in the line broke off. Build the push directly:
                // origin = same file, one rank behind the target. The promotion rank must match the
                // side to move (white 8, black 1); a colour-blind check computed rank 0 or 9 for
                // corrupt variation data and GetPieceAt threw IndexOutOfRangeException.
                bool promRank = game.WhoseTurn == Player.White ? destRank == 8 : destRank == 1;
                if (promo.HasValue && !srcFile.HasValue && promRank)
                {
                    int originRank = game.WhoseTurn == Player.White ? destRank - 1 : destRank + 1;
                    var origin = new Position(destFile, originRank);
                    if (game.GetPieceAt(origin) is Pawn)
                        return new Move(origin, new Position(destFile, destRank), game.WhoseTurn, promo.Value);
                }
                return null;
            }

            char pieceChar = s[0];
            string mid = s.Length > 3 ? s[1..^2].Replace("x", "") : "";
            char? disambigFile = mid.Length > 0 && char.IsLetter(mid[0]) ? mid[0] : (char?)null;
            int? disambigRank = mid.Length > 0 && char.IsDigit(mid[^1]) ? mid[^1] - '0' : (int?)null;

            foreach (var vm in validMoves)
            {
                if (vm.NewPosition.File != destFile || vm.NewPosition.Rank != destRank) continue;
                var piece = game.GetPieceAt(vm.OriginalPosition);
                if (piece == null || SanPieceChar(piece) != pieceChar) continue;
                if (disambigFile.HasValue && char.ToLower(vm.OriginalPosition.File.ToString()[0]) != disambigFile.Value) continue;
                if (disambigRank.HasValue && vm.OriginalPosition.Rank != disambigRank.Value) continue;
                return vm;
            }
            return null;
        }

        private static char SanPieceChar(Piece piece) => piece switch
        {
            King => 'K',
            Queen => 'Q',
            Rook => 'R',
            Bishop => 'B',
            Knight => 'N',
            _ => 'P'
        };
    }
    public class JsonMove
    {
        public int Id { get; set; }
        public int Move { get; set; }
        public string San { get; set; } = string.Empty;
        public string After { get; set; } = string.Empty;
        public string Before { get; set; } = string.Empty;
        public string CommentAfter { get; internal set; } = string.Empty;
        public string CommentBefore { get; internal set; } = string.Empty;
        public string CommentVariations { get; internal set; } = string.Empty;

        public bool IsKey { get; set; }
        public List<JsonDraw> Draws { get; set; } = [];
    }

    public class JsonDraw
    {
        public string Object { get; set; } = string.Empty;
        public string Start { get; set; } = string.Empty;
        public string End { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Move { get; set; } = string.Empty;
        public string Index { get; set; } = string.Empty;
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
        public JsonElement? Val { get; set; } // either a list of itemList or a string.
        public string CommentAfter
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
                    List<JsonMoveItemList> innerList = JsonSerializer.Deserialize<List<JsonMoveItemList>>(Val.Value, options: Options.GetOptions())?.ToList() ?? new List<JsonMoveItemList>() ;

                    // Parts of ONE entry (text, move reference, text) are running text: join with a space,
                    // not a line break (otherwise the intro read "… dass er\n2.d4\nspielen kann").
                    comment = string.Join(" ", innerList.Select(x => x.CommentAfter) ?? [""]);
                }
                else
                {
                    return "";
                }

                return ReplaceCommentStuff(comment);
            }
        }

        private static string ReplaceCommentStuff(string comment)
        {
            comment = comment.Replace("@@StartBracket@@", "(").Replace("@@EndBracket@@", ")");
            comment = findFenTags().Replace(comment, "");
            comment = comment.Replace("@@StartBlockQuote@@", "").Replace("@@EndBlockQuote@@", "");
            comment = comment.Replace("@@LinkStart@@", "").Replace("@@LinkEnd@@", "");
            comment = comment.Replace("@@SANStart@@", "").Replace("@@SANEnd@@", "");
            comment = comment.Replace("@@HeaderStart@@", "").Replace("@@HeaderEnd@@", "");
            comment = comment.Replace("<br/>", "").Replace("<br>", "");
            comment = comment.Replace("</strong>", "").Replace("<strong>", "");
            comment = comment.Replace("</bold>", "").Replace("<bold>", "");
            comment = findHtmltags().Replace(comment, "");

            // Neutralise curly braces from the Chessable text: the comment is later wrapped in {…},
            // and a "}" inside it would end the comment early and dump the rest into the movetext.
            // PGN has no escaping inside {…}, so use round brackets (they appear there anyway, see
            // @@StartBracket@@ above).
            comment = comment.Replace('{', '(').Replace('}', ')');

            // Whitespace like Chessable's export: Chessable writes paragraphs as " <br/><br/> ", and removed
            // FEN markers leave spaces at the edges (double spaces, "{ist hier in der Zugfolge }").
            comment = findWhitespace().Replace(comment, " ").Trim();

            return comment;
        }

        public string CommentBefore
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
                    List<string>? innerList = JsonSerializer.Deserialize<List<JsonMoveItemList>>(Val.Value, options: Options.GetOptions())?.Select(x => x.CommentAfter).ToList();

                    comment = string.Join(" ", innerList ?? [""]);
                }
                else
                {
                    return "";
                }

                return ReplaceCommentStuff(comment);
            }
        }

        /// <summary>
        /// Turns the Chessable "V" data of a move into PGN. Chessable's "V" holds TWO kinds of items:
        /// (a) real sidelines that branch off at the parent move and replay legally, and
        /// (b) transposition or reference notes with absolute move numbers from move 1 that do NOT
        /// continue from here. Emitting both blindly as <c>(…)</c> produced invalid PGN that could not
        /// be replayed (duplicates, foreign move numbers, null moves "--").
        ///
        /// Two stages: the items are split into clusters (single alternative lines) wherever the move
        /// number jumps back, and EACH cluster is replayed with the engine from the parent position
        /// (<paramref name="branchFen"/> = position BEFORE the parent move). If it replays legally it
        /// becomes a real <c>(…)</c> variation, otherwise (illegal move, null move, unknown FEN) it is
        /// written as a <c>{comment}</c>, so the PGN stays valid and the content is kept.
        /// </summary>
        public string GetVariationPgn(string branchFen)
        {
            if (Key != "V" || Val == null || Val.Value.ValueKind != JsonValueKind.Array)
                return "";

            var innerList = JsonSerializer.Deserialize<List<JsonMoveItemList>>(Val.Value, options: Options.GetOptions()) ?? [];

            // ---- Stage 1: split into clusters (alternative lines) ----
            var clusters = new List<List<JsonMoveItemList>>();
            var cur = new List<JsonMoveItemList>();
            int lastOrder = int.MinValue;
            foreach (var item in innerList)
            {
                if (item.Key == "S")
                {
                    string raw = (item.Val?.ValueKind == JsonValueKind.String ? item.Val.Value.GetString() : "") ?? "";
                    int? ord = MoveOrder(raw);
                    if (ord.HasValue)
                    {
                        if (ord.Value <= lastOrder && cur.Count > 0)
                        {
                            clusters.Add(cur);
                            cur = [];
                            lastOrder = int.MinValue;
                        }
                        lastOrder = ord.Value;
                    }
                }
                cur.Add(item);
            }
            if (cur.Count > 0) clusters.Add(cur);

            // ---- Stage 2: replay each cluster, then variation or comment ----
            var parts = new List<string>();
            foreach (var cluster in clusters)
            {
                ChessGame? game = TryNewGame(branchFen);
                var body = new StringBuilder();         // valid variation notation
                var rawText = new StringBuilder();       // plain-text fallback (comment)
                bool anyMove = false, legal = true, hasNull = false;

                foreach (var item in cluster)
                {
                    if (item.Key == "C")
                    {
                        string c = item.CommentAfter;
                        if (c != "") { body.Append($"{{{c}}} "); AppendText(rawText, c); }
                    }
                    else if (item.Key == "V")
                    {
                        // Nested variation: embed as plain text (valid and simple).
                        string nested = item.FlattenToText();
                        if (nested != "") { body.Append($"{{{nested}}} "); AppendText(rawText, nested); }
                    }
                    else if (item.Key == "S")
                    {
                        string raw = ((item.Val?.ValueKind == JsonValueKind.String ? item.Val.Value.GetString() : "") ?? "").Trim();
                        if (raw == "") continue;
                        anyMove = true;
                        AppendText(rawText, raw);
                        if (raw.Contains("--")) { hasNull = true; continue; }
                        if (game != null && legal && !hasNull)
                        {
                            var mv = Game.SanToMove(game, StripMoveNumber(raw));
                            if (mv != null)
                            {
                                try { game.MakeMove(mv, false); body.Append(raw + " "); }
                                catch { legal = false; }
                            }
                            else legal = false;
                        }
                    }
                }

                if (anyMove && legal && !hasNull)
                {
                    string b = body.ToString().Trim();
                    if (b != "") parts.Add($"({b})");
                }
                else
                {
                    string t = rawText.ToString().Trim();
                    if (t != "") parts.Add($"{{{t}}}");
                }
            }
            return string.Join(" ", parts);
        }

        /// <summary>Flattens a (nested) "V" structure to plain text (moves and comments, no brackets, no FEN context).</summary>
        private string FlattenToText()
        {
            if (Val == null || Val.Value.ValueKind != JsonValueKind.Array) return "";
            var list = JsonSerializer.Deserialize<List<JsonMoveItemList>>(Val.Value, options: Options.GetOptions()) ?? [];
            var sb = new StringBuilder();
            foreach (var it in list)
            {
                if (it.Key == "S")
                {
                    string s = ((it.Val?.ValueKind == JsonValueKind.String ? it.Val.Value.GetString() : "") ?? "").Trim();
                    if (s != "") AppendText(sb, s);
                }
                else if (it.Key == "C") { string c = it.CommentAfter; if (c != "") AppendText(sb, c); }
                else if (it.Key == "V") { string n = it.FlattenToText(); if (n != "") AppendText(sb, n); }
            }
            return sb.ToString().Trim();
        }

        private static ChessGame? TryNewGame(string? fen)
        {
            try { return string.IsNullOrWhiteSpace(fen) ? new ChessGame() : new ChessGame(fen); }
            catch { return null; }
        }

        /// <summary>Sort key of an "N." / "N..." move token (white = N*2, black = N*2+1); null for a bare SAN.</summary>
        private static int? MoveOrder(string raw)
        {
            var mw = findWhiteMoveNumber().Match(raw);
            if (mw.Success) return int.Parse(mw.Groups[1].Value) * 2;
            var mb = findBlackMoveNumber().Match(raw);
            if (mb.Success) return int.Parse(mb.Groups[1].Value) * 2 + 1;
            return null;
        }

        /// <summary>Strips the leading move number ("12." / "12...") from a token; a bare SAN stays as is.</summary>
        private static string StripMoveNumber(string raw)
        {
            var m = findLeadingMoveNumber().Match(raw);
            return m.Success ? raw[m.Length..].Trim() : raw.Trim();
        }

        private static void AppendText(StringBuilder sb, string s)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(s);
        }

        [GeneratedRegex(@"^(\d+)\.(?!\.)")]
        private static partial Regex findWhiteMoveNumber();

        [GeneratedRegex(@"^(\d+)\.\.\.")]
        private static partial Regex findBlackMoveNumber();

        [GeneratedRegex(@"^\d+\.(\.\.)?\s*")]
        private static partial Regex findLeadingMoveNumber();

        [GeneratedRegex("<[^>]*>")]
        private static partial Regex findHtmltags();

        [GeneratedRegex(@"\s+")]
        private static partial Regex findWhitespace();

        [GeneratedRegex(@"@@StartFEN@@(.+?)@@EndFEN@@")]
        private static partial Regex findFenTags();

    }

    public partial class ResponseChapterList
    {
        public JsonHomeData HomeData { get; set; } = new();
    }

    public class JsonHomeData
    {
        public List<JsonBook> BooksList { get; set; } = [];
    }

    public class JsonBook
    {
        public int Bid { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class RestResponseLine
    {
        public string? LineJsonContent { get; set; }
    }
    public class RestResponseChapter
    {
        public string? ChapterJsonContent { get; set; }
        public List<RestResponseLine> ResponseLineList { get; set; } = [];
    } 
    public class RestResponseCourse
    {
        public string? CourseJsonContent { get; set; } 
        public List<RestResponseChapter> ChapterList { get; set; } = [];
    }
}
