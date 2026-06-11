# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Project Does

PirateChess extracts Chessable chess courses and converts them to PGN files for ChessBase import. It calls the Chessable API, parses the JSON responses, and generates annotated PGN with ChessBase-specific training annotations.

## Solution Structure

```
piratechess.sln         — includes lib, winform, maui (NOT the CLI)
piratechess/            — CLI entry point (net9.0, NOT in .sln, standalone csproj)
piratechess_lib/        — shared core logic (net8.0)
piratechess_winform/    — WinForms GUI (net9.0-windows7.0)
piratechess_maui/       — MAUI cross-platform app (net9.0)
rawresponses/           — cached API responses (*.restResponse, JSON)
pgn/                    — generated PGN output files
```

## Build Commands

```bash
# Build individual projects
dotnet build ./piratechess_winform/piratechess_winform.csproj
dotnet build ./piratechess_maui/piratechess_maui.csproj

# Run CLI (processes all *.restResponse in rawresponses/ or explicit file args)
dotnet run --project piratechess/piratechess.csproj
dotnet run --project piratechess/piratechess.csproj path/to/course.restResponse

# Publish WinForms (win-x64 single-file)
dotnet publish piratechess_winform/piratechess_winform.csproj -c Release -o ./publish/win-x64 --self-contained false -r win-x64 /p:PublishSingleFile=true /p:DebugType=none

# Publish MAUI Android
dotnet publish ./piratechess_maui/piratechess_maui.csproj -f net9.0-android -o ./publish/maui/android --self-contained false

# Build MAUI iOS (simulator only — ios-arm64 requires signing certificate)
dotnet build ./piratechess_maui/piratechess_maui.csproj -f net9.0-ios -c Debug -r iossimulator-x64

# Build MAUI macOS (code signing must be disabled)
dotnet build ./piratechess_maui/piratechess_maui.csproj -f net9.0-maccatalyst -p:EnableCodeSigning=false
```

There are no test projects.

## Core Architecture

### Data Flow

```
Chessable API  →  PirateChessLib.GetCourse()  →  Game.GeneratePGN()  →  *.pgn
     OR
*.restResponse →  CLI Program.cs (useLocalData: true)  →  *.pgn
```

### Key Files in `piratechess_lib/`

**`Models.cs`** — All data models and PGN generation:
- `RestResponseCourse` / `RestResponseChapter` / `RestResponseLine` — serialization format for `.restResponse` cache files
- `ResponseCourse`, `ResponseChapter`, `ResponseLine`, `Game`, `JsonMove` — Chessable API JSON models
- `Game.GeneratePGN()` — converts `JsonMove` list to PGN string including:
  - ChessBase arrow/circle annotations: `[%cal ...]` / `[%csl ...]`
  - ChessBase training quiz at first key move: `{[%tqu "En","find the move","","","<uci>","",10]}`
  - Nested variation lines `(...)`
- `Game.GetFirstKeyMoveUci()` — uses `ChessDotNet` to convert SAN of first `IsKey` move to UCI (e.g. `e2e4`)
- `JsonMoveItemList.GetVariationPgn()` — recursively builds PGN variation strings

**`Options.cs`** — `Options.GetOptions()` (shared `JsonSerializerOptions` with `PropertyNameCaseInsensitive`) and `PgnInfo` (PGN header struct). Note: there is a separate `piratechess_Winform.Options` class (INI key constants) — these are unrelated despite the same name.

**`JwtHelper.cs`** — `ExtractUidFromToken(jwt)` — decodes JWT payload (Base64URL) and extracts `user.uid` field. Used by both `LoginWithBearer()` and `ExtractUid()`.

**`PirateChessLib.cs`** — API orchestration:
- `Login(email, password)` — SHA-512 hashes password, POSTs to Chessable API
- `LoginWithBearer(jwt)` — sets bearer + extracts UID from JWT
- `GetCourse(bid, lines, useLocalData)` → `GetChapter()` → `GetLine()` — full pipeline
- Sleeps 500–1500ms randomly between API calls; `GetLine()` retries up to 10× with 30s sleep on empty response
- `restResponseCourse` property — the full course cache
- Progress callbacks: `SetChapterCounterEvent`, `SetLineCounterEvent`, `SetCumulativeLinesEvent`, `SetRetryEvent`

### Training Modes

`PirateChessLib` has three mutually-exclusive training annotation modes, selectable in all UIs:

