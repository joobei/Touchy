<img alt="Touchy Logo" src="https://image.ibb.co/eADw5d/logo.png" width="300" />

### A C/C++ plugin for Phantom Omni and 3DS Touch/TouchX

`Currently Only supports Windows though it should be straightforward to port it to GNU/Linux.`


This is a wrapper for OpenHaptics geared towards Unity3D. This plugin is under development. 

## Supports:
- A single Haptic device
- A single Sphere shape

### Build requirements:
* Cmake (unless you want to make a project manually)
* [OpenHaptics](http://support1.geomagic.com/Support/5605/5668/en-US/Article/View/2365/How-do-I-download-and-get-Developer-Support-for-OpenHaptics/378) by 3DS (which, ironically, is closed source). The build system will try to locate the header and library files under $(OH_SDK_BASE) Environment variable in Windows.
* (`Optional`) Set Environment Variable Unity_Plugins_Folder and uncomment the bottom of CMakeLists.txt to make the build system automatically copy the built library to your Assets/Plugins folder.

### Unity Side:
- Copy the plugin to your Assets/Plugins folder
- Drag HapticManager.cs script to an Empty GameObject
- Play
- HapticManager.cs, in the Start() method, adds a sphere of radius 30 mm at (0,0,0)

### Customize
- Both the haptic plugin source and the Unity3D manager source is self-explanatory.
- Due to Unity3D/Mono limitations, if you make changes to the C/C++ library you must completely close and restart unity to load the new build of the .dll.
- Contributions are welcome.


### TODO
- Multiple Haptic Devices
- Automatic VR/Haptic calibration method
- Arbitrary Haptic Shapes
