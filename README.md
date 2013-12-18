Real-time Blend Demo
=====================

Nokia example application demonstrating the real-time use of the blend effect
provided by the Nokia Imaging SDK, the effect being applied to a stream of
images received from the phone's camera.

This example application is hosted in GitHub:
https://github.com/nokia-developer/real-time-blend-demo

For more information on implementation visit Nokia Lumia Developer's Library:
http://developer.nokia.com/Resources/Library/Lumia/#!nokia-imaging-sdk/sample-projects/real-time-blend-demo.html

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
To update your Nuget Package Manager installation in Visual Studio Express to the latest
version, go to Tools -> Extensions and Updates -> Updates -> Visual Studio Gallery
and see if there is a Nuget update available.

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

    Copyright © 2013 Nokia Corporation. All rights reserved.
    
    Nokia, Nokia Developer, and HERE are trademarks and/or registered trademarks of
    Nokia Corporation. Other product and company names mentioned herein may be
    trademarks or trade names of their respective owners.
    
    License
    Subject to the conditions below, you may use, copy, modify and/or merge copies
    of this software and associated content and documentation files (the “Software”)
    to test, develop, publish, distribute, sub-license and/or sell new software
    derived from or incorporating the Software, solely in connection with Nokia
    devices. Some of the documentation, content and/or software maybe licensed under
    open source software or other licenses. To the extent such documentation,
    content and/or software are included, licenses and/or other terms and conditions
    shall apply in addition and/or instead of this notice. The exact terms of the
    licenses, disclaimers, acknowledgements and notices are reproduced in the
    materials provided, or in other obvious locations. No other license to any other
    intellectual property rights is granted herein.
    
    This file, unmodified, shall be included with all copies or substantial portions
    of the Software that are distributed in source code form.
    
    The Software cannot constitute the primary value of any new software derived
    from or incorporating the Software.
    
    Any person dealing with the Software shall not misrepresent the source of the
    Software.
    
    Disclaimer
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
    FOR A PARTICULAR PURPOSE, QUALITY AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES (INCLUDING,
    WITHOUT LIMITATION, DIRECT, SPECIAL, INDIRECT, PUNITIVE, CONSEQUENTIAL,
    EXEMPLARY AND/ OR INCIDENTAL DAMAGES) OR OTHER LIABILITY, WHETHER IN AN ACTION
    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
    SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    
    Nokia Corporation retains the right to make changes to this document at any
    time, without notice.


Version history
---------------

* 1.0: First public release of Real-time Blend Demo
