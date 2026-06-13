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

As I want to get real ownership of bought stuff (and import the courses into chessbase) I created a small programm to extract courses to pgn. Use Winform if possible - more features there (Login as Bearer, save to file)

Steps to use the program:
* fill in username & Password (Or Bearertoken)
* Click Login
* Click fill chapter
* select the course you want to export (or courses)
* Click Generate
* Save the pgn or copy it to your prefered Program

* you can now save (in the winform-version) the raw Response from the server and load it again - this way you have to hit the server only once. if anything is not working just send me an email or open a issue and attach the rrf (.restResponse) file - that way I can debug the Problem without needing the course itself.
  This is also helpfull if I build a new version as reading the local restResponsenses is MUCH faster (bigger courses take ~1 hour to load due to server limitations as I don't want to pull too fast and even with my delay I am regularly hitting some Server limits).

* If you select multiple courses it automaticly saves rawresponse and pgn

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

The token is read locally from `localStorage['chessable.web.production.JWT']` and only copied to your clipboard — it is never sent anywhere.

Gui very barebone for now - especially the dropdownbox is barely visible in Maui when empty.
![image](https://github.com/user-attachments/assets/0f7a25a7-ad2a-4143-84b1-d5ba3c9f789c)

