using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Wheelmap.Utils;
using Windows.UI.Xaml;
using Windows.Foundation;

namespace Wheelmap.Source.UI.Controls {
    
    /// <summary>
    /// Container which pass its FlowDirection property to all images which are direct children
    /// </summary>
    public class InheritDirectionFlowImageContainer : Grid {
        public InheritDirectionFlowImageContainer() {
            this.WatchProperty(nameof(FlowDirection), flowDirection_Changed);
        }

        protected override Size MeasureOverride(Size availableSize) {
            flowDirection_Changed(null, null);
            return base.MeasureOverride(availableSize);
        }

        private void flowDirection_Changed(object sender, DependencyPropertyChangedEventArgs e) {
            foreach (var child in this.Children) {
                if (child is Image) {
                    var image = (Image) child;
                    image.FlowDirection = this.FlowDirection;
                }
            }
        }
    }
}
