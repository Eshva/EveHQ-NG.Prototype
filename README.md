# EveHQ-NG proof of concepts application #
It is a EveHQ-NG proof of concepts application. EveHQ-NG stands for EveHQ Next Generation. EveHQ itself is a well known support application for Eve Online game. I am the current maintainer of EveHQ. And it will die on May 2018. Why? Because CCP, the company behind the Eve Online will depricate some web-services on May 2018 that EveHQ uses and support other that not used by EveHQ.

Why don't I update the present version of EveHQ to use the new web-services? Because the change is huge and the source code of EveHQ is shit. It easer to rewrite the application from scratch than update its code. Spagetti-code written in VB.NET using WinForms RAD-approach where UI-code mixed with web-related code in files tens thousand lines files... Programmers will understand me.

Secondly this application designed to work only in Windows. There are requests to make it multi platform. With WinForms and classic .NET Framework it isn't possible.

The third reason is I reenvisioned this tool. The legacy EveHQ is a banch of not connected or loosely connected tools. I want to make it more 'EVE professions' centric tool. For instance when the player plays the role of a manufacturer he doesn't need a map or a ship fitter but needs a way to control production lines. And controversal if he is in a solo PvP session he doesn't need the manufacturer tools but needs the galaxy map and some intel about pilots around.

## Prototype targets ##
So what is this prototype for?

Before I will start to develop the real usefull code I need to solve some infrastructure problems. Too many new technologies will be used and should be banded together. The application will be distributed as a standalone multi platform application for Windows, Linux and MacOS. It will use CCP's SSO-authentication and ESI web-services to do it's job but will be able to work with no internet connection in some kind of offline-mode. To be able utilize SSO-authentication it should be able to register a custom protocol (URL-scheme).

Unfortunatly I have no Mac and don't plan to spend money to buy it. May be someday I will setup a Hackintosh in a virtual machine but up then the MacOS version is not a target. I will test this prototype only on Windows 10 and Ubuntu 17.10.

So the list of must have features of this prototype is as follows (on both Windows and Linux):
* [x] Buildable.
* [x] Register custom protocol.
* [x] Singleton instance.
* [ ] Add an EVE-character using SSO-authentication.
* [ ] Aqure and display the character info and the current skill training queue using ESI web-service.
* [ ] HMR for Electron main.
* [ ] HMR for Electron renderer.

## The Base Architecture ##
* Frontend implemented using the current Angular version. So it is a SPA.
* Backend implemented using .NET Core 2. 
* Frontend talks to backend using ASP.NET Core web-service.
* Frontend and backend placed into an Electron shell.

## Build ##
You need to use npm 4.6.1. Newer versions up to current (5.5.1) are buggy.
