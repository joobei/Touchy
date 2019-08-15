<img alt="Touchy Logo" src="https://image.ibb.co/eADw5d/logo.png" width="300" />
<img alt="Crossmodal Learning Logo" src="https://www.crossmodal-learning.org/4034886/cml-logo-200x200-527600dd4d55892300f5b82bd80fc0918e4c070e.png" width="100" />
<img alt="DFG Logo" src="https://www.dfg.de/zentralablage/bilder/service/logos_corporate_design/logo_international_415.png" width="100" />

### A Unity3D plugin for Phantom Omni and 3DS Touch/TouchX written in C/C++ 

### Tested With:
- <img alt="Sensable Phantom Omni" src="https://www.researchgate.net/profile/Eduardo_Castello/publication/316538856/figure/fig7/AS:487955379822598@1493349036621/Phantom-OMNI-Haptic-Device-by-SensAble-Technologies.jpg" width="200px">Sensable Phantom Omni (IEEE 1394 Interface)
- <img alt="3D Systems Touch" src="https://www.3dsystems.com/sites/default/files/styles/image_general_full_size/public/2017-12/3d-systems-touch-hero.png" width="170px">3DSystems Touch 
  
`Currently Only supports Windows though it should be straightforward to port it to GNU/Linux.`

This is a wrapper for OpenHaptics geared towards Unity3D. This plugin is under development. 

## Supports:
- Sphere rendering
- Gravity well rendering

### Build requirements:
* Cmake (unless you want to make a project manually)
* [OpenHaptics](https://3dssupport.microsoftcrmportals.com/knowledgebase/article/KA-01460/en-us) by 3DS (which, ironically, is closed source). The build system will try to locate the header and library files under $(OH_SDK_BASE) Environment variable in Windows.
* (`Optional`) Set Environment Variable Unity_Plugins_Folder and uncomment the bottom of CMakeLists.txt to make the build system automatically copy the built library to your Assets/Plugins folder.

### To use in Unity 3D:
- Copy the built plugin to your Assets/Plugins folder (or make the build system automatically do it for you - see CMakelists.txt)
- Create a new c# script that inherits from :Haptic
- Implement OnStylusButtonDown method
- (Optional) use stopCallback and startSphereCallback(...) to add a sphere and test your haptic device.
- Press Play

### Customize
- Both the haptic plugin source and the Unity3D haptic class source are self-explanatory.
- Due to Unity3D/Mono limitations, if you make changes to the C/C++ library you must completely close and restart unity to load the new build of the .dll.
- Feel free to open an issue with feature requests
