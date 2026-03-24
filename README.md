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

If you need the Bearertoken use Developertools (Tab Network), Refresh the Dashboard. Look for getUserSettings and copy the value for Authorization (starts with Bearer). Use right cick and copy value, paste to any editor, copy everything after Bearer, this is your bearer token. 

Gui very barebone for now - especially the dropdownbox is barely visible in Maui when empty.
![image](https://github.com/user-attachments/assets/0f7a25a7-ad2a-4143-84b1-d5ba3c9f789c)

