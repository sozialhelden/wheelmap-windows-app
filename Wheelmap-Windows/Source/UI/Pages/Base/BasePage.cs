﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Utils;
using Wheelmap_Windows.Utils.Eventbus;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap_Windows.UI.Pages.Base {
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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            Parameter = e.Parameter;
            OnNewParams(Parameter);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
            Log.d(this, "OnNavigatedFrom: " + e);
            base.OnNavigatingFrom(e);
            BusProvider.DefaultInstance.Unregister(this);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            Log.d(this, "OnNavigatedFrom: " + e);
            base.OnNavigatedFrom(e);
            BusProvider.DefaultInstance.Unregister(this);
        }
        
        public virtual void OnNewParams(object args) {
            Parameter = args;
        }
    }
}
