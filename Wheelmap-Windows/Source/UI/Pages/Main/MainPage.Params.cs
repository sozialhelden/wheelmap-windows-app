using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Extensions;
using Wheelmap.Model;
using Wheelmap.Source.UI.Pages.Node;
using Wheelmap.UI.Pages.Base;
using Wheelmap.Utils;
using Wheelmap.Utils.Eventbus;
using Wheelmap.Utils.Eventbus.Events;

namespace Wheelmap.Source.UI {
    public sealed partial class MainPage : BasePage {
        public override void OnNewParams(object args) {
            base.OnNewParams(args);
            
            if (args is WheelmapParams) {
                WheelmapParams whParams = args as WheelmapParams;

                // wait for the rest of the ui setup
                Dispatcher.PostDelayed(()=> initWithWheelmapParams(whParams), 2000);
                
            }

        }

        private void initWithWheelmapParams(WheelmapParams args) {
            if (args.Search?.Length > 0) {
                ShowListTapped(MenuShowListItem, null);
                DataHolder.Instance.QueryString = args.Search;
            }
            if (args.ShowDetailsFromId > 0) {
                var node = Database.Instance.Table<Node>().Where(x => x.wm_id == args.ShowDetailsFromId).FirstOrDefault();
                if (node != null) {
                    SelectedNodeChangedEvent e = new SelectedNodeChangedEvent();
                    e.node = node;
                    BusProvider.DefaultInstance.Post(e);
                }
            }

            if (args.Path == WheelmapParams.PATH_HELP) {
                ShowHelpTapped(MenuShowHelpItem, null);
            }
        }
    }
}
