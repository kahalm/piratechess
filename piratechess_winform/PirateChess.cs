using piratechess_lib;
using System.Text.Json;

namespace piratechess_Winform
{
    public partial class PirateChess : Form, IDisposable
    {
        private string _coursename = string.Empty;
        private readonly PirateChessLib _pirate = new();
        public PirateChess()
        {
            InitializeComponent();


            // Read values from INI
            var settings = INIFileHandler.ReadFromINI(Options.filePath, Options.section, Options.key1, Options.key2, Options.key3, Options.key4, Options.key5);
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

            setEditVisibility();

            _pirate.SetChapterCounterEvent(SetChapterCounter);
            _pirate.SetLineCounterEvent(SetLineCounter);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Write values to INI
            INIFileHandler.WriteToINI(Options.filePath, Options.section, Options.key1, "",
                Options.key2, radioButtonBearer.Checked ? "1" : "", Options.key3, textBoxBearer.Text, Options.key4, textBoxEmail.Text, Options.key5, textBoxPwd.Text);

            base.OnFormClosed(e);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            LoadLines();
        }

        private void LoadLines(int maxLines = 10000)
        {
            SetButtonsEnabledState(false);
            var bid = (string?)comboBoxChapters.SelectedValue ?? "";
            textBoxPGN.Text = "";
            textBoxCumulativeLines.Text = "0";

            new Thread(() =>
            {
                (string? pgn, _coursename) = _pirate.GetCourse(bid, maxLines);

                Invoke(new Action(() =>
                {
                    textBoxPGN.Text = pgn;
                    SetButtonsEnabledState(true);
                }));

                MessageBox.Show("Finished.", "Finished", MessageBoxButtons.OK, MessageBoxIcon.None,
         MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }).Start();

        }

        private void ButtonFirstTenLines_Click_1(object sender, EventArgs e)
        {
            LoadLines(10);
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
                textBoxCumulativeLines.Text = lineCounter;

            }));
        }

        private void SetButtonsEnabledState(bool state)
        {
            buttonLogin.Enabled = state;
            buttonLoadChapters.Enabled = state;
            buttonFirstTenLines.Enabled = state;
            buttonParseAll.Enabled = state;
            buttonSavePNG.Enabled = state;
        }

        private void ButtonSavePNG_Click(object sender, EventArgs e)
        {
            // Create a SaveFileDialog to allow the user to choose the save location
            using SaveFileDialog saveFileDialog = new();
            // Set the default file name

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
                    // Write the content of the TextBox to the selected file
                    File.WriteAllText(filePath, textBoxPGN.Text);
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
                result = _pirate.LoginWithBearer(textBoxEmail.Text);
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

            comboBoxChapters.DataSource = new BindingSource(chapters, "");
            comboBoxChapters.DisplayMember = "Value";
            comboBoxChapters.ValueMember = "Key";
        }

        private void ComboBoxChapters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxChapters.SelectedValue is not null and not (object)"")
            {
                buttonFirstTenLines.Enabled = true;
                buttonParseAll.Enabled = true;
            }
            else
            {
                buttonFirstTenLines.Enabled = false;
                buttonParseAll.Enabled = false;
            }
        }

        private void buttonSaveRestResponse_Click(object sender, EventArgs e)
        {
            // Create a SaveFileDialog to allow the user to choose the save location
            using SaveFileDialog saveFileDialog = new();
            // Set the default file name

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

        } 
    }
}
