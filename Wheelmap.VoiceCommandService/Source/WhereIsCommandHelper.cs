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
    class WhereIsCommandHelper : VoiceCommandHandler {
        public override string CommandKey {
            get {
                return "whereIs";
            }
        }
        
        public override async Task Handle() {
            string nodeTypeString = voiceCommand.Properties[Constants.Cortana.PhraseList.NODE_TYPES][0];

            await ShowProgressScreen("Suche: " + nodeTypeString);

            var nodeType = Database.Instance.Table<NodeType>().Where(x => x.localizedName == nodeTypeString).First();

            if (nodeType == null) {
                reportError();
                return;
            }

            var access = await Geolocator.RequestAccessAsync();
            if (access != GeolocationAccessStatus.Allowed) {
                reportErrorNoLocationAccess();
                return;
            }

            var point = (await new Geolocator().GetGeopositionAsync()).Coordinate.Point;
            var bbox = GeoMath.GetBoundingBox(point.Position, 2);

            List<Node> nodes = await new NodesRequest(bbox).Execute();

            if (nodes?.Count() <= 0) {
                reportErrorNotFound();
                return;
            }

            nodes = nodes.Where(x => {
                return x.nodeType.Id == nodeType.Id;
            }).ToList();

            nodes = Nodes.OrderItemsByDistance(nodes, point).ToList();

            var node = await AskUserForNode(nodes);
            Log.d(this, "Selected Node: " + node);

            if (node == null) {
                reportErrorNotFound();
                return;
            }

            LaunchAppInForeground(null, new WheelmapParams {
                ShowDetailsFromId = node.wm_id
            }.ToString());
            
        }
        
        private async void reportErrorNotFound() {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "Kein Ort gefunden";
            userMessage.DisplayMessage = "Kein Ort gefunden";
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            await voiceServiceConnection.ReportSuccessAsync(response);
        }

        private async void reportErrorNoLocationAccess() {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "Kein Location Zugriff";
            userMessage.DisplayMessage = "Kein Location Zugriff";
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            await voiceServiceConnection.ReportSuccessAsync(response);
        }

        private async void reportError() {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "Nicht gefunden";
            userMessage.DisplayMessage = "Nicht gefunden";
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            await voiceServiceConnection.ReportSuccessAsync(response);
        }
    }
}
