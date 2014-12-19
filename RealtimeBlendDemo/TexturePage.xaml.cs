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
            FullscreenTextures = new ObservableCollection<Texture>
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

            PositionalTextures = new ObservableCollection<Texture>
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