Real-time Blend Demo
=====================

Lumia example application demonstrating the real-time use of the blend effect
provided by the Lumia Imaging SDK, the effect being applied to a stream of
images received from the phone's camera.

This example application is hosted in GitHub:
https://github.com/microsoft/real-time-blend-demo

For more information on implementation visit Lumia Developer's Library:
http://go.microsoft.com/fwlink/?LinkId=528376

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
   	Copyright (c) 2014 Microsoft Mobile
 
	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:
	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.
	
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.

Version history
---------------

* 1.2: Partial blend filter

* 1.1: Fourth public release of Real-time Blend Demo
  - Updated to the latest Nokia Imaging SDK version 1.1.177

* 1.0: First public release of Real-time Blend Demo
