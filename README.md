Use at your own risk!
Don't know if it is against any TOS, but be warned if you get banned its not my fault. Use only for personal backups.

Project to extract chessable courses.

As I want to get real ownership of bought stuff (and import the courses into chessbase) I created a small programm to extract courses to pgn. Use Winform if possible - more features there (Login as Bearer, save to file)

Steps to use the program:
* fill in username & Password (Or Bearertoken)
* Click Login
* Click fill chapter
* select the course you want to export
* Click Generate
* Save the pgn or copy it to your prefered Program

No Video export and none planned.

Currently only exporting Moves & Text.

Planned:
* More Errorhandling (especially in the Maui version)
* Export arrows and colours

If you need the Bearertoken use Developertools (Tab Network), Refresh the Dashboard. Look for getUserSettings and copy the value for Authorization (starts with Bearer). Use right cick and copy value, paste to any editor, copy everything after Bearer, this is your bearer token. 

Gui very barebone for now - especially the dropdownbox is barely visible in Maui when empty.
![image](https://github.com/user-attachments/assets/0f7a25a7-ad2a-4143-84b1-d5ba3c9f789c)

