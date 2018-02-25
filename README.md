# MySync
MySync - Binary file synchronization software with GIT-like PUSH/PULL.
Under AGPL-3.0 license.

## NOT CONTINUED

**Currently at fully-working state, but shouldn't be used for production as it is no more supported. <br>**

---

# Description
Whole project is about to simplify the synchronization of project's content (audio, textures, maps etc.) for both small and big teams.
Designed for *content* synchronization, supports all types of files, syncing code is *not recommended* (you can exclude folder with code and use some github/other hosting for other Version Control software - GIT/Mercurial/SVN etc.).

Client and Server application should run on every platform that supports .NET 4.5 with Mono (tested Windows 7/10 64bit and Debian 8 64bit).

# Features
* Main Feature - GIT-like **push**, **pull**, **discard**,
* Supported binary files,
* Built-in commit compression,
* Fast client-side version control,
* Fast HTTP download/upload,
* Own custom request API based on HTTP-POST,
* Fancy Metro-like UI.

# (No more) Planned features
* Collision detection (this will prevent overriding your changes by commit, this is currently common and requires some attention),
* Users (partially implemented),
* Permisions system,
* Web Dashboard,
* SSL support.

# Tutorials
1. Server and project setup, see wiki: https://github.com/Erdroy/MySync/wiki/

# Contributing
Feel free to contribute, there is a lot of work to do (see https://trello.com/b/NxN9L6Gf/mysync)

---

MySync © 2016-2018 Damian 'Erdroy' Korczowski
