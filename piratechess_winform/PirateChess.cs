using piratechess_lib;
using System.Text;
using System.Text.Json;

namespace piratechess_Winform
{
    public partial class PirateChess : Form, IDisposable
    {
        private string _coursename = string.Empty;
        private string _exportFolder = string.Empty;
        private string _lastPgn = string.Empty;
        private readonly PirateChessLib _pirate = new();
        private readonly System.Windows.Forms.Timer _elapsedTimer = new() { Interval = 1000 };
        private DateTime _startTime;
        public PirateChess()
        {
            InitializeComponent();


            // Read values from INI
            var settings = INIFileHandler.ReadFromINI(Options.filePath, Options.section, Options.key1, Options.key2, Options.key3, Options.key4, Options.key5, Options.key6, Options.key7, Options.key8);
            /* if (!settings.TryGetValue(Options.key1, out string? value1))
             {
                 value1 = "";
             }*/
            if (!settings.TryGetValue(Options.key2, out string? value2))
            {
                value2 = "";
            }
            if (!settings.TryGetValue(Options.key3, out string? value3))
            {
                value3 = "";
            }
            if (!settings.TryGetValue(Options.key4, out string? value4))
            {
                value4 = "";
            }
            if (!settings.TryGetValue(Options.key5, out string? value5))
            {
                value5 = "";
            }
            if (!settings.TryGetValue(Options.key6, out string? value6))
            {
                value6 = "";
            }
            if (!settings.TryGetValue(Options.key7, out string? value7))
            {
                value7 = "";
            }
            if (!settings.TryGetValue(Options.key8, out string? value8))
            {
                value8 = "";
            }

            if (value2 == "1")
            {
                radioButtonBearer.Checked = true;
            }
            else
            {
                radioButtonLogin.Checked = true;
            }
            textBoxBearer.Text = value3;
            textBoxEmail.Text = value4;
            textBoxPwd.Text = value5;
            _exportFolder = value6;
            radioButtonAllKeyMoves.Checked = value7 == "1";
            radioButtonNoTrainingMove.Checked = value7 == "2";
            radioButtonFirstKeyMove.Checked = value7 != "1" && value7 != "2";
            checkBoxAddMoveEmptyChapters.Checked = value8 == "1";

            setEditVisibility();

            _pirate.SetChapterCounterEvent(SetChapterCounter);
            _pirate.SetLineCounterEvent(SetLineCounter);
            _pirate.SetCumulativeLinesEvent(SetCumulativeLinesCounter);
            _pirate.SetRetryEvent(AppendLog);
            _elapsedTimer.Tick += (s, e) =>
            {
                var elapsed = DateTime.Now - _startTime;
                labelElapsed.Text = $"Elapsed: {(int)elapsed.TotalHours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}";
            };
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Write values to INI
            INIFileHandler.WriteToINI(Options.filePath, Options.section, Options.key1, "",
                Options.key2, radioButtonBearer.Checked ? "1" : "", Options.key3, textBoxBearer.Text, Options.key4, textBoxEmail.Text, Options.key5, textBoxPwd.Text,
                Options.key6, _exportFolder, Options.key7, radioButtonAllKeyMoves.Checked ? "1" : radioButtonNoTrainingMove.Checked ? "2" : "",
                Options.key8, checkBoxAddMoveEmptyChapters.Checked ? "1" : "");

            base.OnFormClosed(e);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int count = checkedListBoxChapters.CheckedItems.Count;
            var confirm = MessageBox.Show(
                $"Export {count} course{(count == 1 ? "" : "s")}?",
                "Confirm export",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            using var folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Select export folder";
            if (!string.IsNullOrEmpty(_exportFolder) && Directory.Exists(_exportFolder))
                folderDialog.InitialDirectory = _exportFolder;
            if (folderDialog.ShowDialog() != DialogResult.OK)
                return;

            _exportFolder = folderDialog.SelectedPath;
            _startTime = DateTime.Now;
            labelElapsed.Text = "Elapsed: 00:00:00";
            _elapsedTimer.Start();
            LoadLines(autoExport: true, exportFolder: _exportFolder);
        }

        private void LoadLines(bool useLocalData = false, int maxLines = 10000, bool autoExport = false, string exportFolder = "")
        {
            SetButtonsEnabledState(false);
            var selectedBids = checkedListBoxChapters.CheckedItems
                .Cast<KeyValuePair<string, string>>()
                .Select(kv => kv.Key)
                .ToList();
            textBoxPGN.Text = "";
            textBoxCurLines.Text = "0";
            textBoxCumulativeLines.Text = "0";

            bool allKeyMoves = radioButtonAllKeyMoves.Checked;
            bool noTrainingMove = radioButtonNoTrainingMove.Checked;
            bool addMoveToEmptyChapters = checkBoxAddMoveEmptyChapters.Checked;
            new Thread(() =>
            {
                _pirate.AllKeyMovesTraining = allKeyMoves;
                _pirate.NoTrainingMove = noTrainingMove;
                _pirate.AddMoveToEmptyChapters = addMoveToEmptyChapters;
                var allPgn = new StringBuilder();
                if (useLocalData)
                {
                    (string? pgn, _coursename) = _pirate.GetCourse("", maxLines, useLocalData: true);
                    allPgn.Append(pgn);
                }
                else
                {
                    for (int courseIdx = 0; courseIdx < selectedBids.Count; courseIdx++)
                    {
                        var bid = selectedBids[courseIdx];
                        Invoke(new Action(() =>
                        {
                            textBoxCourse.Text = $"{courseIdx + 1}/{selectedBids.Count}";
                            for (int i = 0; i < checkedListBoxChapters.Items.Count; i++)
                            {
                                if (((KeyValuePair<string, string>)checkedListBoxChapters.Items[i]).Key == bid)
                                {
                                    checkedListBoxChapters.SelectedIndex = i;
                                    break;
                                }
                            }
                        }));

                        (string? pgn, _coursename) = _pirate.GetCourse(bid, maxLines);
                        allPgn.Append(pgn);

                        if (autoExport && !string.IsNullOrEmpty(_coursename))
                        {
                            string safeName = string.Concat(_coursename.Split(Path.GetInvalidFileNameChars()));
                            string pgnDir = Path.Combine(exportFolder, "pgn");
                            string rawDir = Path.Combine(exportFolder, "rawresponses");
                            Directory.CreateDirectory(pgnDir);
                            Directory.CreateDirectory(rawDir);
                            File.WriteAllText(Path.Combine(pgnDir, safeName + ".pgn"), pgn ?? "");
                            File.WriteAllText(Path.Combine(rawDir, safeName + ".restResponse"),
                                JsonSerializer.Serialize(_pirate.restResponseCourse));
                        }
                    }
                }

                _lastPgn = allPgn.ToString();
                int lineCount = _lastPgn.Count(c => c == '\n');
                Invoke(new Action(() =>
                {
                    textBoxPGN.Text = $"[PGN generated: {lineCount} lines — use Save PGN to export]";
                    SetButtonsEnabledState(true);
                    _elapsedTimer.Stop();
                }));

                MessageBox.Show("Finished.", "Finished", MessageBoxButtons.OK, MessageBoxIcon.None,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }).Start();
        }

        private void ButtonFirstTenLines_Click_1(object sender, EventArgs e)
        {
            LoadLines(maxLines: 10);
        }

        public void SetChapterCounter(string chapterCounter)
        {
            Invoke(new Action(() =>
            {
                textBoxDurchlauf.Text = chapterCounter;

            }));
        }

        public void SetLineCounter(string lineCounter)
        {
            Invoke(new Action(() =>
            {
                textBoxCurLines.Text = lineCounter;
            }));
        }

        public void SetCumulativeLinesCounter(string cumLines)
        {
            Invoke(new Action(() =>
            {
                textBoxCumulativeLines.Text = cumLines;
            }));
        }

        public void AppendLog(string message)
        {
            Invoke(new Action(() =>
            {
                textBoxLog.AppendText(message + Environment.NewLine);
            }));
        }

        private void SetButtonsEnabledState(bool state)
        {
            buttonLogin.Enabled = state;
            buttonLoadChapters.Enabled = state;
            buttonFirstTenLines.Enabled = state;
            buttonParseAll.Enabled = state;
            buttonSavePNG.Enabled = state;
            buttonSaveRestResponse.Enabled = state;
            buttonLoadRestResponse.Enabled = state;
        }

        private void ButtonSavePNG_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new();
            saveFileDialog.InitialDirectory = Path.GetFullPath("pgn");

            string invalidChars = new(Path.GetInvalidFileNameChars());
            string sanitizedFilename = string.Concat(_coursename.Split(invalidChars.ToCharArray()));

            saveFileDialog.FileName = $"{sanitizedFilename}.pgn";  // Adjust this default filename as needed
            saveFileDialog.Filter = "PGN files (*.pgn)|*.pgn|All files (*.*)|*.*"; // File type filter

            // Show the save file dialog and check if the user selected a file
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the file path chosen by the user
                string filePath = saveFileDialog.FileName;

                try
                {
                    // Write the last generated PGN to the selected file
                    File.WriteAllText(filePath, _lastPgn);
                    MessageBox.Show("File saved successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while saving the file: " + ex.Message);
                }
            }
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            string result;

            if (radioButtonBearer.Checked)
            {
                result = _pirate.LoginWithBearer(textBoxBearer.Text);
            }
            else
            {
                result = _pirate.Login(textBoxEmail.Text, textBoxPwd.Text);
            }

            if (result != "")
            {
                MessageBox.Show(result);
            }
            else
            {
                buttonLoadChapters.Enabled = true;
            }
        }

