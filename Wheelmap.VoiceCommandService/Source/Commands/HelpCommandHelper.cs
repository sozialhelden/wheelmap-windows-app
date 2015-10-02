using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Utils;

namespace Wheelmap.VoiceCommandService {
    class HelpCommandHelper : VoiceCommandHandler {
        public override string[] CommandKey {
            get {
                return new string[] { Constants.Cortana.Command.HELP };
            }
        }

        public override async Task Handle() {
            await LaunchAppInForeground(null, new WheelmapParams {
                Path = WheelmapParams.PATH_HELP
            }.ToString());
        }
    }
}
