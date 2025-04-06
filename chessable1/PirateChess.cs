using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using piratechess_lib;
using RestSharp;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace piratechess_Winform
{
    public partial class PirateChess : Form, IDisposable
    {
        private string _coursename = string.Empty;
        public PirateChess()
        {
            InitializeComponent();


            // Read values from INI
            var settings = INIFileHandler.ReadFromINI(Options.filePath, Options.section, Options.key1, Options.key2, Options.key3);
            if (!settings.TryGetValue(Options.key1, out string? value1))
            {
                value1 = "";
            }
            if (!settings.TryGetValue(Options.key2, out string? value2))
            {
                value2 = "";
            }
            if (!settings.TryGetValue(Options.key3, out string? value3))
            {
                value3 = "";
            }


            textBoxBearer.Text = value1;
            textBoxUid.Text = value2;
            textBoxBid.Text = value3;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Write values to INI
            INIFileHandler.WriteToINI(Options.filePath, Options.section, Options.key1, textBoxBearer.Text,
                Options.key2, textBoxUid.Text, Options.key3, textBoxBid.Text);

            base.OnFormClosed(e);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBoxBearer.Text.Length < 3000)
            {
                MessageBox.Show("bearer missing");
                return;
            }

            if (textBoxUid.Text.Length == 0)
            {
                MessageBox.Show("uid (userid) missing");
                return;
            }

            if (textBoxBid.Text.Length == 0)
            {
                MessageBox.Show("bid (Courseid) missing");
                return;
            }

            new Thread(() =>
            {
                var pirate = new PirateChessLib(textBoxUid.Text, textBoxBearer.Text);

               (var pgn, _coursename) = pirate.GetCourse(textBoxUid.Text);


                Invoke(new Action(() =>
                {
                    textBoxPGN.Text = pgn;
                }));


                MessageBox.Show("Finished.", "Finished", MessageBoxButtons.OK, MessageBoxIcon.None,
         MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }).Start();
        }

        private void ButtonFirstTenLines_Click_1(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                var pirate = new PirateChessLib(textBoxUid.Text, textBoxBearer.Text);
               var pgn = pirate.GetCourse(textBoxBid.Text, 10);


                Invoke(new Action(() =>
                {
                    textBoxPGN.Text = pgn.ToString();
                }));
            }).Start();
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
    }

}
