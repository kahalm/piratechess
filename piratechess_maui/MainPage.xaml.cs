using piratechess_lib;

namespace piratechess_maui
{
    public partial class MainPage : ContentPage
    {
        private readonly PirateChessLib _pirate = new();
        public MainPage()
        {
            InitializeComponent();
            _pirate.SetRetryEvent(AppendLog);
        }

        private void AppendLog(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                EditorLog.Text += message + Environment.NewLine;
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SwitchUseBearer.IsToggled = Preferences.Get("useBearer", false);
            EntryBearer.Text = Preferences.Get("bearer", "");
            EntryEmail.Text = Preferences.Get("email", "");
            EntryPassword.Text = Preferences.Get("password", "");
            string trainingMode = Preferences.Get("trainingMode", "firstkey");
            RadioAllKeyMoves.IsChecked = trainingMode == "allkeys";
            RadioNoTrainingMove.IsChecked = trainingMode == "notraining";
            RadioFirstKeyMove.IsChecked = trainingMode == "firstkey";
            CheckBoxAddMoveEmptyChapters.IsChecked = Preferences.Get("addMoveToEmpty", false);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Preferences.Set("useBearer", SwitchUseBearer.IsToggled);
            Preferences.Set("bearer", EntryBearer.Text ?? "");
            Preferences.Set("email", EntryEmail.Text ?? "");
            Preferences.Set("password", EntryPassword.Text ?? "");
            string trainingMode = RadioAllKeyMoves.IsChecked ? "allkeys"
                : RadioNoTrainingMove.IsChecked ? "notraining"
                : "firstkey";
            Preferences.Set("trainingMode", trainingMode);
            Preferences.Set("addMoveToEmpty", CheckBoxAddMoveEmptyChapters.IsChecked);
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
            string result;
            if (SwitchUseBearer.IsToggled)
                result = _pirate.LoginWithBearer(EntryBearer.Text);
            else
                result = _pirate.Login(EntryEmail.Text, EntryPassword.Text);

            if (result != "")
                await Shell.Current.DisplayAlert("Error", result, "OK");
            else
                await Shell.Current.DisplayAlert("Login ok", "Login ok", "OK");
        }

        private void OnSwitchUseBearerToggled(object sender, ToggledEventArgs e)
        {
            EntryBearer.IsVisible = e.Value;
            EntryEmail.IsVisible = !e.Value;
            EntryPassword.IsVisible = !e.Value;
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

            EditorLog.Text = "";
            bool allKeyMoves = RadioAllKeyMoves.IsChecked;
            bool noTrainingMove = RadioNoTrainingMove.IsChecked;
            bool addMoveToEmpty = CheckBoxAddMoveEmptyChapters.IsChecked;

            new Thread(() =>
            {
                _pirate.AllKeyMovesTraining = allKeyMoves;
                _pirate.NoTrainingMove = noTrainingMove;
                _pirate.AddMoveToEmptyChapters = addMoveToEmpty;
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
