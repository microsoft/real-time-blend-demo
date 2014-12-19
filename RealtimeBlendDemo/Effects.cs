/*
 * Copyright (c) 2014 Microsoft Mobile
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using Lumia.Imaging;
using Lumia.Imaging.Compositing;
using RealtimeBlendDemo.Resources;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Phone.Media.Capture;
using Windows.Storage.Streams;
using Windows.UI;

namespace RealtimeBlendDemo
{
    public class Effects : ICameraEffect
    {
        private PhotoCaptureDevice _photoCaptureDevice;
        private CameraPreviewImageSource _cameraPreviewImageSource;
        private BlendEffect _blendEffect;
        
        private Rect _targetArea;
        private double _targetAreaRotation;
        private Uri _blendImageUri;
        private IImageProvider _blendImageProvider;
        private int _effectIndex = 1;
        private const int EffectCount = 16;
        private readonly Semaphore _semaphore = new Semaphore(1, 1);

        public String EffectName { get; private set; }

        public double GlobalAlpha { get; set; }

        public PhotoCaptureDevice PhotoCaptureDevice
        {
            set
            {
                if (_photoCaptureDevice != value)
                {
                    while (!_semaphore.WaitOne(100));

                    _photoCaptureDevice = value;

                    Initialize();

                    _semaphore.Release();
                }
            }
        }

        public Effects()
        {
            GlobalAlpha = 0.5;
            _targetArea = new Rect(0, 0, 1, 1);
        }

        ~Effects()
        {
            while (!_semaphore.WaitOne(100));

            Uninitialize();

            _semaphore.Release();
        }

        public async Task GetNewFrameAndApplyEffect(IBuffer frameBuffer, Size frameSize)
        {
            if (_semaphore.WaitOne(500))
            {
                var scanlineByteSize = (uint)frameSize.Width * 4; // 4 bytes per pixel in BGRA888 mode
                var bitmap = new Bitmap(frameSize, ColorMode.Bgra8888, scanlineByteSize, frameBuffer);

                try
                {
                    if (_blendEffect != null)
                    {
                        _blendEffect.GlobalAlpha = GlobalAlpha;

                        var renderer = new BitmapRenderer(_blendEffect, bitmap);
                        await renderer.RenderAsync();
                    }
                    else
                    {
                        var renderer = new BitmapRenderer(_cameraPreviewImageSource, bitmap);
                        await renderer.RenderAsync();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("RealtimeBlendDemo.GetNewFrameAndApplyEffect(): "
                        + ex.ToString());
                }

                _semaphore.Release();
            }
        }

        public void SetTexture(Uri textureUri)
        {
            if (_semaphore.WaitOne(500))
            {
                Uninitialize();

                _blendImageUri = textureUri;

                Initialize();

                _semaphore.Release();
            }
        }

        public void SetTargetArea(Rect targetArea, double targetAreaRotation)
        {
            if (_semaphore.WaitOne(500))
            {
                _targetArea = targetArea;
                _targetAreaRotation = targetAreaRotation;

                if (_blendEffect != null) {
                    _blendEffect.TargetArea = targetArea;
                    _blendEffect.TargetAreaRotation = targetAreaRotation;
                }

                _semaphore.Release();
            }
        }

        public void NextEffect()
        {
            if (_semaphore.WaitOne(500))
            {
                Uninitialize();

                _effectIndex++;

                if (_effectIndex >= EffectCount)
                {
                    _effectIndex = 0;
                }

                Initialize();

                _semaphore.Release();
            }
        }

        public void PreviousEffect()
        {
            if (_semaphore.WaitOne(500))
            {
                Uninitialize();
                
                _effectIndex--;

                if (_effectIndex < 0)
                {
                    _effectIndex = EffectCount - 1;
                }

                Initialize();

                _semaphore.Release();
            }
        }

        private void Uninitialize()
        {
            if (_cameraPreviewImageSource != null)
            {
                _cameraPreviewImageSource.Dispose();
                _cameraPreviewImageSource = null;
            }

            if (_blendEffect != null)
            {
                _blendEffect.Dispose();
                _blendEffect = null;
            }           

            if (_blendImageProvider != null)
            {
                _blendImageProvider = null;
            }
        }

        private void Initialize()
        {
            _cameraPreviewImageSource = new CameraPreviewImageSource(_photoCaptureDevice);

            if (_blendImageUri != null)
            {
                _blendImageProvider = new StreamImageSource((System.Windows.Application.GetResourceStream(_blendImageUri).Stream));
            }
            else
            {
                var colorStops = new GradientStop[]
                {
                    new GradientStop { Color = Color.FromArgb(0xFF, 0xFF, 0x00, 0x00), Offset = 0.0 },
                    new GradientStop { Color = Color.FromArgb(0xFF, 0x00, 0xFF, 0x00), Offset = 0.7 },
                    new GradientStop { Color = Color.FromArgb(0xFF, 0x00, 0x00, 0xFF), Offset = 1.0 }
                };

                var gradient = new RadialGradient(new Point(0, 0), new EllipseRadius(1, 0), colorStops);

                var size = new Size(640, 480);

                _blendImageProvider = new GradientImageSource(size, gradient);
            }

            var nameFormat = "{0}/" + EffectCount + " - {1}";

            switch (_effectIndex)
            {
                case 0:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_None);
                    }
                    break;

                case 1:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Normal);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource,_blendImageProvider, BlendFunction.Normal, GlobalAlpha);
                    }
                    break;

                case 2:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Multiply);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource, _blendImageProvider, BlendFunction.Multiply, GlobalAlpha);
                    }
                    break;

                case 3:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Add);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource, _blendImageProvider, BlendFunction.Add, GlobalAlpha);
                    }
                    break;

                case 4:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Color);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource,_blendImageProvider, BlendFunction.Color, GlobalAlpha);
                    }
                    break;

                case 5:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Colorburn);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource,_blendImageProvider, BlendFunction.Colorburn, GlobalAlpha);
                    }
                    break;

                case 6:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Colordodge);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource,_blendImageProvider, BlendFunction.Colordodge, GlobalAlpha);
                    }
                    break;

                case 7:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Overlay);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource,_blendImageProvider, BlendFunction.Overlay, GlobalAlpha);
                    }
                    break;

                case 8:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Softlight);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource,_blendImageProvider, BlendFunction.Softlight, GlobalAlpha);
                    }
                    break;

                case 9:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Screen);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource,_blendImageProvider, BlendFunction.Screen, GlobalAlpha);
                    }
                    break;

                case 10:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Hardlight);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource,_blendImageProvider, BlendFunction.Hardlight, GlobalAlpha);
                    }
                    break;

                case 11:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Darken);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource,_blendImageProvider, BlendFunction.Darken, GlobalAlpha);
                    }
                    break;

                case 12:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Lighten);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource, _blendImageProvider, BlendFunction.Lighten, GlobalAlpha);
                    }
                    break;

                case 13:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Hue);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource, _blendImageProvider, BlendFunction.Hue, GlobalAlpha);
                    }
                    break;

                case 14:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Exclusion);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource, _blendImageProvider, BlendFunction.Exclusion, GlobalAlpha);
                    }
                    break;

                case 15:
                    {
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Blend_Difference);
                        _blendEffect = new BlendEffect(_cameraPreviewImageSource,_blendImageProvider, BlendFunction.Difference, GlobalAlpha);
                    }
                    break;
            }

            if (_blendEffect != null)
            {
                _blendEffect.TargetArea = _targetArea;
                _blendEffect.TargetAreaRotation = _targetAreaRotation;                
            }
        }
    }
}