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
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Navigation;

namespace RealtimeBlendDemo
{

    /// <summary>
    /// Page for selecting the texture for the blend effect.
    /// </summary>
    public partial class TexturePage : PhoneApplicationPage
    {
        public ObservableCollection<Texture> FullscreenTextures { get; private set; }
        public ObservableCollection<Texture> PositionalTextures { get; private set; }

        public TexturePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            FullscreenTextures = new ObservableCollection<Texture>()
            {
                new Texture(new Uri(@"Assets/Textures/texture1.png", UriKind.Relative), new Uri(@"Assets/Textures/texture1-thumb.jpg", UriKind.Relative)),
                new Texture(new Uri(@"Assets/Textures/texture2.jpg", UriKind.Relative), new Uri(@"Assets/Textures/texture2-thumb.jpg", UriKind.Relative)),
                new Texture(new Uri(@"Assets/Textures/texture3.jpg", UriKind.Relative), new Uri(@"Assets/Textures/texture3-thumb.jpg", UriKind.Relative)),
                new Texture(new Uri(@"Assets/Textures/texture4.jpg", UriKind.Relative), new Uri(@"Assets/Textures/texture4-thumb.jpg", UriKind.Relative)),
                new Texture(new Uri(@"Assets/Textures/texture5.jpg", UriKind.Relative), new Uri(@"Assets/Textures/texture5-thumb.jpg", UriKind.Relative)),
                new Texture(new Uri(@"Assets/Textures/texture6.jpg", UriKind.Relative), new Uri(@"Assets/Textures/texture6-thumb.jpg", UriKind.Relative)),
                new Texture(new Uri(@"Assets/Textures/texture7.jpg", UriKind.Relative), new Uri(@"Assets/Textures/texture7-thumb.jpg", UriKind.Relative)),
                new Texture(new Uri(@"Assets/Textures/texture8.jpg", UriKind.Relative), new Uri(@"Assets/Textures/texture8-thumb.jpg", UriKind.Relative)),
                new Texture(null, new Uri(@"Assets/Textures/gradient-thumb.jpg", UriKind.Relative))
            };

            PositionalTextures = new ObservableCollection<Texture>()            
            {
                new Texture(new Uri(@"Assets/Textures/texture1.png", UriKind.Relative), new Uri(@"Assets/Textures/texture1-thumb.jpg", UriKind.Relative), true),
                new Texture(new Uri(@"Assets/Textures/texture9.png", UriKind.Relative), new Uri(@"Assets/Textures/texture9-thumb.jpg", UriKind.Relative), true)
            };

            DataContext = this;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            DataContext = null;
            FullscreenTextures = null;
        }

        private void Thumbnail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.Texture = ((FrameworkElement)sender).DataContext as Texture;

            NavigationService.GoBack();
        }
    }
}