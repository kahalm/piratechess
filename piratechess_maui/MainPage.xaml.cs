using piratechess_lib;

namespace piratechess_maui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnButtonFirstTenLinesClicked(object sender, EventArgs e)
        {

            var pirate = new PirateChessLib(EntryUid.Text, EntryBearer.Text);
            (var pgn, _) = pirate.GetCourse(EntryBid.Text, 10);

            EditorPgn.Text = pgn;
        }

        private void OnButtonGenerateCourseClicked(object sender, EventArgs e)
        {

            var pirate = new PirateChessLib(EntryUid.Text, EntryBearer.Text);
            pirate.SetChapterCounterEvent(ChapterCounter);
            pirate.SetLineCounterEvent(LineCounter);



            new Thread(() =>
            {
                (var pgn, _) = pirate.GetCourse(EntryBid.Text);

                EditorPgn.Text = pgn;
            }).Start();
        }

        private void LineCounter(string obj)
        {
            Task task = LineCounterAsync(obj);
            task.Wait();
        }

        private async Task LineCounterAsync(string obj)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                LabelChapterCounter.Text = obj;
            });
        }

        private void ChapterCounter(string obj)
        {
            Task task = ChapterCounterAsync(obj);
            task.Wait();
        }

        private async Task ChapterCounterAsync(string obj)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                LabelLineCounter.Text = obj;
            });
        }
    }
}