        private void PirateChess_Load(object sender, EventArgs e)
        {

        }

        private void setEditVisibility()
        {
            if (radioButtonBearer.Checked)
            {
                labelBearer.Visible = true;
                textBoxBearer.Visible = true;
                labelEmail.Visible = false;
                textBoxEmail.Visible = false;
                labelPwd.Visible = false;
                textBoxPwd.Visible = false;
            }
            else
            {
                labelBearer.Visible = false;
                textBoxBearer.Visible = false;
                labelEmail.Visible = true;
                textBoxEmail.Visible = true;
                labelPwd.Visible = true;
                textBoxPwd.Visible = true;
            }

        }
        private void RadioButtonBearer_CheckedChanged(object sender, EventArgs e)
        {
            setEditVisibility();
        }

        private void ButtonLoadChapters_Click(object sender, EventArgs e)
        {
            var chapters = _pirate.GetChapters();

            if (chapters.Count == 0)
            {

                MessageBox.Show("Error", "No chapters found. Most likely login wrong.", MessageBoxButtons.OK, MessageBoxIcon.None,
         MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

            var tmp = new List<(string a, string b)>();
            foreach (var chapter in chapters)
            {
                tmp.Add((chapter.Key, chapter.Value));
            }

            checkedListBoxChapters.DataSource = new BindingSource(chapters, "");
            checkedListBoxChapters.DisplayMember = "Value";
            checkedListBoxChapters.ValueMember = "Key";
        }

        private void CheckedListBoxChapters_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int checkedAfter = checkedListBoxChapters.CheckedItems.Count
                + (e.NewValue == CheckState.Checked ? 1 : -1);
            buttonFirstTenLines.Enabled = checkedAfter > 0;
            buttonParseAll.Enabled = checkedAfter > 0;
            buttonSelectAll.Text = checkedAfter == checkedListBoxChapters.Items.Count ? "Select None" : "Select All";
        }

        private void ButtonSelectAll_Click(object sender, EventArgs e)
        {
            bool selectAll = buttonSelectAll.Text == "Select All";
            for (int i = 0; i < checkedListBoxChapters.Items.Count; i++)
                checkedListBoxChapters.SetItemChecked(i, selectAll);
            buttonSelectAll.Text = selectAll ? "Select None" : "Select All";
            buttonFirstTenLines.Enabled = selectAll;
            buttonParseAll.Enabled = selectAll;
        }

        private void buttonSaveRestResponse_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new();
            saveFileDialog.InitialDirectory = Path.GetFullPath("rawresponses");

            string invalidChars = new(Path.GetInvalidFileNameChars());
            string sanitizedFilename = string.Concat(_coursename.Split(invalidChars.ToCharArray()));

            saveFileDialog.FileName = $"{sanitizedFilename}.restResponse";  // Adjust this default filename as needed
            saveFileDialog.Filter = "RRF files (*.restResponse)|*.restResponse|All files (*.*)|*.*"; // File type filter

            // Show the save file dialog and check if the user selected a file
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the file path chosen by the user
                string filePath = saveFileDialog.FileName;

                try
                {
                    // Write the content of the TextBox to the selected file
                    File.WriteAllText(filePath, JsonSerializer.Serialize(_pirate.restResponseCourse));
                    MessageBox.Show("File saved successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while saving the file: " + ex.Message);
                }
            }
        }

        private void buttonLoadRestResponse_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.InitialDirectory = Path.GetFullPath("rawresponses");
            openFileDialog.Filter = "RRF files (*.restResponse)|*.restResponse|All files (*.*)|*.*";
            openFileDialog.Title = "Load Rest Response";

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string json = File.ReadAllText(openFileDialog.FileName);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var course = JsonSerializer.Deserialize<RestResponseCourse>(json, options);

            if (course == null)
            {
                MessageBox.Show("Deserialization failed: file did not contain a valid RestResponseCourse.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Assign deserialized course to the library instance
            _pirate.restResponseCourse = course;

            LoadLines(useLocalData: true);
        } 
    }
}
