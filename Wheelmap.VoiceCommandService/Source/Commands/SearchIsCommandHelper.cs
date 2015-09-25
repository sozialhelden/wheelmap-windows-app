using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Api.Calls;
using Wheelmap.Extensions;
using Wheelmap.Model;
using Wheelmap.Utils;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace Wheelmap.VoiceCommandService {
    class SearchIsCommandHelper : VoiceCommandHandler {
        public override string CommandKey {
            get {
                return "search";
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
