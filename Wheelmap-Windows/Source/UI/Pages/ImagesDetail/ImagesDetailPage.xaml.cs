using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap.Source.UI.Pages.ImagesDetail {

    public sealed partial class ImagesDetailPage : Page {

        List<Photo> photos;

        public ImagesDetailPage() {
            this.InitializeComponent();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            var args = e.Parameter as ImagesDetailArguments;
            photos = args.photos;

            for (int i=0;i<photos.Count();i++) {
                var title = (i + 1) + "/" + photos.Count();
                mFlipView.Items.Add(new PivotDataHolder(title, photos[i]));
            }

            mFlipView.SelectedIndex = args.selectedItem;
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            currentPage.Text = (mFlipView.SelectedIndex + 1) + "/" + photos.Count();
        }
    }

    public class ImagesDetailArguments {
        public int selectedItem;
        public List<Photo> photos;
    }

    class PivotDataHolder {

        private string title;
        private Photo photo;
        public Model.Image Image {
            get {
                return photo.GetSource();
            }
        }
        
        public PivotDataHolder(string title, Photo photo) {
            this.title = title;
            this.photo = photo;
        }

        public override string ToString() {
            return title;
        }
    }
}
