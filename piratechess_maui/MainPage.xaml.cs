using piratechess_lib;

namespace piratechess_maui
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {

            var pirate = new PirateChessLib(EntryUid.Text, EntryBearer.Text);
            (var pgn, _) = pirate.GetCourse(EntryBid.Text, 10);

            EditorPgn.Text = pgn;
        }
    }

}