| Mode | `AllKeyMovesTraining` | `NoTrainingMove` | Behavior |
|------|-----------------------|------------------|----------|
| `firstkey` (default) | false | false | `[%tqu]` only on the first `IsKey` move |
| `allkeys` | true | false | `[%tqu]` on every `IsKey` move matching the color of the first key move |
| `notraining` | false | true | No `[%tqu]` annotations at all |

The CLI generates **all three variants** per `.restResponse` and saves them as `<CourseName>_firstkey.pgn`, `<CourseName>_allkeys.pgn`, `<CourseName>_notraining.pgn`.

### PGN Format Details

- Round header format: `{chapterCounter+1:000}.{lineCount+1:000}` where both counters are already 1-based — effectively `arrayIndex+2` (e.g. ChapterList[0] line 0 → `"002.002"`)
- `JsonMove.IsKey = true` marks the first move the user must find (puzzle start)
- UCI for training annotations is computed by `SanToMove()` which replays moves on a `ChessGame` instance using `GetValidMoves()` and SAN matching — **not** via `PgnReader`
- Chessable HTML annotation tokens stripped by `ReplaceCommentStuff()`: `@@StartBracket@@`/`@@EndBracket@@` → `()`; `@@StartFEN@@...@@EndFEN@@` → removed; `@@StartBlockQuote@@`, `@@LinkStart@@`, `@@SANStart@@`, `@@HeaderStart@@` → removed (along with their `@@End...@@` counterparts); HTML tags via regex

### Configuration

**WinForms** (`settings.ini`, read/written by `piratechess_Winform.INIFileHandler`):
```ini
[Settings]
useBearer=1                ; "1" = JWT bearer, "" = email/password
bearer=<jwt>
email=<email>
pwd=<password>
exportFolder=<path>
allKeyMovesTraining=       ; "" = firstkey, "1" = allkeys, "2" = notraining
addMoveToEmptyChapters=1   ; "1" = replace empty/null-move-only lines with "1. e4"
```

**MAUI** uses `Preferences.Get/Set()` (platform-native key-value storage) with keys: `useBearer`, `bearer`, `email`, `password`, `trainingMode` (`"firstkey"` / `"allkeys"` / `"notraining"`), `addMoveToEmpty`.

**CLI** reads only `exportFolder` and `addMoveToEmptyChapters` from `settings.ini` (located next to the executable).

### WinForms-Specific Files (`piratechess_winform/`)

- `PirateChess.cs` — main form; runs `GetCourse()` on a background `Thread`, uses `Invoke()` for UI updates
- `Ini.cs` — `INIFileHandler` class for reading/writing `settings.ini`
- `Options.cs` — INI key constants (`key1`..`key8`), not the lib `Options` class
- `Testdata.cs` — test/sample data
- WinForm auto-export: when batch-exporting multiple courses, saves both `.pgn` and `.restResponse` files to the selected export folder

### MAUI-Specific (`piratechess_maui/`)

- `MainPage.xaml.cs` — main UI; runs `GetCourse()` on a background `Thread`, uses `MainThread.BeginInvokeOnMainThread()` / `InvokeOnMainThreadAsync()` for UI updates
- Single-course export only (no batch like WinForm)

### Threading Pattern

All UIs run `GetCourse()` on a separate `Thread` (not `Task`). UI callbacks (`SetChapterCounterEvent`, `SetLineCounterEvent`, etc.) are invoked from the worker thread and must marshal to the UI thread:
- WinForms: `Invoke(new Action(...))`
- MAUI: `MainThread.BeginInvokeOnMainThread(...)` or `MainThread.InvokeOnMainThreadAsync(...)`

## GitHub Actions

Workflows trigger on `release: published` and `workflow_dispatch`. Key constraints:

| Target | Runner | Notes |
|--------|--------|-------|
| WinForms win-x64/win-x86 | `windows-latest` | Single-file EXE |
| MAUI Android (.apk) | `windows-latest` | Version from git tag |
| MAUI iOS | `macos-26` | `iossimulator-x64 -c Debug` only (no cert) |
| MAUI macOS | `macos-26` | `-p:EnableCodeSigning=false` required |
| MAUI Windows | `windows-latest` | No `-r win-x64` flag (not needed) |

`macos-26` runner is required for Apple's new versioning (Xcode 26 compatibility).

## NuGet Dependencies

- `ChessDotNet 1.0.0` — SAN-to-UCI conversion via `PgnReader<ChessGame>` (in `piratechess_lib`)
- `RestSharp 112.1.0` — Chessable API HTTP calls
- `Newtonsoft.Json 13.0.3` — JSON deserialization (winform)
