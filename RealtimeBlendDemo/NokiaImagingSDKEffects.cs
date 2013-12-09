/*
 * Copyright © 2013 Nokia Corporation. All rights reserved.
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners. 
 * See LICENSE.TXT for license information.
 */

using Nokia.Graphics.Imaging;
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
    public class NokiaImagingSDKEffects : ICameraEffect
    {
        private PhotoCaptureDevice _photoCaptureDevice = null;
        private CameraPreviewImageSource _cameraPreviewImageSource = null;
        private FilterEffect _filterEffect = null;
        private BlendFilter _blendFilter = null;
        private Uri _blendImageUri = null;
        private IImageProvider _blendImageProvider = null;
        private int _effectIndex = 1;
        private int _effectCount = 16;
        private double _effectLevel = 0.5;
        private Semaphore _semaphore = new Semaphore(1, 1);

        public String EffectName { get; private set; }

        public double EffectLevel
        {
            get
            {
                return _effectLevel;
            }

            set
            {
                if (_effectLevel != value)
                {
                    _effectLevel = value;
                }
            }
        }

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

        ~NokiaImagingSDKEffects()
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
                    if (_filterEffect != null)
                    {
                        _blendFilter.Level = _effectLevel;

                        var renderer = new BitmapRenderer(_filterEffect, bitmap);
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

        public void NextEffect()
        {
            if (_semaphore.WaitOne(500))
            {
                Uninitialize();

                _effectIndex++;

                if (_effectIndex >= _effectCount)
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
                    _effectIndex = _effectCount - 1;
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

            if (_filterEffect != null)
            {
                _filterEffect.Dispose();
                _filterEffect = null;
            }

            if (_blendFilter != null)
            {
                _blendFilter.Dispose();
                _blendFilter = null;
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
                    new GradientStop() { Color = Color.FromArgb(0xFF, 0xFF, 0x00, 0x00), Offset = 0.0 },
                    new GradientStop() { Color = Color.FromArgb(0xFF, 0x00, 0xFF, 0x00), Offset = 0.7 },
                    new GradientStop() { Color = Color.FromArgb(0xFF, 0x00, 0x00, 0xFF), Offset = 1.0 }
                };

                var gradient = new RadialGradient(new Point(0, 0), new EllipseRadius(1, 0), colorStops);

                var size = new Size(640, 480);

                _blendImageProvider = new GradientImageSource(size, gradient);
            }

            var nameFormat = "{0}/" + _effectCount + " - {1}";

            switch (_effectIndex)
            {
                case 0:
                    {
                        EffectName = String.Format(nameFormat, 1, AppResources.Filter_None);
                    }
                    break;

                case 1:
                    {
                        EffectName = String.Format(nameFormat, 2, AppResources.Filter_Blend_Normal);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Normal, EffectLevel);
                    }
                    break;

                case 2:
                    {
                        EffectName = String.Format(nameFormat, 3, AppResources.Filter_Blend_Multiply);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Multiply, EffectLevel);
                    }
                    break;

                case 3:
                    {
                        EffectName = String.Format(nameFormat, 4, AppResources.Filter_Blend_Add);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Add, EffectLevel);
                    }
                    break;

                case 4:
                    {
                        EffectName = String.Format(nameFormat, 5, AppResources.Filter_Blend_Color);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Color, EffectLevel);
                    }
                    break;

                case 5:
                    {
                        EffectName = String.Format(nameFormat, 6, AppResources.Filter_Blend_Colorburn);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Colorburn, EffectLevel);
                    }
                    break;

                case 6:
                    {
                        EffectName = String.Format(nameFormat, 7, AppResources.Filter_Blend_Colordodge);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Colordodge, EffectLevel);
                    }
                    break;

                case 7:
                    {
                        EffectName = String.Format(nameFormat, 8, AppResources.Filter_Blend_Overlay);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Overlay, EffectLevel);
                    }
                    break;

                case 8:
                    {
                        EffectName = String.Format(nameFormat, 9, AppResources.Filter_Blend_Softlight);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Softlight, EffectLevel);
                    }
                    break;

                case 9:
                    {
                        EffectName = String.Format(nameFormat, 10, AppResources.Filter_Blend_Screen);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Screen, EffectLevel);
                    }
                    break;

                case 10:
                    {
                        EffectName = String.Format(nameFormat, 11, AppResources.Filter_Blend_Hardlight);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Hardlight, EffectLevel);
                    }
                    break;

                case 11:
                    {
                        EffectName = String.Format(nameFormat, 12, AppResources.Filter_Blend_Darken);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Darken, EffectLevel);
                    }
                    break;

                case 12:
                    {
                        EffectName = String.Format(nameFormat, 13, AppResources.Filter_Blend_Lighten);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Lighten, EffectLevel);
                    }
                    break;

                case 13:
                    {
                        EffectName = String.Format(nameFormat, 14, AppResources.Filter_Blend_Hue);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Hue, EffectLevel);
                    }
                    break;

                case 14:
                    {
                        EffectName = String.Format(nameFormat, 15, AppResources.Filter_Blend_Exclusion);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Exclusion, EffectLevel);
                    }
                    break;

                case 15:
                    {
                        EffectName = String.Format(nameFormat, 15, AppResources.Filter_Blend_Difference);
                        _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Difference, EffectLevel);
                    }
                    break;
            }

            if (_blendFilter != null)
            {
                var filters = new List<IFilter>();

                filters.Add(_blendFilter);

                _filterEffect = new FilterEffect(_cameraPreviewImageSource)
                {
                    Filters = filters
                };
            }
        }
    }
}