using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Wheelmap.Cortana {
    public class DefaultCommand : CortanaCommand {

        public override string CommandKey {
            get {
                return null;
            }
        }

        public override void OnHandleCommand(VoiceCommandActivatedEventArgs args) {
            initWindow();
            App.Instance.ShowMainPage(args);
            Window.Current.Activate();
        }
    }
}
