**Important Note**: This project is not working anymore due to Shazam server-side changes. Please see the [relevent issue](https://github.com/tomer8007/windows-shazam/issues/4) for more details.

# Shazam for Windows 7 #
[Shazam](http://www.shazam.com) is a popular music identification service for smartphones.
It works really well, but it doesn't have a version for Windows 7 computers.
_windows-shazam_ is a little software that takes a sample of music from your microphone, and recongnize it by sending it to Shazam's servers.

It basically does exactly the same thing as the official Shazam app, but on your windows 7 computer instead of your smartphone.

![http://i.imgur.com/AGsiyqN.png](http://i.imgur.com/AGsiyqN.png)

To download this software, see the Download section below.

## How it works ##
This software listens to a 10-seconds sample from your default microphone. Then it sends it to Shazam's server at http://msft.shazamid.com/orbit/DoRecognition1, and makes it think the request was sent from a smartphone.

## Usage ##
_windows-shazam_ is a portable software, so you don't have to install anything. Just run it, and make sure it has the essential DLLs in the same folder.

Basically just click the button, let it listen and get the song name. For more details see the [usage wiki page](https://github.com/tomer8007/windows-shazam/wiki/Usage).

## Download ##
You can download the latest compiled version from the [releases page](https://github.com/tomer8007/windows-shazam/releases).

Instructions:

1. Download the zip file.
2. Extract all the files to a folder.
3. Run the shazam.exe file and follow the [usage instructions](https://github.com/tomer8007/windows-shazam/wiki/Usage).
