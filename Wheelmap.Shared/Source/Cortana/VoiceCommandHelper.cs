using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Utils;
using Windows.ApplicationModel.VoiceCommands;

namespace Wheelmap.Cortana {
    public static class VoiceCommandHelper {
        public static async void UpdateNodeTypePhraseList(IList<Model.NodeType> categories) {
            
            List<string> phraseList = new List<string>();
            foreach (var c in categories) {
                phraseList.Add(c.localizedName);
            }

            foreach (var command in VoiceCommandDefinitionManager.InstalledCommandDefinitions) {
                await command.Value.SetPhraseListAsync(Constants.Cortana.PhraseList.NODE_TYPES, phraseList);
            }
            
        }
    }
    
}
