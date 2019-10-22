

# Trove Downloader

This program is to download all the items in the Humble Bundle Trove. The code is a mess, but it works, and is relatively efficient at what it does (using a total of 20MB of RAM during the entire process)

*****
# How it works

* Download (or compile) the files listed below.
* On first run you will have to provide a session key (Details on how to get it below)
* The program will download trove.json from the Humble Bundle API
* it will then ask you what you would like to download. Your choices are Windows, Mac, Linux, or Everything

****
# Installation.

If you have Visual Studio then you can compile it yourself.  
If you don't, the built installer is below.  
* [Windows](https://ltscdn.m6.nz/humble/trove-downloader.exe?version=1.3.2)

Put the files where-ever you want and move onto the next part.

****

# Usage

### Session Key

You will need to get a session key, [Instructions here](https://github.com/talonius/hb-downloader/wiki/Using-Session-Information-From-Windows-For-hb-downloader)

### Arguments

The program takes 4 arguments:

* Session Key - Required
* Location - Optional - defaults to same folder.
* Platform - Optional - defaults to Windows
* Download Type - Optional - defaults to direct

### Actually using it - Windows

1. Open up the application. If you've missed anything, it'll let you know

### FAQ

 * The application in bin/Release is only 1,106KB. Surely this can't be the entire application, can it?
 

> Believe it or not, that's the entire application.

* Why is it taking so long to start up?

> The trove downloader takes a bit to start up because it's downloading the list of games from Humble Bundle. Don't worry, it should pop up within about 30 seconds

* Why is the installer so big if the application is only 1MB or so?

> The installer you're downloading from the CDN includes the prerequisite .NET 4.6 installer, as well as a font, and a few library the application needs

* It crashed! Help!

> File an issue [here](https://github.com/td512/Humble-Trove-Downloader/issues) and I'll take a look. If you've got the .NET stack trace, please include that in your report
 
****

# Improving this

I also plan to add Mac support, but for now, this will do.

If you have any ideas on how to improve this, submit a PR, PM me on reddit (/u/td512) or ping me on discord (TheoM#0331)

****

# Contributions

A big thank you to [Advanced Installer](https://www.advancedinstaller.com) for providing a license for their application. Without that, the installation wouldn't quite be so simple
![Advanced Installer](https://github.com/td512/Humble-Trove-Downloader/blob/master/contrib/contrib_logo_ai.png =200x)
