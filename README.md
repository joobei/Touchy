<img alt="Touchy Logo" src="https://image.ibb.co/eADw5d/logo.png" width="300" />

### A C/C++ plugin for Phantom Omni and 3DS Touch/TouchX
```diff
- Currently Only supports Windows though it should be straightforward to port it to GNU/Linux.

```
This is a wrapper for OpenHaptics geared towards Unity3D. This plugin is under development. 

Build requirements:
* Cmake
* [OpenHaptics](http://support1.geomagic.com/Support/5605/5668/en-US/Article/View/2365/How-do-I-download-and-get-Developer-Support-for-OpenHaptics/378) by 3DS (which, ironically, is closed source). The build system will try to locate the header and library files under $(OH_SDK_BASE) Environment variable in Windows.
* (`Optional`) Set Environment Variable Unity_Plugins_Folder and uncomment the bottom of CMakeLists.txt to make the build system automatically copy the built library to your Assets/Plugins folder.