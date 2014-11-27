Real-time Blend Demo
=====================

Nokia example application demonstrating the real-time use of the blend effect
provided by the Nokia Imaging SDK, the effect being applied to a stream of
images received from the phone's camera.

This example application is hosted in GitHub:
https://github.com/Microsoft/real-time-blend-demo

For more information on implementation visit Nokia Lumia Developer's Library:
http://developer.nokia.com/Resources/Library/Lumia/nokia-imaging-sdk/sample-projects/real-time-blend-demo.html

Developed with Microsoft Visual Studio Express for Windows Phone 2012.

Compatible with:

 * Windows Phone 8

Tested to work on:

 * Nokia Lumia 520
 * Nokia Lumia 1020
 * Nokia Lumia 1520


Instructions
------------

Make sure you have the following installed:

* Windows 8
* Windows Phone SDK 8.0
* Nuget 2.7 or later

This project uses the Nuget Automatic Package Restore that was introduced in Nuget 2.7.
For this reason you must have Nuget 2.7 or later, otherwise building the project will fail.

To update your Nuget Package Manager installation:

1. Open Visual Studio Express for Windows Phone
2. From the menu, navigate to Tools, Extensions and Updates, Updates, Visual Studio Gallery
3. Check if there is a Nuget update available and if yes, install it.

For more information on Nuget see: http://docs.nuget.org/docs/reference/package-restore

To build and run the sample:

1. Open the SLN file:
   File > Open Project, select the solution (.sln postfix) file
2. Select the target 'Device' and platform 'ARM'.
3. Press F5 to build the project and run it on device.

If the project does not compile on the first attempt it's possible that you
did not have the required packages yet. With Nuget 2.7 or later the missing
packages are fetched automatically when build process is invoked, so try
building again. If some packages cannot be found there should be an
error stating this in the Output panel in Visual Studio Express.

For more information on deploying and testing applications see:
http://msdn.microsoft.com/library/windowsphone/develop/ff402565(v=vs.105).aspx


About the implementation
------------------------

Important folders:

| Folder | Description |
| ------ | ----------- |
| / | Contains the project file, the license information and this file (README.md) |
| RealtimeBlendDemo | Root folder for the implementation files.  |
| RealtimeBlendDemo/Assets | Graphic assets like icons and tiles. |
| RealtimeBlendDemo/Properties | Application property files. |
| RealtimeBlendDemo/Resources | Application resources. |

Important files:

| File | Description |
| ---- | ----------- |
| Mainpage.cs | Renders camera live feed with the selected blend mode, level and texture. |

Important classes:

| Class | Description |
| ----- | ----------- |
| MainPage | Renders camera live feed to an Image with the selected blend mode, level and texture. |


Known issues
------------

None.


License
-------

See the license file delivered with this project.
The license is also available online at
https://github.com/Microsoft/real-time-blend-demo/blob/master/Licence.txt


Version history
---------------

* 1.2: Partial blend filter

* 1.1: Fourth public release of Real-time Blend Demo
  - Updated to the latest Nokia Imaging SDK version 1.1.177

* 1.0: First public release of Real-time Blend Demo
