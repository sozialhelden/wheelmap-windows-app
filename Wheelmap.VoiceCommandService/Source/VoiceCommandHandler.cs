using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Model;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.VoiceCommands;

namespace Wheelmap.VoiceCommandService {
    abstract class VoiceCommandHandler {

        public abstract string CommandKey { get; }

        public WheelmapVoiceCommandService service;
        public VoiceCommand voiceCommand;
        public VoiceCommandServiceConnection voiceServiceConnection {
            get {
                return service.GetVoiceServiceConnection();
            }
        }

        protected ResourceContext context;

        public void SetArgs(WheelmapVoiceCommandService service, VoiceCommand command) {
            this.service = service;
            this.voiceCommand = command;
            context = ResourceContext.GetForViewIndependentUse();
        }

        public abstract Task Handle();
        
        /// <summary>
        /// Show a progress screen. These should be posted at least every 5 seconds for a 
        /// long-running operation, such as accessing network resources over a mobile 
        /// carrier network.
        /// </summary>
        /// <param name="message">The message to display, relating to the task being performed.</param>
        /// <returns></returns>
        protected async Task ShowProgressScreen(string message) {
            var userProgressMessage = new VoiceCommandUserMessage();
            userProgressMessage.DisplayMessage = userProgressMessage.SpokenMessage = message;

            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await voiceServiceConnection.ReportProgressAsync(response);
        }
        
        /// <summary>
        /// Provide a simple response that launches the app. Expected to be used in the
        /// case where the voice command could not be recognized (eg, a VCD/code mismatch.)
        /// </summary>
        protected async void LaunchAppInForeground(VoiceCommandUserMessage userMessage = null, string args = null) {
            if (userMessage == null) {
                userMessage = new VoiceCommandUserMessage();
                // TODO
                userMessage.SpokenMessage = "Öffne App";
                userMessage.DisplayMessage = "Öffne App";
            }
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            response.AppLaunchArgument = args ?? "";
            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }


        public async Task<Node> AskUserForNode(ICollection<Node> nodes) {

            if (nodes == null) {
                return null;
            }
            
            if (nodes.Count() == 1) {
                return nodes.First();
            }

            var contentTiles = new List<VoiceCommandContentTile>();

            int i = 1;
            foreach (var node in nodes) {
                var tile = new VoiceCommandContentTile();

                // To handle UI scaling, Cortana automatically looks up files with FileName.scale-<n>.ext formats based on the requested filename.
                // See the VoiceCommandService\Images folder for an example.
                tile.ContentTileType = VoiceCommandContentTileType.TitleWithText;

                tile.AppContext = node;
                //tile.Image = await StorageFile.GetFileFromApplicationUriAsync((node.MapIconFileUri));
                //tile.AppLaunchArgument = node.wm_id.ToString(CultureInfo.InvariantCulture);
                tile.Title = i + ": " + node.name;
                tile.TextLine1 = Stati.From(node.wheelchairStatus).GetLocalizedMessage(context);
                tile.TextLine2 = node.DistanceString;
                contentTiles.Add(tile);
                i++;
            }

            // TODO 
            var userPrompt = new VoiceCommandUserMessage();
            userPrompt.DisplayMessage =
                userPrompt.SpokenMessage = "Suche aus";

            var userPrompt2 = new VoiceCommandUserMessage();
            userPrompt2.DisplayMessage =
                userPrompt2.SpokenMessage = "Suche aus 2";

            var response = VoiceCommandResponse.CreateResponseForPrompt(userPrompt, userPrompt2, contentTiles);
            var voiceCommandDisambiguationResult = await voiceServiceConnection.RequestDisambiguationAsync(response);
            if (voiceCommandDisambiguationResult != null) {
                return (Node) voiceCommandDisambiguationResult.SelectedItem.AppContext;
            }
            return null;
        }

    }
}
