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
using System.Windows.Input;
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

        private Point _position;
        private Point _initialPosition;
        private double _angle;
        private double _initialAngle;
        private double _scale;

        private const double DEFAULT_SCALE = 0.5;

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
                DragHintText.Visibility = Visibility.Visible;
                PinchHintText.Visibility = Visibility.Visible;

                _angle = 0;
                _initialAngle = 0;
                _scale = DEFAULT_SCALE;
                _position = new Point(0.5, 0.5);
                _initialPosition = new Point(0.5, 0.5);

                RefreshTargetArea();
            }
            else {
                DragHintText.Visibility = Visibility.Collapsed;
                PinchHintText.Visibility = Visibility.Collapsed;
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

        private void RefreshTargetArea()        
        {
            double x = Orientation.HasFlag(PageOrientation.LandscapeRight) ? 1 - _position.X : _position.X;
            double y = Orientation.HasFlag(PageOrientation.LandscapeRight) ? 1 - _position.Y : _position.Y;

            if (App.Texture.IsPositional) {
                _cameraEffect.SetTargetArea(
                    new Windows.Foundation.Rect(
                        x - (_scale / 2),
                        y - (_scale / 2),
                        _scale,
                        _scale),
                        _angle
                );
            }
        }

        private void LayoutRoot_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            _initialAngle = _angle;
        }

        private void LayoutRoot_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                // Rotate
                _angle = _initialAngle + AngleOf(e.PinchManipulation.Original) - AngleOf(e.PinchManipulation.Current);

                // Scale
                _scale *= e.PinchManipulation.DeltaScale;

                // Translate according to pinch center
                double deltaX = (e.PinchManipulation.Current.SecondaryContact.X + e.PinchManipulation.Current.PrimaryContact.X) / 2 -
                    (e.PinchManipulation.Original.SecondaryContact.X + e.PinchManipulation.Original.PrimaryContact.X) / 2;
                deltaX /= LayoutRoot.ActualWidth;
                    
                double deltaY = (e.PinchManipulation.Current.SecondaryContact.Y + e.PinchManipulation.Current.PrimaryContact.Y) / 2 -
                    (e.PinchManipulation.Original.SecondaryContact.Y + e.PinchManipulation.Original.PrimaryContact.Y) / 2;
                deltaY /= LayoutRoot.ActualHeight;

                _position.X = _initialPosition.X + deltaX;
                _position.Y = _initialPosition.Y + deltaY;               
            }
            else {
                // Translate
                _initialAngle = _angle;
                _position.X += e.DeltaManipulation.Translation.X / LayoutRoot.ActualWidth;
                _position.Y += e.DeltaManipulation.Translation.Y / LayoutRoot.ActualHeight;
                _initialPosition.X = _position.X;
                _initialPosition.Y = _position.Y;
            }

            e.Handled = true;

            RefreshTargetArea();
        }

        private double AngleOf(PinchContactPoints points)
        {
            Point vec = new Point(points.SecondaryContact.X - points.PrimaryContact.X, points.SecondaryContact.Y - points.PrimaryContact.Y);

            double angle = Math.Atan2(vec.Y, vec.X);

            if (angle < 0)
            {
                angle += 2 * Math.PI;
            }

            return angle * 180 / Math.PI;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _cameraEffect.EffectLevel = e.NewValue;
        }

    }
}