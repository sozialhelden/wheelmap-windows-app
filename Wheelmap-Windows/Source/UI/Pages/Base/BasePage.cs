using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Source.UI;
using Wheelmap.Utils;
using Wheelmap.Utils.Eventbus;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap.UI.Pages.Base {
    public class BasePage : Page, BackDelegate {

        public object Parameter;

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
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.Content is BackDelegate && rootFrame.Content != this) {
                (rootFrame.Content as BackDelegate).GoBack();
            }
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            Parameter = e.Parameter;
            OnNewParams(Parameter);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
            Log.d("OnNavigatedFrom: " + e);
            base.OnNavigatingFrom(e);
            BusProvider.DefaultInstance.Unregister(this);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            Log.d("OnNavigatedFrom: " + e);
            base.OnNavigatedFrom(e);
            BusProvider.DefaultInstance.Unregister(this);
        }
        
        public virtual void OnNewParams(object args) {
            Parameter = args;
        }

        public virtual void ShowOnDetailFrame(Type type, object args = null) {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.Content is MainPage) {
                (rootFrame.Content as MainPage).ShowOnDetailFrame(type, args);
            }
        }
        
        public virtual PageShowOn GetShowOnSmall() {
            if (Parameter is BasePageArguments) {
                return (Parameter as BasePageArguments).ShowOnSmall;
            }
            return PageShowOn.DO_NOT_CHANGE;
        }

        public virtual PageShowOn GetShowOnBig() {
            if (Parameter is BasePageArguments) {
                return (Parameter as BasePageArguments).ShowOnBig;
            }
            return PageShowOn.DO_NOT_CHANGE;
        }

    }

    public class BasePageArguments {
        public PageShowOn ShowOnSmall = PageShowOn.DO_NOT_CHANGE;
        public PageShowOn ShowOnBig = PageShowOn.DO_NOT_CHANGE;

        public override bool Equals(object obj) {
            if (obj is BasePageArguments) {
                var o = (BasePageArguments)obj;
                return ShowOnSmall == o.ShowOnSmall && ShowOnBig == o.ShowOnBig;
            }
            return false;
        }
        public override int GetHashCode() {
            return (ShowOnBig + "" + ShowOnSmall).GetHashCode();
        }
    }

    public enum PageShowOn {
        // page should be shown on detailContainer
        DETAIL,
        // page should be shown on menuContainer
        MENU,
        // the page should stay on the page it was before
        DO_NOT_CHANGE,
        // the page should disappear
        NONE
    }
}
