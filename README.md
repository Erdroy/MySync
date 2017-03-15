# MySync
MySync - File synchronization software with GIT-like PUSH/PULL.
Under AGPL-3.0 license.

*** UNDER DEVELOPMENT! ***
For current progress, see trello or/and my Discord server, links below in the Contact section.
-----------------

# Short description
Whole project is about to simplify the synchronization of project's content(audio, textures, maps etc.) for both small and big teams.
Designed for Content synchronization, supports all types of files, syncing code is not recommended(you can exclude folder with code and use some github/other hosting for other Version Control software - GIT/Mercurial/SVN etc.).

Client and Server application is designed to work on every platform that supports .NET 4.5 with Mono(Some day, I'll use .NET Core).
However, server is easiest to install on Linux with Mono(And it is recommended, you don't need to worry about IP's etc.).

# Features
* Main Feature - GIT-like push, pull, discard etc.,
* Supported binary files,
* Built-in commit compression - you do not need to wait long minutes waiting for large files to upload,
* Fast client-side version control,
* Fast HTTP download/upload,
* Custom port support,
* Own custom request API based on HTTP-POST(state-less),
* Fancy Metro-like UI(WIP).

# Planned features
* Collision detection(this will prevent overriding your changes by commit, this is currently common and requires some attention),
* Users(partially implemented),
* Permisions system,
* Web Dashboard,
* SSL support.

# Contribution/Contact
Feel free to contribute, there is a lot of work to do(see https://trello.com/b/NxN9L6Gf/mysync), 
also you can join my Discord server and suggest some features, talk with me: https://discordapp.com/invite/0kfQL9KtL4XfOK7G

---

MySync © 2016-2017 Damian 'Erdroy' Korczowski
