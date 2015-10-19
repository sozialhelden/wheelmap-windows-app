using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Wheelmap.Cortana {
    public class CortanaManager {

        private const string TAG = nameof(VoiceCommandManager);
        private static IDictionary<string, CortanaCommand> Commands = new Dictionary<string, CortanaCommand>();
        
        static CortanaManager() {
            // TODO add commands
        }

        private static void AddCommand(CortanaCommand command) {
            Commands.Add(command.CommandKey, command);
        }

        public static void OnActivated(VoiceCommandActivatedEventArgs args) {
            var commands = args as VoiceCommandActivatedEventArgs;
            SpeechRecognitionResult result = commands.Result;
            string voiceCommandName = result.RulePath[0];

            CortanaCommand command;
            if (Commands.ContainsKey(voiceCommandName)) {
                command = Commands[voiceCommandName];
            } else {
                // get default command
                command = new DefaultCommand();
            }

            command.OnHandleCommand(args);

        }

        public static async void RegisterCommands() {
            try {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Resources/Commands/Commands.xml"));
                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(file);
            } catch (Exception e) {
                // just in case the cortana guidelines changed
                Log.e(e);
            }
        }
        
    }
}
