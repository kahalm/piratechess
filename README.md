Use at your own risk!
Don't know if it is against any TOS, but be warned if you get banned its not my fault. Use only for personal backups.

Project to extract chessable courses.

As I want to get real ownership of bought stuff (and import the courses into chessbase) I created a small programm to extract courses to pgn.

Steps to use the program:
* log into chessable, open any course and use f12 (Developer Tools) -> Network to capture traffic
* refresh
* search for uid and choose any of the requests (e.g. getNotifications).
* copy uid & bearer token to the programm (bearer is under Request Headers -> Authorization, everything after bearer)
* open course and copy the coursenumber from the url to bid

  Klick "generiere kompletten Kurs" and wait - some slowness is wanted some just happened, don't want to optimize.
  Wait til everything finished, copy content of the Textbox to a Textfile named something.png, import into chessbase & enjoy

Now also tested with positions/taktiks.
No Video export and none planned.
