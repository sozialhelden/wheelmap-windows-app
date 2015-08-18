using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Wheelmap_Windows.Source.UI.Pages {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MenuPage : Page {
        public MenuPage() {
            this.InitializeComponent();

            // for testing
            this.background.Background = new SolidColorBrush(GetRandomColor());
        }

        public Color GetRandomColor() {
            Random random = new Random();
            return Color.FromArgb(255,(byte)random.Next(256), (byte)random.Next(256), (byte) random.Next(256));
        }
    }
    
}
