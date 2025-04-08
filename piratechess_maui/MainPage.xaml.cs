using piratechess_lib;

namespace piratechess_maui
{
    public partial class MainPage : ContentPage
    {
        private readonly PirateChessLib _pirate = new();
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnButtonFirstTenLinesClicked(object sender, EventArgs e)
        {
            GenerateLines(10);
        }
        private void OnButtonLoginClicked(object sender, EventArgs e)
        {
            _pirate.Login(EntryBearer.Text, EntryUid.Text);
        }
        private void OnButtonLoadChapterClicked(object sender, EventArgs e)
        {
            var items = _pirate.GetChapters();

            myPicker.ItemsSource = items.ToList();
        }

        private void GenerateLines(int maxLines = 10000)
        {
            var selected = (KeyValuePair<string, string>)myPicker.SelectedItem;
            _pirate.SetChapterCounterEvent(ChapterCounter);
            _pirate.SetLineCounterEvent(LineCounter);

            new Thread(() =>
            {
                (var pgn, _) = _pirate.GetCourse(selected.Key, maxLines);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    EditorPgn.Text = pgn;
                });
            }).Start();
        }

        private void OnButtonGenerateCourseClicked(object sender, EventArgs e)
        {
            GenerateLines();
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
