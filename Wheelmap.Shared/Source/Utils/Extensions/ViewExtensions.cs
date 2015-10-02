using Windows.UI.Xaml.Controls;

namespace Wheelmap.Extensions {
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
