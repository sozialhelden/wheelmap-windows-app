using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Wheelmap.Cortana {
    public abstract class CortanaCommand {

        public virtual string CommandKey { get; }

        protected void initWindow() {
            App.Instance.InitMangers();
            App.Instance.InitWindow();
        }

        public abstract void OnHandleCommand(VoiceCommandActivatedEventArgs args);
    }
}
