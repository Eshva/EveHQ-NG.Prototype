# EveHQ-NG proof of concepts application #
It is a EveHQ-NG proof of concepts application. EveHQ-NG stands for EveHQ Next Generation. EveHQ itself is a well known support application for Eve Online game. I am the current maintainer of EveHQ. And it will die on May 2018. Why? Because CCP, the company behind the Eve Online will depricate some web-services on May 2018 that EveHQ uses and support other that not used by EveHQ.

Why don't I update the present version of EveHQ to use the new web-services? Because the change is huge and the source code of EveHQ is shit. It easer to rewrite the application from scratch than update its code. Spagetti-code written in VB.NET using WinForms RAD-approach where UI-code mixed with web-related code in files tens thousand lines files... Programmers will understand me.

Secondly this application designed to work only in Windows. There are requests to make it multi platform. With WinForms and classic .NET Framework it isn't possible.

The third reason is I reenvisioned this tool. The legacy EveHQ is a banch of not connected or loosely connected tools. I want to make it more 'EVE professions' centric tool. For instance when the player plays the role of a manufacturer he doesn't need a map or a ship fitter but needs a way to control production lines. And controversal if he is in a solo PvP session he doesn't need the manufacturer tools but needs the galaxy map and some intel about pilots around.

## Prototype targets ##
So what is this prototype for?

Before I will start to develop the real usefull code I need to solve some infrastructure problems. Too many new technologies will be used and should be banded together.

## Build ##
You need to use npm 4.6.1. Newer versions up to current (5.5.1) are buggy.
