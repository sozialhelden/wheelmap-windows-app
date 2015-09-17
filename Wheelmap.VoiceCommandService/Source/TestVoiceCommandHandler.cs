using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;

namespace Wheelmap.VoiceCommandService {
    class TestVoiceCommandHandler : VoiceCommandHandler {

        public override string CommandKey {
            get {
                return "search";
            }
        }

        public async override Task Handle() {

            var search = voiceCommand.Properties["dictatedSearchTerms"][0];

            await ShowProgressScreen("Suche: "+ search);
            await Task.Delay(2000);

            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = search;
            userMessage.DisplayMessage = search;
            var response = VoiceCommandResponse.CreateResponse(userMessage);

            await voiceServiceConnection.ReportSuccessAsync(response);
        }
    }
}
