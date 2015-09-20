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

            bool isToiletSearch = IsToiletSynonym(nodeTypeString);
            var nodeTypes = Database.Instance.Table<NodeType>().Where(x => x.localizedName.Contains(nodeTypeString)).ToList();

            if ((nodeTypes == null || nodeTypes.Count() == 0) && !isToiletSearch) {
                reportErrorNotFound(false);
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
                reportErrorNotFound(isToiletSearch);
                return;
            }

            nodes = nodes.Where(x => {
                return (nodeTypes.Any(y => y.Id == x.nodeType.Id)) ||
                (isToiletSearch && x.wheelchairToiletStatus == "yes");
            }).ToList();

            nodes = Nodes.OrderItemsByDistance(nodes, point).ToList();

            var node = await AskUserForNode(nodes, isToiletSearch);
            Log.d(this, "Selected Node: " + node);

            if (node == null) {
                reportErrorNotFound(isToiletSearch);
                return;
            }

            LaunchAppInForeground(null, new WheelmapParams {
                ShowDetailsFromId = node.wm_id
            }.ToString());
            
        }

        private bool IsToiletSynonym(string word) {
            var synonyms = "TOILET_SYNONYM".t(context, R.File.CORTANA).Split('/').ToList();
            return synonyms.Contains(word);
        }
        
        private async void reportErrorNotFound(bool isToiletSearch) {

            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = isToiletSearch 
                ? "ERROR_TOILET_NOT_FOUNT_SpokenMessage".t(context, R.File.CORTANA) 
                : "ERROR_PLACE_NOT_FOUNT_SpokenMessage".t(context, R.File.CORTANA);
            userMessage.DisplayMessage = isToiletSearch
                ? "ERROR_TOILET_NOT_FOUNT_DisplayMessage".t(context, R.File.CORTANA)
                : "ERROR_PLACE_NOT_FOUNT_DisplayMessage".t(context, R.File.CORTANA);

            var response = VoiceCommandResponse.CreateResponse(userMessage);
            await voiceServiceConnection.ReportSuccessAsync(response);
        }

        private async void reportErrorNoLocationAccess() {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "ERROR_NO_LOCTION_ACCESS_SpokenMessage".t(context, R.File.CORTANA);
            userMessage.DisplayMessage = "ERROR_NO_LOCTION_ACCESS_DisplayMessage".t(context, R.File.CORTANA);
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            await voiceServiceConnection.ReportSuccessAsync(response);
        }
    }
}
