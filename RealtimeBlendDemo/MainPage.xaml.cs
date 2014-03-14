/**
 * Copyright (c) 2013 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RealtimeBlendDemo.Resources;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Windows.Phone.Media.Capture;

namespace RealtimeBlendDemo
{
    public partial class MainPage : PhoneApplicationPage
    {
        private MediaElement _mediaElement;
        private NokiaImagingSDKEffects _cameraEffect;
        private CameraStreamSource _cameraStreamSource;
        private Semaphore _cameraSemaphore = new Semaphore(1, 1);
        private bool _zooming;

        public MainPage()
        {
            InitializeComponent();

            ApplicationBar = new ApplicationBar();

            var textureButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/texture.png", UriKind.Relative))
            {
                Text = AppResources.MainPage_TextureButton
            };
            textureButton.Click += TextureButton_Click;

            ApplicationBar.Buttons.Add(textureButton);

            var previousButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/previous.png", UriKind.Relative))
            {
                Text = AppResources.MainPage_PreviousEffectButton
            };
            previousButton.Click += PreviousButton_Click;

            ApplicationBar.Buttons.Add(previousButton);

            var nextButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/next.png", UriKind.Relative))
            {
                Text = AppResources.MainPage_NextEffectButton
            };
            nextButton.Click += NextButton_Click;

            ApplicationBar.Buttons.Add(nextButton);

            var aboutMenuItem = new ApplicationBarMenuItem {Text = AppResources.MainPage_AboutPageButton};
            aboutMenuItem.Click += AboutMenuItem_Click;

            ApplicationBar.MenuItems.Add(aboutMenuItem);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.Camera != null)
            {
                Initialize();
            }
            else
            {
                StatusTextBlock.Text = AppResources.MainPage_Status_InitializingCameraFailed;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Uninitialize();
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);

            if (App.Camera != null)
            {
                double canvasAngle;

                if (Orientation.HasFlag(PageOrientation.LandscapeLeft))
                {
                    canvasAngle = App.Camera.SensorRotationInDegrees - 90;
                }
                else if (Orientation.HasFlag(PageOrientation.LandscapeRight))
                {
                    canvasAngle = App.Camera.SensorRotationInDegrees + 90;
                }
                else // PageOrientation.PortraitUp
                {
                    canvasAngle = App.Camera.SensorRotationInDegrees;
                }

                BackgroundVideoBrush.RelativeTransform = new RotateTransform()
                {
                    CenterX = 0.5,
                    CenterY = 0.5,
                    Angle = canvasAngle
                };

                App.Camera.SetProperty(KnownCameraGeneralProperties.EncodeWithOrientation, canvasAngle);
            }
        }

        private void Initialize()
        {
            StatusTextBlock.Text = AppResources.MainPage_Status_InitializingCamera;

            _cameraEffect = new NokiaImagingSDKEffects {PhotoCaptureDevice = App.Camera, EffectLevel = 0.5};
            _cameraEffect.SetTexture(App.Texture.File);

            if (App.Texture.IsPositional)
            {
                _cameraEffect.SetTargetArea(
                    new Windows.Foundation.Rect(
                        0.25,
                        0.25,
                        0.5,
                        0.5)
                );
                PositioningHintText.Visibility = Visibility.Visible;
            }
            else {
                _cameraEffect.SetTargetArea(
                    new Windows.Foundation.Rect(0, 0, 1, 1)
                );
                PositioningHintText.Visibility = Visibility.Collapsed;
            }

            LevelSlider.Value = 0.5;

            _cameraStreamSource = new CameraStreamSource(_cameraEffect, App.Camera.CaptureResolution);
            _cameraStreamSource.FrameRateChanged += CameraStreamSource_FPSChanged;

            _mediaElement = new MediaElement {Stretch = Stretch.UniformToFill, BufferingTime = new TimeSpan(0)};
            _mediaElement.SetSource(_cameraStreamSource);

            // Using VideoBrush in XAML instead of MediaElement, because otherwise
            // CameraStreamSource.CloseMedia() does not seem to be called by the framework:/

            BackgroundVideoBrush.SetSource(_mediaElement);

            StatusTextBlock.Text = _cameraEffect.EffectName;
        }

        private void Uninitialize()
        {
            StatusTextBlock.Text = "";

            if (_mediaElement != null)
            {
                _mediaElement.Source = null;
                _mediaElement = null;
            }

            if (_cameraStreamSource != null)
            {
                _cameraStreamSource.FrameRateChanged -= CameraStreamSource_FPSChanged;
                _cameraStreamSource = null;
            }

            _cameraEffect = null;
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            _cameraEffect.NextEffect();

            StatusTextBlock.Text = _cameraEffect.EffectName;
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            _cameraEffect.PreviousEffect();

            StatusTextBlock.Text = _cameraEffect.EffectName;
        }

        private void TextureButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/TexturePage.xaml", UriKind.Relative));
        }

        private void CameraStreamSource_FPSChanged(object sender, int e)
        {
            FrameRateTextBlock.Text = String.Format(AppResources.MainPage_FrameRateCounter_Format, e);
        }

        private async void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_cameraSemaphore.WaitOne(100))
            {
                await App.Camera.FocusAsync();

                _cameraSemaphore.Release();
            }
        }

        private void LayoutRoot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (App.Texture.IsPositional && !_zooming)
            {
                _cameraEffect.SetTargetArea(
                    new Windows.Foundation.Rect(
                        e.GetPosition(LayoutRoot).X / LayoutRoot.ActualWidth - 0.25,
                        e.GetPosition(LayoutRoot).Y / LayoutRoot.ActualHeight - 0.25,
                        0.5,
                        0.5)
                );
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _cameraEffect.EffectLevel = e.NewValue;
        }

        private void Slider_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            _zooming = true;
        }

        private void Slider_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            _zooming = false;
        }

    }
}