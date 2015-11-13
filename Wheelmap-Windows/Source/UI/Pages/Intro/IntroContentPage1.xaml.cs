using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap.UI.Pages.Base;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap.Source.UI.Pages.Intro {

    sealed partial class IntroContentPage1 : BasePage {
        public IntroContentPage1() {
            this.InitializeComponent();
        }
        
        public override void OnNewParams(object args) {
            base.OnNewParams(args);
            if (args is IntroArguments) {
                var arguments = args as IntroArguments;
                imageView.Source = new BitmapImage(new
                          Uri(arguments.ImageUri, UriKind.RelativeOrAbsolute));
                titleTextBlock.Text = arguments.Title;
                textTextBlock.Text = arguments.Text;
            }
        }
        
    }
}
