using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Extensions;
using Wheelmap.Utils;
using Windows.ApplicationModel.VoiceCommands;

namespace Wheelmap.Cortana {
    public static class VoiceCommandHelper {

        private const string TAG = nameof(VoiceCommandHelper);

        /// <summary>
        /// fill the phraselist for nodeType to use them in voice commands
        /// also add words for toilet/wc if not already exists in NodeTypes
        /// </summary>
        /// <param name="categories"></param>
        public static async void UpdateNodeTypePhraseList(IList<Model.NodeType> categories) {


            List<string> phraseList = new List<string>();
            foreach (var c in categories) {
                phraseList.Add(c.localizedName);

                var parts = c.localizedName.Split('/');
                phraseList.AddAll(parts);

            }

            var toiletWords = "TOILET_SYNONYM".t(R.File.CORTANA);
            var toiletSynonyms = toiletWords.Split('/');
            phraseList.AddAll(toiletSynonyms);

            Log.d(TAG, phraseList);           

            foreach (var command in VoiceCommandDefinitionManager.InstalledCommandDefinitions) {
                await command.Value.SetPhraseListAsync(Constants.Cortana.PhraseList.NODE_TYPES, phraseList);
            }
            
        }
    }
    
}
