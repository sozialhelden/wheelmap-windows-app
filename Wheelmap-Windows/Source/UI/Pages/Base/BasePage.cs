using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Utils;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap_Windows.UI.Pages.Base {
    public class BasePage : Page, BackDelegate {

        public BasePage() {
        }

        public virtual string Title {
            get {
                return null;
            }  
        }

        public virtual bool CanGoBack() {
            return false;
        }

        public virtual void GoBack() {
        }
        
    }
}
