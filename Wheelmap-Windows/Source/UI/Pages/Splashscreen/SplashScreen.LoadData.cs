using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Api.Calls;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.Utils;

namespace Wheelmap_Windows.Source.UI.Pages.Splashscreen {
    public sealed partial class ExtendedSplashPage {
        public async Task<bool> LoadData() {

            var categories = await new CategoryRequest().Query();

            DataHolder.Instance.Categories.Clear();
            foreach (Category c in categories) {
                DataHolder.Instance.Categories.Add(c.identifier, c);
            }
            
            return await new IconDownloadRequest().Query();

        }
    }
}
