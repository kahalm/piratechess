> # ⛔ This project is discontinued
>
> **PirateChess (the standalone Winform/MAUI/CLI app) is no longer maintained.**
>
> 👉 **Use [RookHub](https://rookhub.oberschmid.homes) instead** — the chess training
> platform that now carries this functionality forward (Chessable course export and much more).
>
> This repository is archived and kept read-only for reference only. No further updates,
> releases or bugfixes will be made here.

---

Use at your own risk!
Don't know if it is against any TOS, but be warned if you get banned its not my fault. Use only for personal backups.

Project to extract chessable courses.

As I want to get real ownership of bought stuff (and import the courses into chessbase) I created a small programm to extract courses to pgn. Use Winform if possible - more features there (batch export, save to file).

Login works with a JWT bearer token only. The email/password login was removed because Chessable blocks the API login behind Cloudflare.

## Which file do I download?

Every release ships each Windows build twice. Same program, different packaging:

| File | Size | Requirement |
|------|------|-------------|
| `piratechess_winform.exe` | ~110 MB | none, the .NET runtime is inside |
| `piratechess_winform-win-x86.exe` | ~102 MB | none, 32-bit machines |
| `piratechess_winform-needs-dotnet9.exe` | ~3 MB | [.NET 9 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/9.0) installed |
| `piratechess_winform-win-x86-needs-dotnet9.exe` | ~3 MB | same, 32-bit machines |
| `piratechess_maui-win-x64.exe` | ~240 MB | none |
| `piratechess_maui-win-x64-needs-dotnet9.exe` | ~99 MB | .NET 9 Desktop Runtime installed |

If in doubt take `piratechess_winform.exe`. It is the big one, but it just runs. The
`-needs-dotnet9` files are for machines that already have the runtime and where the
download size matters. Releases 0.30 and older only had the self-contained variant;
0.31 accidentally only had the framework-dependent one, which is why its EXE was 3 MB
and refused to start without the runtime.

Steps to use the program:
* paste your bearer token (see below)
* Click Login
* Click fill chapter
* select the course you want to export (or courses)
* Click Generate
* Save the pgn or copy it to your prefered Program

* you can now save (in the winform-version) the raw Response from the server and load it again - this way you have to hit the server only once. if anything is not working just send me an email or open a issue and attach the rrf (.restResponse) file - that way I can debug the Problem without needing the course itself.
  This is also helpfull if I build a new version as reading the local restResponsenses is MUCH faster (bigger courses take ~1 hour to load due to server limitations as I don't want to pull too fast and even with my delay I am regularly hitting some Server limits).

* If you select multiple courses it automaticly saves rawresponse and pgn. If lines had to be skipped because Chessable sent broken data, an `.errors.txt` with the details lands next to the rawresponse. Please attach both to a bug report.

* **Extra delay**: between two server calls the program already waits a random 500-1500 ms. In the Settings box (Winform) resp. the "Extra delay between calls (ms)" fields (Maui) you can add your own extra wait on top of that, as a random value between your min and max. Both values are milliseconds and must be positive; 0/0 means only the built-in delay. Raise it if you still hit server limits, but keep in mind that every step adds up over a whole course.

No Video export and none planned.

Currently only exporting Moves & Text.

Planned:
* More Errorhandling (especially in the Maui version)
* For Puzzles add Trainingscomment
* Klickable Variants
* Export arrows and colours - Done

## Get Bearer Token

If you need the Bearertoken use Developertools (Tab Network), Refresh the Dashboard. Look for getUserSettings and copy the value for Authorization (starts with Bearer). Use right cick and copy value, paste to any editor, copy everything after Bearer, this is your bearer token.

A valid token has the JWT format `header.payload.signature` (three Base64-URL blocks separated by dots) and is valid for 7 days.

### Easier: the RepCheck browser extension

Instead of digging through the DevTools network tab, you can grab the token with the **RepCheck** browser extension / userscript ([github.com/kahalm/repcheck](https://github.com/kahalm/repcheck)). RepCheck is primarily an opening-repertoire deviation checker for chess.com and lichess (it pairs with the [RookHub](https://rookhub.oberschmid.homes) chess platform), but since v1.8.0 it can also read your Chessable bearer token for use here:

* **Extension**: while logged in on chessable.com, open the RepCheck toolbar popup → "Chessable-Token" → **"Token kopieren"**, then paste the token into PirateChess.
* **Userscript**: on chessable.com open the Tampermonkey menu → **"🔑 Chessable-Token kopieren"**.

The token is read locally from `localStorage['chessable.web.production.JWT']` and only copied to your clipboard — it is never sent anywhere. Saving in RepCheck needs a [RookHub](https://rookhub.oberschmid.homes) account.

PirateChess points at both in its bearer-token error messages.

Gui very barebone for now - especially the dropdownbox is barely visible in Maui when empty.
![image](https://github.com/user-attachments/assets/0f7a25a7-ad2a-4143-84b1-d5ba3c9f789c)

