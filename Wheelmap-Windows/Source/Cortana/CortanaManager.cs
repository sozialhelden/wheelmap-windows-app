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

namespace Wheelmap_Windows.Cortana {
    public class CortanaManager {

        private const string TAG = nameof(VoiceCommandManager);
        
        public static void OnActivated(VoiceCommandActivatedEventArgs args) {
            var commands = args as VoiceCommandActivatedEventArgs;
            SpeechRecognitionResult result = commands.Result;
            string voiceCommandName = result.RulePath[0];
            if (voiceCommandName == "testCommand") {
                // TODO
            }
            Log.d(TAG, "VoiceCommand: " + voiceCommandName);
            App.Instance.InitMangers();
            App.Instance.InitWindow();
            App.Instance.ShowMainPage(args);
            Window.Current.Activate();
        }

        public static async void RegisterCommands() {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Resources/Commands/Commands.xml"));
            await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(file);
        }

    }
}
