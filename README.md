# Open Terraria API (OTAPI)  [![Build Status](https://travis-ci.org/DeathCradle/Open-Terraria-API.svg?branch=master)](https://travis-ci.org/DeathCradle/Open-Terraria-API) [![Slack](https://img.shields.io/badge/Chat%20on-Slack-blue.svg)](https://openterraria.com/slack)
*Supporting Terraria v1.3!*  

The Open Terraria API is a modification of the official Terraria client & server software.

In this API it contains the tools to run a cross platform vanilla client or server with plugin support. Plugins written using .NET as well as LUA can tap into and use the extended functionalities such as:
 1. OWIN based web server & RESTful api using the Web API 2
 2. Console and chat logging system
 3. Many plugin hooks such as Drawing,Server,Player,NPC and World.
 4. Optional Entity Framework 6 support (v7 soon) for cross platform database access [Tested: MSSQL,MySQL,SQLite]
 5. Memory optimisations

What makes this mod special is not only how quick updates are, but the capability to patch both the client and server binaries for each platform. This means you can write a generic cross platform plugin for both the client and server with one code base.


While the client support is limited feel free to join the TDSM team over on slack to help out or request functionalities. [![Slack](https://img.shields.io/badge/Chat%20on-Slack-blue.svg)](https://openterraria.com/slack)


Known plugins or mods using this API:
  - TDSM - [https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod) [MOD]
  - Orion [https://github.com/NyxStudios/Orion/](https://github.com/NyxStudios/Orion/)
