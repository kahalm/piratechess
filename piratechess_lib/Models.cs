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
                        move.CommentAfter = string.Join(Environment.NewLine, responseMoveAfter.Data.Select(x => x.CommentAfter).ToList());
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
                if (move.CommentAfter != "")
                {
                    pgn += $"{{{move.CommentAfter}}} ";
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
                    List<string>? innerList = JsonSerializer.Deserialize<List<JsonMoveItemList>>(Val.Value, options: Options.GetOptions())?.Select(x => x.CommentAfter).ToList();

                    comment = string.Join(Environment.NewLine, innerList ?? [""]);
                }
                else
                {
                    return "";
                }

                return replaceCommentStuff(comment);
            }
        }

        private string replaceCommentStuff(string comment)
        {
            comment = comment.Replace("@@StartBracket@@", "(").Replace("@@EndBracket@@", ")");
            comment = comment.Replace("@@StartFEN@@", "").Replace("@@EndFEN@@", "");
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

                return replaceCommentStuff(comment);
            }
        }

        [GeneratedRegex("<[^>]*>")]
        private static partial Regex findHtmltags();

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
        public JsonHomeData HomeData { get; set; } = new ();
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
}
