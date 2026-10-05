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
        /// <summary>Start position of a game — fallback when the line names no FEN of its own.</summary>
        private const string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        /// <summary>Chessable's placeholder for "no move" (introduction lines) — an empty San counts too.</summary>
        private static bool IsNullSan(string? san) => (san ?? "").Trim() is "" or "--";

        /// <summary>Two comments without a move in between ("} {") — see <see cref="GeneratePGN"/>.</summary>
        private static readonly Regex CommentGap = new(@"\}\s*\{", RegexOptions.Compiled);

        /// <summary>Board from a FEN without throwing (Chessable also ships pattern diagrams without a king).</summary>
        private static ChessGame? NewGameOrNull(string? fen)
        {
            try { return string.IsNullOrWhiteSpace(fen) ? new ChessGame() : new ChessGame(fen); }
            catch { return null; }
        }

        /// <summary>
        /// Position BEFORE every move of the line — the anchor points for variations (see
        /// <see cref="JsonMoveItemList.GetVariationParts"/>). Chessable ships them in the "before" of the
        /// move; where it is missing (the first move of a line often has no "after" object at all) the
        /// position is replayed from the starting position. What cannot be replayed stays empty — then
        /// there simply is no fallback anchor for that move.
        /// </summary>
        private static List<string> MainlineFens(SortedList<int, JsonMove> moves, Dictionary<int, ResponseMove> afterByMoveId, string? initial)
        {
            var fens = new List<string>(moves.Count);
            ChessGame? game = NewGameOrNull(string.IsNullOrWhiteSpace(initial) ? StartFen : initial);
            for (int i = 0; i < moves.Count; i++)
            {
                string fromJson = afterByMoveId.TryGetValue(moves.Keys[i], out var r) ? (r.Before ?? "") : "";
                fens.Add(fromJson != "" ? fromJson : (game?.GetFen() ?? ""));

                var move = moves.Values[i];
                if (game == null) continue;
                if (IsNullSan(move.San)) { game = null; continue; }   // from here on the position is no longer certain
                var mv = SanToMove(game, (move.San ?? "").Trim());
                if (mv == null) { game = null; continue; }
                try { game.MakeMove(mv, false); }
                catch { game = null; }
            }
            return fens;
        }

        public string GeneratePGN(bool allKeyMovesTraining = false, bool noTrainingMove = false)
        {
            string pgn = "";
            SortedList<int, JsonMove> sortedMoves = [];
            var afterByMoveId = new Dictionary<int, ResponseMove>();
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
                        afterByMoveId[move.Id] = responseMoveAfter;
                        move.CommentAfter = string.Join(" ", responseMoveAfter.Data
                            .Where(d => d.Key == "C")
                            .Select(d => d.CommentAfter)
                            .Where(c => c != ""));
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

            // Variations only now, when every position of the line is known: a cluster that cannot be
            // played from its parent move is attached to the position its MOVE NUMBER points at (see
            // GetVariationParts). Chessable hangs the reference lines of an introduction on the null move
            // at the end — unplayable from there, but ordinary variations from move 1 on.
            var anchorFens = MainlineFens(sortedMoves, afterByMoveId, Initial);
            var variationsAt = anchorFens.Select(_ => new List<string>()).ToList();
            for (int i = 0; i < sortedMoves.Count; i++)
            {
                if (!afterByMoveId.TryGetValue(sortedMoves.Keys[i], out var resp) || resp.Data == null) continue;
                foreach (var data in resp.Data.Where(d => d.Key == "V"))
                {
                    foreach (var (anchor, part) in data.GetVariationParts(resp.Before ?? "", anchorFens))
                    {
                        if (part == "") continue;
                        variationsAt[anchor >= 0 && anchor < variationsAt.Count ? anchor : i].Add(part);
                    }
                }
            }
            for (int i = 0; i < sortedMoves.Count; i++)
                sortedMoves.Values[i].CommentVariations = string.Join(" ", variationsAt[i]);

            int lastMove = 0;
            // Move numbers like Chessable's own export: "N." before white, "N..." before black when black
            // starts the line or moves right after variations (otherwise a strict PGN reader cannot place it).
            var initialParts = (Initial ?? "").Split(' ');
            bool blackStarts = initialParts.Length > 1 && initialParts[1] == "b";
            // Chessable's NULL MOVE ("--") sits at the END of introduction lines, where no move follows.
            // It is not written out: chess.js — the PGN reader of RookHub's viewer, move list and
            // repertoire view — does not know it and silently drops the WHOLE game. Its comments are
            // kept. Only at the end: with a real move behind it the move sequence would be wrong
            // without a placeholder (never seen in real data).
            var trailingNullIds = new HashSet<int>();
            for (int i = sortedMoves.Count - 1; i >= 0; i--)
            {
                if (!IsNullSan(sortedMoves.Values[i].San)) break;
                trailingNullIds.Add(sortedMoves.Keys[i]);
            }

            bool afterVariations = false;
            foreach (var moveEntry in sortedMoves)
            {
                JsonMove move = moveEntry.Value;
                bool nullMove = trailingNullIds.Contains(moveEntry.Key);

                if (move.CommentBefore != "")
                {
                    pgn += $"{{{move.CommentBefore}}} ";
                }

                if (!nullMove)
                {
                    if (lastMove < move.Move)
                    {
                        pgn += lastMove == 0 && blackStarts ? $"{move.Move}... " : $"{move.Move}. ";
                    }
                    else if (afterVariations)
                    {
                        pgn += $"{move.Move}... ";
                    }
                    pgn += move.San + " ";
                }

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

                if (!nullMove) lastMove = move.Move;
            }

            // Merge two comments without a move in between into ONE: chess.js rejects "{a} {b}" and drops
            // the game, and ChessBase showed nothing after the first block. A "}" cannot come from the
            // Chessable text (ReplaceCommentStuff replaces curly braces), so the spot is unambiguous.
            pgn = CommentGap.Replace(pgn, " ");
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
        public string GetVariationPgn(string branchFen) =>
            string.Join(" ", GetVariationParts(branchFen, []).Select(p => p.Pgn));

        /// <summary>
        /// Like <see cref="GetVariationPgn(string)"/>, but with FALLBACK ANCHORS: when a cluster cannot be
        /// played from the parent position, the position its MOVE NUMBER points at is used instead
        /// (<paramref name="anchorFens"/>[i] = position before move i of the line). Those are the
        /// transposition/reference notes Chessable hangs on the null move at the end of an introduction
        /// line: unplayable from there, ordinary variations from their own move number on. Without this
        /// half the introduction ended up as comment text — plain, unclickable text in ChessBase.
        /// <para>Per cluster: <c>Anchor</c> = index into <paramref name="anchorFens"/> the variation must
        /// hang on, or -1 for the parent position (also for the comment fallback).</para>
        /// </summary>
        public List<(int Anchor, string Pgn)> GetVariationParts(string branchFen, IReadOnlyList<string> anchorFens)
        {
            var result = new List<(int Anchor, string Pgn)>();
            if (Key != "V" || Val == null || Val.Value.ValueKind != JsonValueKind.Array)
                return result;

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
                    else if (lastOrder != int.MinValue)
                    {
                        // A move WITHOUT a number continues the line and takes the next half-move. Without
                        // counting it, "… 3.Nc3 a6 … 3...h6" looked like a continuation (7 > 6) although
                        // "a6" already holds half-move 7 — and the whole block became a comment.
                        lastOrder++;
                    }
                }
                cur.Add(item);
            }
            if (cur.Count > 0) clusters.Add(cur);

            // ---- Stage 2: replay each cluster, then variation, sub-variation or comment ----
            var rendered = new List<RenderedCluster>();   // variations already built from THIS V block
            var entries = new List<(int Anchor, RenderedCluster? Cluster, string Text)>();

            foreach (var cluster in clusters)
            {
                string firstRaw = cluster
                    .Where(it => it.Key == "S")
                    .Select(it => ((it.Val?.ValueKind == JsonValueKind.String ? it.Val.Value.GetString() : "") ?? "").Trim())
                    .FirstOrDefault(r => r != "") ?? "";
                int? ord = MoveOrder(firstRaw);

                int anchor = -1;
                RenderedCluster? host = null;
                int hostSpot = -1;
                RenderedCluster? built = RenderCluster(cluster, branchFen);

                // (1) Not playable from here? Then look for the MAIN LINE position its MOVE NUMBER points
                // at (full move number AND side to move must match — without that condition a move that
                // happens to be legal somewhere else would be attached there).
                if (built == null && ord.HasValue)
                {
                    for (int a = 0; a < anchorFens.Count; a++)
                    {
                        if (!FenHasOrder(anchorFens[a], ord.Value)) continue;
                        built = RenderCluster(cluster, anchorFens[a]);
                        if (built == null) continue;
                        anchor = a;
                        break;
                    }
                }

                // (2) Otherwise: an alternative to a move INSIDE a previous variation of the same block
                // ("… 7.f4 7...Be7 … 7...Qb6 is 'best'"). That is where the sentence means it — collected
                // at the end of the line those pieces read as disconnected fragments.
                if (built == null && ord.HasValue)
                {
                    for (int r = rendered.Count - 1; r >= 0 && built == null; r--)
                    {
                        foreach (var spot in rendered[r].Spots)
                        {
                            if (spot.Order != ord.Value) continue;
                            built = RenderCluster(cluster, spot.Fen);
                            if (built == null) continue;
                            host = rendered[r];
                            hostSpot = spot.TokenIndex;
                            break;
                        }
                    }
                }

                if (built == null)
                {
                    string rawText = PlainTextOf(cluster);
                    if (rawText != "") entries.Add((-1, null, $"{{{rawText}}}"));
                    continue;
                }

                rendered.Add(built);
                if (host != null) host.AddNested(hostSpot, built);   // written out WITH its host
                else entries.Add((anchor, built, ""));
            }

            foreach (var (a, c, t) in entries)
                result.Add((a, c != null ? c.ToPgn() : t));
            return result;
        }

        /// <summary>A built variation: the tokens of its body, the positions BEFORE its moves (anchor points
        /// for sub-variations) and the sub-variations per move token.</summary>
        private sealed class RenderedCluster
        {
            public List<string> Body { get; } = [];
            public List<(int Order, string Fen, int TokenIndex)> Spots { get; } = [];
            private readonly Dictionary<int, List<RenderedCluster>> _nested = [];

            /// <summary>Attaches a sub-variation behind the move <paramref name="moveTokenIndex"/> — and behind
            /// the comments belonging to that move, otherwise the alternative would sit in mid-sentence.</summary>
            public void AddNested(int moveTokenIndex, RenderedCluster child)
            {
                int at = moveTokenIndex;
                while (at + 1 < Body.Count && Body[at + 1].StartsWith('{')) at++;
                if (!_nested.TryGetValue(at, out var list)) _nested[at] = list = [];
                list.Add(child);
            }

            public string ToPgn()
            {
                var sb = new StringBuilder("(");
                for (int i = 0; i < Body.Count; i++)
                {
                    if (sb.Length > 1) sb.Append(' ');
                    sb.Append(Body[i]);
                    if (!_nested.TryGetValue(i, out var kids)) continue;
                    foreach (var kid in kids) { sb.Append(' '); sb.Append(kid.ToPgn()); }
                }
                return sb.Append(')').ToString();
            }
        }

        /// <summary>Replays a cluster from <paramref name="fen"/>; returns the built variation, or null when
        /// it is not playable from there (illegal move, null move, unknown FEN, no move at all). The
        /// position before every move is kept — sub-variations are anchored at those.</summary>
        private static RenderedCluster? RenderCluster(List<JsonMoveItemList> cluster, string? fen)
        {
            ChessGame? game = TryNewGame(fen);
            if (game == null) return null;

            var rc = new RenderedCluster();
            bool anyMove = false;
            foreach (var item in cluster)
            {
                if (item.Key == "C")
                {
                    string c = item.CommentAfter;
                    if (c != "") rc.Body.Add($"{{{c}}}");
                }
                else if (item.Key == "V")
                {
                    // Nested variation: embed as plain text (valid and simple).
                    string nested = item.FlattenToText();
                    if (nested != "") rc.Body.Add($"{{{nested}}}");
                }
                else if (item.Key == "S")
                {
                    string raw = ((item.Val?.ValueKind == JsonValueKind.String ? item.Val.Value.GetString() : "") ?? "").Trim();
                    if (raw == "") continue;
                    if (raw.Contains("--")) return null;
                    string fenBefore = game.GetFen();
                    rc.Spots.Add((OrderOfFen(fenBefore), fenBefore, rc.Body.Count));
                    var mv = Game.SanToMove(game, StripMoveNumber(raw));
                    if (mv == null) return null;
                    try { game.MakeMove(mv, false); }
                    catch { return null; }
                    anyMove = true;
                    rc.Body.Add(raw);
                }
            }

            return anyMove && rc.Body.Count > 0 ? rc : null;
        }

        /// <summary>Move-number key of a position (<see cref="MoveOrder"/>: white = N*2, black = N*2+1).</summary>
        private static int OrderOfFen(string fen)
        {
            var parts = (fen ?? "").Split(' ');
            if (parts.Length < 6 || !int.TryParse(parts[5], out int fullmove)) return -1;
            return fullmove * 2 + (parts[1] == "b" ? 1 : 0);
        }

        /// <summary>The cluster as plain text — the fallback when it is not playable anywhere.</summary>
        private static string PlainTextOf(List<JsonMoveItemList> cluster)
        {
            var sb = new StringBuilder();
            foreach (var item in cluster)
            {
                if (item.Key == "C") { string c = item.CommentAfter; if (c != "") AppendText(sb, c); }
                else if (item.Key == "V") { string n = item.FlattenToText(); if (n != "") AppendText(sb, n); }
                else if (item.Key == "S")
                {
                    string raw = ((item.Val?.ValueKind == JsonValueKind.String ? item.Val.Value.GetString() : "") ?? "").Trim();
                    if (raw != "") AppendText(sb, raw);
                }
            }
            return sb.ToString().Trim();
        }

        /// <summary>Does the position match the move number of a token (<see cref="MoveOrder"/>: white = N*2,
        /// black = N*2+1)? Only the FEN itself is read — full move number and side to move.</summary>
        private static bool FenHasOrder(string fen, int order)
        {
            var parts = (fen ?? "").Split(' ');
            if (parts.Length < 6) return false;
            if (!int.TryParse(parts[5], out int fullmove)) return false;
            return fullmove * 2 + (parts[1] == "b" ? 1 : 0) == order;
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
            // No space before a punctuation mark: the plain text of a cluster is assembled from moves and
            // text pieces, and a piece starting with "." or "," belongs to the word before it
            // ("1.d4 . And maybe this is true." -> "1.d4. And maybe this is true.").
            if (sb.Length > 0 && !(s.Length > 0 && ",.;:!?)".Contains(s[0]))) sb.Append(' ');
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
