using piratechess_lib;

namespace piratechess_maui
{
    public partial class MainPage : ContentPage
    {
        private readonly PirateChessLib _pirate = new();
        private IDispatcherTimer? _elapsedTimer;
        private DateTime _startTime;
        private string _lastPgn = string.Empty;

        public MainPage()
        {
            InitializeComponent();
            _pirate.SetRetryEvent(AppendLog);

            _elapsedTimer = Dispatcher.CreateTimer();
            _elapsedTimer.Interval = TimeSpan.FromSeconds(1);
            _elapsedTimer.Tick += (s, e) =>
            {
                var elapsed = DateTime.Now - _startTime;
                LabelElapsed.Text = $"Elapsed: {(int)elapsed.TotalHours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}";
            };
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
            EntryBearer.Text = Preferences.Get("bearer", "");
            string trainingMode = Preferences.Get("trainingMode", "firstkey");
            RadioAllKeyMoves.IsChecked = trainingMode == "allkeys";
            RadioNoTrainingMove.IsChecked = trainingMode == "notraining";
            RadioFirstKeyMove.IsChecked = trainingMode == "firstkey";
            CheckBoxAddMoveEmptyChapters.IsChecked = Preferences.Get("addMoveToEmpty", false);
            EntryExtraDelayMin.Text = Preferences.Get("extraDelayMinMs", 0).ToString();
            EntryExtraDelayMax.Text = Preferences.Get("extraDelayMaxMs", 0).ToString();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Preferences.Set("bearer", EntryBearer.Text ?? "");
            string trainingMode = RadioAllKeyMoves.IsChecked ? "allkeys"
                : RadioNoTrainingMove.IsChecked ? "notraining"
                : "firstkey";
            Preferences.Set("trainingMode", trainingMode);
            Preferences.Set("addMoveToEmpty", CheckBoxAddMoveEmptyChapters.IsChecked);
            if (TryGetExtraDelay(out int extraDelayMin, out int extraDelayMax))
            {
                Preferences.Set("extraDelayMinMs", extraDelayMin);
                Preferences.Set("extraDelayMaxMs", extraDelayMax);
            }
        }

        /// <summary>
        /// Reads the extra delay from both entry fields. Only whole, non-negative
        /// milliseconds with max >= min are allowed; empty counts as 0.
        /// </summary>
        private bool TryGetExtraDelay(out int min, out int max)
        {
            max = 0;
            string minText = EntryExtraDelayMin.Text?.Trim() ?? "";
            string maxText = EntryExtraDelayMax.Text?.Trim() ?? "";

            if (!int.TryParse(minText.Length == 0 ? "0" : minText, out min))
                return false;
            if (!int.TryParse(maxText.Length == 0 ? "0" : maxText, out max))
                return false;

            return min >= 0 && max >= min;
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
            string result = _pirate.LoginWithBearer(EntryBearer.Text);

            if (result != "")
                await Shell.Current.DisplayAlert("Error", result, "OK");
            else
                await Shell.Current.DisplayAlert("Login ok", "Login ok", "OK");
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

            if (!TryGetExtraDelay(out int extraDelayMin, out int extraDelayMax))
            {
                await Shell.Current.DisplayAlert("Warning",
                    "Extra delay must be a positive number of milliseconds, and max must not be smaller than min.", "OK");
                return;
            }

            var selected = (KeyValuePair<string, string>)myPicker.SelectedItem;
            _pirate.SetChapterCounterEvent(ChapterCounter);
            _pirate.SetLineCounterEvent(LineCounter);

            EditorPgn.Text = "";
            EditorLog.Text = "";
            LabelElapsed.Text = "Elapsed: 00:00:00";
            _startTime = DateTime.Now;
            _elapsedTimer?.Start();
            bool allKeyMoves = RadioAllKeyMoves.IsChecked;
            bool noTrainingMove = RadioNoTrainingMove.IsChecked;
            bool addMoveToEmpty = CheckBoxAddMoveEmptyChapters.IsChecked;

            new Thread(() =>
            {
                try
                {
                    _pirate.AllKeyMovesTraining = allKeyMoves;
                    _pirate.NoTrainingMove = noTrainingMove;
                    _pirate.AddMoveToEmptyChapters = addMoveToEmpty;
                    _pirate.ExtraDelayMinMs = extraDelayMin;
                    _pirate.ExtraDelayMaxMs = extraDelayMax;
                    (var pgn, var coursename) = _pirate.GetCourse(selected.Key, maxLines);
                    _lastPgn = pgn ?? "";
                    string pgnSnapshot = _lastPgn;
                    AppendLog($"{coursename}: {_pirate.ErrorCount} error(s)");
                    AppendLog("All data loaded - rendering PGN ...");

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        EditorPgn.Text = pgnSnapshot;
                        _elapsedTimer?.Stop();
                        AppendLog("Done.");
                    });
                }
                catch (Exception ex)
                {
                    AppendLog($"ERROR: {ex.Message}");
                    MainThread.BeginInvokeOnMainThread(() => _elapsedTimer?.Stop());
                }
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
                LabelLineCounter.Text = obj;
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
                LabelChapterCounter.Text = obj;
            });
        }
    }
}
