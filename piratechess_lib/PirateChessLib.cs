using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;

namespace piratechess_lib
{
    public class PirateChessLib(string uid, string bearer)
    {
        private int _cumLines = 0;
        private readonly StringBuilder _pgn = new();
        private readonly string _bearer = bearer;
        private readonly string _uid = uid;

        public (string,string) GetCourse(string bid, int lines = 10000)
        {   
            _cumLines = 0;
            var coursename = string.Empty; 
            var caseInvariant = Options.GetOptions();

            var url = $"https://www.chessable.com/api/v1/getCourse?uid={_uid}&bid={bid}";
            RestClient client = new(url);


            var request = GenerateRequest(_bearer);

            var response = client.Execute(request);
            /*Todo
            Invoke(new Action(() =>
            {
                textBoxPGN.Text = "";
                textBoxCumulativeLines.Text = _cumLines.ToString();
            }));
            */
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
                    return ("","");
                }

                var durchlauf = 0;
                foreach (Chapter item in course.Course.Data)
                {
                    durchlauf++;
                    /*Todo
                    Invoke(new Action(() =>
                    {
                        textBoxDurchlauf.Text = $"{durchlauf} / {course.Course.Data.Count}";
                        textBoxLid.Text = item.Id.ToString();
                    }));
                    */
                    coursename = GetChapter(Options.GetOptions(), lines, durchlauf, bid, item.Id.ToString());
                    var rand = new Random();
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

            var request = GenerateRequest(_bearer);

            RestResponse response = client.Execute(request);


            string content = response.Content ?? "";
            string coursename = "";
            if (content != null)
            {
                ResponseChapter responseChapter = JsonSerializer.Deserialize<ResponseChapter>(content, options: caseInvariant) ?? new ResponseChapter();
                coursename = responseChapter.List.Name;
                var count = 0;

                foreach (Line line in responseChapter.List.Data)
                {
                    count++;
                    /*Todo
                    Invoke(new Action(() =>
                    {
                        textBoxCurLines.Text = $"{count} / {responseChapter.List.Data.Count}";
                        textBoxOid.Text = line.Id.ToString();

                    }));
                    */
                    var pgnHeader = new PgnInfo
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

                var request = GenerateRequest(_bearer);

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
                        [Round "{pgnHeader.Round:000}.{pgnHeader.Subround:000}"]
                        [White "{pgnHeader.White}"]
                        [Black "{pgnHeader.Black}"]
                        [FEN "{pgnHeader.FEN}"]
                        [Result "*"]

                        {pgn}


                        """);
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
    }
}
