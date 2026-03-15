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
                                string v = data.GetVariationPgn();
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
            int lastMove = 0;
            foreach (JsonMove move in sortedMoves.Values)
            {
                if (move.CommentBefore != "")
                {
                    pgn += $"{{{move.CommentBefore}}} ";
                }

                if (lastMove < move.Move)
                {
                    pgn += $"{move.Move}. ";
                }
                pgn += move.San + " ";

                var arrowList = move.Draws.Where(x => x.Object == "arrow").ToList();
                var circleList = move.Draws.Where(x => x.Object == "circle").ToList();

                string annotation = "";

                if (arrowList.Count > 0)
                {
                    annotation += "[%cal ";
                    var firstrun = true;
                    foreach (JsonDraw draw in arrowList)
                    {
                        annotation += $"{(firstrun ? "" : ",")}{draw.Color.ToUpper()}{draw.Start}{draw.End}";
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
                        annotation += $"{(firstrun ? "" : ",")}{draw.Color.ToUpper()}{draw.Start}";
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

                if (move.CommentVariations != "")
                {
                    pgn += move.CommentVariations + " ";
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
        public string Before { get; set; } = string.Empty;
        public string CommentAfter { get; internal set; } = string.Empty;
        public string CommentBefore { get; internal set; } = string.Empty;
        public string CommentVariations { get; internal set; } = string.Empty;

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
        public JsonElement? Val { get; set; } //entweder eine Liste von itemList oder ein string.
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

                    comment = string.Join(Environment.NewLine, innerList.Select(x => x.CommentAfter) ?? [""]);
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

                    comment = string.Join(Environment.NewLine, innerList ?? [""]);
                }
                else
                {
                    return "";
                }

                return ReplaceCommentStuff(comment);
            }
        }

        public string GetVariationPgn()
        {
            if (Key != "V" || Val == null || Val.Value.ValueKind != JsonValueKind.Array)
                return "";

            var innerList = JsonSerializer.Deserialize<List<JsonMoveItemList>>(Val.Value, options: Options.GetOptions()) ?? [];
            var sb = new StringBuilder("(");
            string pendingComment = "";

            foreach (var item in innerList)
            {
                if (item.Key == "S" && item.Val != null && item.Val.Value.ValueKind == JsonValueKind.String)
                {
                    if (pendingComment != "")
                    {
                        sb.Append($"{{{pendingComment}}} ");
                        pendingComment = "";
                    }
                    sb.Append(item.Val.Value.GetString());
                    sb.Append(' ');
                }
                else if (item.Key == "C")
                {
                    string c = item.CommentAfter;
                    if (c != "")
                        pendingComment += (pendingComment != "" ? " " : "") + c;
                }
                else if (item.Key == "V")
                {
                    if (pendingComment != "")
                    {
                        sb.Append($"{{{pendingComment}}} ");
                        pendingComment = "";
                    }
                    sb.Append(item.GetVariationPgn());
                    sb.Append(' ');
                }
            }

            if (pendingComment != "")
                sb.Append($"{{{pendingComment}}}");

            return sb.ToString().TrimEnd() + ")";
        }

        [GeneratedRegex("<[^>]*>")]
        private static partial Regex findHtmltags();

        [GeneratedRegex(@"@@StartFEN@@(.+?)@@EndFEN@@")]
        private static partial Regex findFenTags();

    }

    public partial class ResponseLogin
    {
        public string Jwt { get; set; } = string.Empty;

        public int Uid
        {
            get
            {
                return JwtHelper.ExtractUidFromToken(Jwt);
            }
        }
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
