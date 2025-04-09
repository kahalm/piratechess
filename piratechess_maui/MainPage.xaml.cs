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
            GenerateLinesAsync(10);
        }
        private void OnButtonLoginClicked(object sender, EventArgs e)
        {
            Login();
        }

        private async void Login()
        {
           var result = _pirate.Login(EntryBearer.Text, EntryUid.Text);

            if (result != "")
            {
                await Shell.Current.DisplayAlert("Error", result, "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Login ok", "Login ok", "OK");
            }
        }
        private void OnButtonLoadChapterClicked(object sender, EventArgs e)
        {
            LoadChaptersAsync();
        }

        private async void LoadChaptersAsync()
        {
            var items = _pirate.GetChapters();

            if (items.Count == 0)
            {

                await Shell.Current.DisplayAlert("Warning", "No chapters found", "OK");
            }
            myPicker.ItemsSource = items.ToList();
            myPicker.SelectedIndex = 0;
        }

        private async void GenerateLinesAsync(int maxLines = 10000)
        {
            if (myPicker.SelectedIndex == -1)
            {
                await Shell.Current.DisplayAlert("Warning", "Please Select a chapter", "OK");
                return;
            }

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
            GenerateLinesAsync();
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
