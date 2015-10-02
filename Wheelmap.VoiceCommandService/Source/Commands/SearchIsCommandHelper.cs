using System.Threading.Tasks;
using Wheelmap.Utils;

namespace Wheelmap.VoiceCommandService {
    class SearchIsCommandHelper : VoiceCommandHandler {
        public override string[] CommandKey {
            get {
                return new string[] { Constants.Cortana.Command.SEARCH };
            }
        }
        
        public override async Task Handle() {
            string search = voiceCommand.Properties[Constants.Cortana.PhraseList.SEARCH][0];
            
            await LaunchAppInForeground(null, new WheelmapParams {
                Search = search
            }.ToString());

        }
        
    }
}
