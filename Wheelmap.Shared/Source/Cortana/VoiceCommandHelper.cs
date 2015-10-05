using System;
using System.Collections.Generic;
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
        /// 
        /// not used at the moment due to Constants.Cortana.PhraseList.NODE_TYPES is not used
        /// </summary>
        /// <param name="categories"></param>
        public static async Task UpdateNodeTypePhraseList(IList<Model.NodeType> categories) {
            await Task.Delay(1);

            List<string> phraseList = new List<string>();
            foreach (var c in categories) {
                phraseList.Add(c.localizedName);

                var parts = c.localizedName.Split('/');
                phraseList.AddAll(parts);

            }

            var toiletWords = "TOILET_SYNONYM".t(R.File.CORTANA);
            var toiletSynonyms = toiletWords.Split('/');
            phraseList.AddAll(toiletSynonyms);

            Log.d(phraseList);

            var commands = VoiceCommandDefinitionManager.InstalledCommandDefinitions;
            foreach (var command in commands) {
                await command.Value.SetPhraseListAsync(Constants.Cortana.PhraseList.NODE_TYPES, phraseList);
            }
            
        }
    }
    
}
