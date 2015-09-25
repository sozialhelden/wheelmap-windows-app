using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wheelmap.Model;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.VoiceCommands;
using Wheelmap.Utils;
using Wheelmap.Extensions;

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
        protected async Task LaunchAppInForeground(VoiceCommandUserMessage userMessage = null, string args = null) {
            if (userMessage == null) {
                userMessage = new VoiceCommandUserMessage();
                userMessage.SpokenMessage = "OPEN_APP_SpokenMessage".t(context, R.File.CORTANA);
                userMessage.DisplayMessage = "OPEN_APP_DisplayMessage".t(context, R.File.CORTANA);
            }
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            response.AppLaunchArgument = args ?? "";
            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }


        public async Task<Node> AskUserForNode(List<Node> nodes, bool isToiletSearch) {

            if (nodes == null || nodes.Count() == 0) {
                return null;
            }
            
            if (nodes.Count() == 1) {
                return nodes.First();
            }
            
            if (nodes.Count() > 10) {
                nodes = nodes.GetRange(0,10);
            }

            var contentTiles = new List<VoiceCommandContentTile>();

            int i = 1;
            foreach (var node in nodes) {
                var tile = new VoiceCommandContentTile();
                
                tile.ContentTileType = VoiceCommandContentTileType.TitleWithText;

                tile.AppContext = node;
                tile.Title = i + ": " + node.name;
                
                if (isToiletSearch && node.wheelchairToiletStatus == "yes") {
                    // on toilet search there can only be node with the toiletstatus yes
                    tile.TextLine1 = Stati.From(node.wheelchairToiletStatus).GetLocalizedToiletMessage(context);
                } else {
                    // the node was not choosen because its toilet status
                    // this can also be true if the node is of NodeType Toilet but has no toiletstatus rating
                    tile.TextLine1 = Stati.From(node.wheelchairStatus).GetLocalizedMessage(context);
                }

                tile.TextLine2 = node.DistanceString;
                contentTiles.Add(tile);
                i++;
            }
            
            var userPrompt = new VoiceCommandUserMessage();
            userPrompt.SpokenMessage = "CHOOSE_A_PLACE_SpokenMessage_1".t(context, R.File.CORTANA);
            userPrompt.DisplayMessage = "CHOOSE_A_PLACE_DisplayMessage_1".t(context, R.File.CORTANA);

            var userPrompt2 = new VoiceCommandUserMessage();
            userPrompt2.SpokenMessage = "CHOOSE_A_PLACE_SpokenMessage_2".t(context, R.File.CORTANA);
            userPrompt2.DisplayMessage = "CHOOSE_A_PLACE_DisplayMessage_2".t(context, R.File.CORTANA);

            var response = VoiceCommandResponse.CreateResponseForPrompt(userPrompt, userPrompt2, contentTiles);
            response.AppLaunchArgument = new WheelmapParams().ToString();
            var voiceCommandDisambiguationResult = await voiceServiceConnection.RequestDisambiguationAsync(response);
            if (voiceCommandDisambiguationResult != null) {
                return (Node) voiceCommandDisambiguationResult.SelectedItem.AppContext;
            }
            return null;
        }

    }
}
