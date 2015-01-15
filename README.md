Hockey Streams Launcher
=====================
Hockey Streams Launcher is an open source application that enables you to view RTMP streams from hockeystreams.com in various media players such as VLC, MPC-HC, or MPlayer2 through [Livestreamer](http://livestreamer.tanuki.se/en/latest/) and [RTMPDump](http://rtmpdump.mplayerhq.hu/)

[![Build status](https://ci.appveyor.com/api/projects/status/o4exgam8doqcx96c?svg=true)](https://ci.appveyor.com/project/lukehutton/streamlauncher)
   
![](https://github.com/lukehutton/streamlauncher/blob/master/docs/screen1.PNG)

Features
----------
* SD/HD streaming
* Choose location of streams
* Auto-login
* Filtering by leagues, in progress, coming soon
* Highlight favorite team
* Show game information (time left, scoring)
* On/off scoring
* Choose from multiple media players via [Livestreamer](http://livestreamer.tanuki.se/en/latest/)
* Uses [RTMPDump](http://rtmpdump.mplayerhq.hu/) 
* One click away to play

Roadmap
----------
* On-Demand with Date picker
* Play live from beginning
* Game stats/roster/preview
* Resizable window

Prerequisites
-------------
 - [.NET Framework 4.5](http://www.microsoft.com/en-us/download/details.aspx?id=30653) [Required] 
 - [Microsoft Visual C++ 2008 Redistributable Package](http://www.microsoft.com/en-us/download/details.aspx?id=29) [Required]
 - [Livestreamer (CMD)](http://livestreamer.tanuki.se/en/latest/) [Required] 
 - One of the following media players:
   - [VLC Media Player](http://www.videolan.org/vlc/index.html) 
   - [MPC-HC](http://mpc-hc.org/)
   - [mplayer2](http://www.mplayer2.org/)

Downloads
-------
You are more than welcome to take the code provided and compile a version of it yourself, or you can find the latest release candidates here: [downloads](https://github.com/lukehutton/streamlauncher/releases).

Issues
-------
If you are experiencing any issues such as crashes, please report them [here](https://github.com/lukehutton/streamlauncher/issues/new).

How to install
-------
1. Download prequisites above: 
  1. Install Livestreamer bundled with RTMPDump (not .zip installation). Tested with version v1.11.1.
  2. Install VLC Media Player. Tested with v2.1.5.
2. Download and install latest in [downloads](https://github.com/lukehutton/streamlauncher/releases)   
3. Set the correct paths in the Settings dialog for Livestreamer and media player installations.
4. Set proper file caching for the media player under Media Player Args:
   (See http://livestreamer.readthedocs.org/en/latest/issues.html#enable-caching-in-your-player)
   i.e. for vlc, --file-caching=5000 and for mplayer2, -cache 4096

Build History
-------
See [build history](https://ci.appveyor.com/project/lukehutton/streamlauncher/history) at CI/CD cloud provider [Appveyor](http://www.appveyor.com)