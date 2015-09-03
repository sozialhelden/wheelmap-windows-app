using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Wheelmap_Windows.Extensions {
    public static class ViewExtensions {

        public static void HideVerticalScrollBarsIfContentFits(this ScrollViewer scrollViewer) {
            if (scrollViewer.ScrollableHeight < scrollViewer.ViewportHeight) {
                ScrollViewer.SetVerticalScrollBarVisibility(scrollViewer, ScrollBarVisibility.Disabled);
            } else {
                ScrollViewer.SetVerticalScrollBarVisibility(scrollViewer, ScrollBarVisibility.Auto);
            }
        }

    }
}
