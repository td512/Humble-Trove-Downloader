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
If you dont then the built installer is below.  
If you are worried about the security of these they were built by gitlab on my behalf from the code I uploaded, here [have a look.](https://gitlab.com/silver_rust/trove_downloader/pipelines)

* [Windows](https://ltscdn.m6.nz/humble/trove-downloader.exe)

Put the files where-ever you want and move onto the next part.

****

# Useage

### Api Key

You will need to get a session key, [Instructions here](https://github.com/talonius/hb-downloader/wiki/Using-Session-Information-From-Windows-For-hb-downloader)

### Arguments

The program takes 4 arguments:

* Session Key - Required
* Location - Optional - defaults to same folder.
* Platform - Optional - defaults to Windows
* Download Type - Optional - defaults to direct

### Actually using it - Windows

1. Open up the application. If you've missed anything, it'll let you know

****

# Improving this

I also plan to add Mac support, but for now, this will do.

If you have any ideas on how to improve this, submit a PR, PM me on reddit (/u/td512) or ping me on discord (TheoM#0331)
