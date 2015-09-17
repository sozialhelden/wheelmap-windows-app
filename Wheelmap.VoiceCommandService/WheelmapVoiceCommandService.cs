
//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

namespace Wheelmap.VoiceCommandService {
    public sealed class WheelmapVoiceCommandService : IBackgroundTask {

        /// <summary>
        /// the service connection is maintained for the lifetime of a cortana session, once a voice command
        /// has been triggered via Cortana.
        /// </summary>
        VoiceCommandServiceConnection voiceServiceConnection;

        /// <summary>
        /// Lifetime of the background service is controlled via the BackgroundTaskDeferral object, including
        /// registering for cancellation events, signalling end of execution, etc. Cortana may terminate the 
        /// background service task if it loses focus, or the background task takes too long to provide.
        /// 
        /// Background tasks can run for a maximum of 30 seconds.
        /// </summary>
        BackgroundTaskDeferral serviceDeferral;

        /// <summary>
        /// ResourceMap containing localized strings for display in Cortana.
        /// </summary>
        ResourceMap cortanaResourceMap;

        /// <summary>
        /// The context for localized strings.
        /// </summary>
        ResourceContext cortanaContext;

        /// <summary>
        /// Get globalization-aware date formats.
        /// </summary>
        DateTimeFormatInfo dateFormatInfo;

        /// <summary>
        /// Background task entrypoint. Voice Commands using the <VoiceCommandService Target="...">
        /// tag will invoke this when they are recognized by Cortana, passing along details of the 
        /// invocation. 
        /// 
        /// Background tasks must respond to activation by Cortana within 0.5 seconds, and must 
        /// report progress to Cortana every 5 seconds (unless Cortana is waiting for user
        /// input). There is no execution time limit on the background task managed by Cortana,
        /// but developers should use plmdebug (https://msdn.microsoft.com/en-us/library/windows/hardware/jj680085%28v=vs.85%29.aspx)
        /// on the Cortana app package in order to prevent Cortana timing out the task during
        /// debugging.
        /// 
        /// Cortana dismisses its UI if it loses focus. This will cause it to terminate the background
        /// task, even if the background task is being debugged. Use of Remote Debugging is recommended
        /// in order to debug background task behaviors. In order to debug background tasks, open the
        /// project properties for the app package (not the background task project), and enable
        /// Debug -> "Do not launch, but debug my code when it starts". Alternatively, add a long
        /// initial progress screen, and attach to the background task process while it executes.
        /// </summary>
        /// <param name="taskInstance">Connection to the hosting background service process.</param>
        public async void Run(IBackgroundTaskInstance taskInstance) {
            serviceDeferral = taskInstance.GetDeferral();
            
            // Register to receive an event if Cortana dismisses the background task. This will
            // occur if the task takes too long to respond, or if Cortana's UI is dismissed.
            // Any pending operations should be cancelled or waited on to clean up where possible.
            taskInstance.Canceled += OnTaskCanceled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            // Load localized resources for strings sent to Cortana to be displayed to the user.
            cortanaResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");

            // Select the system language, which is what Cortana should be running as.
            cortanaContext = ResourceContext.GetForViewIndependentUse();

            // Get the currently used system date format
            dateFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;

            // This should match the uap:AppService and VoiceCommandService references from the 
            // package manifest and VCD files, respectively. Make sure we've been launched by
            // a Cortana Voice Command.
            if (triggerDetails != null) {
                try {
                    voiceServiceConnection =
                        VoiceCommandServiceConnection.FromAppServiceTriggerDetails(
                            triggerDetails);

                    voiceServiceConnection.VoiceCommandCompleted += OnVoiceCommandCompleted;

                    VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                    // Depending on the operation (defined in AdventureWorks:AdventureWorksCommands.xml)
                    // perform the appropriate command.
                    switch (voiceCommand.CommandName) {
                        case "whenIsTripToDestination":
                            var destination = voiceCommand.Properties["destination"][0];
                            await SendCompletionMessageForDestination(destination);
                            break;
                        case "cancelTripToDestination":
                            var cancelDestination = voiceCommand.Properties["destination"][0];
                            await SendCompletionMessageForCancellation(cancelDestination);
                            break;
                        case "search":
                            var a = voiceCommand.Properties["dictatedSearchTerms"][0];
                            await SendCompletionMessageForDestination(a);
                            break;
                        default:
                            // As with app activation VCDs, we need to handle the possibility that
                            // an app update may remove a voice command that is still registered.
                            // This can happen if the user hasn't run an app since an update.
                            LaunchAppInForeground();
                            break;
                    }
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine("Handling Voice Command failed " + ex.ToString());
                    System.Diagnostics.Debug.WriteLine("Handling Voice Command failed " + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Handle the Trip Cancellation task. This task demonstrates how to prompt a user
        /// for confirmation of an operation, show users a progress screen while performing
        /// a long-running task, and showing a completion screen.
        /// </summary>
        /// <param name="destination">The name of a destination, expected to match the phrase list.</param>
        /// <returns></returns>
        private async Task SendCompletionMessageForCancellation(string destination) {
            // Begin loading data to search for the target store. If this operation is going to take a long time,
            // for instance, requiring a response from a remote web service, consider inserting a progress screen 
            // here, in order to prevent Cortana from timing out. 
            string progressScreenString = string.Format(
                cortanaResourceMap.GetValue("ProgressLookingForTripToDest", cortanaContext).ValueAsString,
                destination);
            await ShowProgressScreen(progressScreenString);

            List<KeyValuePair<string, string>> trips = new List<KeyValuePair<string, string>>();
            trips.Add(new KeyValuePair<string, string>("key", "value"));
            trips.Add(new KeyValuePair<string, string>("key2", "value2"));
            trips.Add(new KeyValuePair<string, string>("key3", "value3"));

            // We might have multiple trips to the destination. For now, we just pick the first.
            KeyValuePair<string, string>? trip = null;
            if (trips.Count() > 1) {
                // If there is more than one trip, provide a disambiguation screen rather than just picking one
                // however, more advanced logic here might be ideal (ie, if there's a significant number of items,
                // you may want to just fall back to a link to your app where you can provide a deeper search experience.
                string disambiguationDestinationString = string.Format(
                    cortanaResourceMap.GetValue("DisambiguationWhichTripToDest", cortanaContext).ValueAsString,
                    destination);
                string disambiguationRepeatString = cortanaResourceMap.GetValue("DisambiguationRepeat", cortanaContext).ValueAsString;
                trip = await DisambiguateTrips(trips, disambiguationDestinationString, disambiguationRepeatString);
            } else {
                // One or no trips exist with that destination, so retrieve it, or return null.
                trip = trips.FirstOrDefault();
            }

            var userPrompt = new VoiceCommandUserMessage();

            VoiceCommandResponse response;
            if (trip == null) {
                var userMessage = new VoiceCommandUserMessage();
                // In this scenario, perhaps someone has modified data on your service outside of this 
                // apps control. If you're accessing a remote service, having a background task that
                // periodically refreshes the phrase list so it's likely to be in sync is ideal.
                // This is unlikely to occur for this sample app, however.
                string noSuchTripToDestination = string.Format(
                    cortanaResourceMap.GetValue("NoSuchTripToDestination", cortanaContext).ValueAsString,
                    destination);
                userMessage.DisplayMessage = userMessage.SpokenMessage = noSuchTripToDestination;

                response = VoiceCommandResponse.CreateResponse(userMessage);
                await voiceServiceConnection.ReportSuccessAsync(response);
            } else {
                // Prompt the user for confirmation that we've selected the correct trip to cancel.
                string cancelTripToDestination = string.Format(
                    cortanaResourceMap.GetValue("CancelTripToDestination", cortanaContext).ValueAsString,
                    destination);
                userPrompt.DisplayMessage = userPrompt.SpokenMessage = cancelTripToDestination;
                var userReprompt = new VoiceCommandUserMessage();
                string confirmCancelTripToDestination = string.Format(
                    cortanaResourceMap.GetValue("ConfirmCancelTripToDestination", cortanaContext).ValueAsString,
                    destination);
                userReprompt.DisplayMessage = userReprompt.SpokenMessage = confirmCancelTripToDestination;

                response = VoiceCommandResponse.CreateResponseForPrompt(userPrompt, userReprompt);

                var voiceCommandConfirmation = await voiceServiceConnection.RequestConfirmationAsync(response);

                // If RequestConfirmationAsync returns null, Cortana's UI has likely been dismissed.
                if (voiceCommandConfirmation != null) {
                    if (voiceCommandConfirmation.Confirmed == true) {
                        string cancellingTripToDestination = string.Format(
                       cortanaResourceMap.GetValue("CancellingTripToDestination", cortanaContext).ValueAsString,
                       destination);
                        await ShowProgressScreen(cancellingTripToDestination);

                        // Provide a completion message to the user.
                        var userMessage = new VoiceCommandUserMessage();
                        string cancelledTripToDestination = string.Format(
                            cortanaResourceMap.GetValue("CancelledTripToDestination", cortanaContext).ValueAsString,
                            destination);
                        userMessage.DisplayMessage = userMessage.SpokenMessage = cancelledTripToDestination;
                        response = VoiceCommandResponse.CreateResponse(userMessage);
                        await voiceServiceConnection.ReportSuccessAsync(response);
                    } else {
                        // Confirm no action for the user.
                        var userMessage = new VoiceCommandUserMessage();
                        string keepingTripToDestination = string.Format(
                            cortanaResourceMap.GetValue("KeepingTripToDestination", cortanaContext).ValueAsString,
                            destination);
                        userMessage.DisplayMessage = userMessage.SpokenMessage = keepingTripToDestination;

                        response = VoiceCommandResponse.CreateResponse(userMessage);
                        await voiceServiceConnection.ReportSuccessAsync(response);
                    }
                }
            }
        }

        /// <summary>
        /// Show a progress screen. These should be posted at least every 5 seconds for a 
        /// long-running operation, such as accessing network resources over a mobile 
        /// carrier network.
        /// </summary>
        /// <param name="message">The message to display, relating to the task being performed.</param>
        /// <returns></returns>
        private async Task ShowProgressScreen(string message) {
            var userProgressMessage = new VoiceCommandUserMessage();
            userProgressMessage.DisplayMessage = userProgressMessage.SpokenMessage = message;

            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await voiceServiceConnection.ReportProgressAsync(response);
        }

        /// <summary>
        /// Demonstrates providing the user with a choice between multiple items. In this case, if a user
        /// has two trips to the same destination with different dates, this will provide a way to differentiate
        /// them. Provide a way to choose between the items.
        /// </summary>
        /// <param name="trips">The set of trips to choose between</param>
        /// <param name="disambiguationMessage">The initial disambiguation message</param>
        /// <param name="secondDisambiguationMessage">Repeat prompt retry message</param>
        /// <returns></returns>
        private async Task<KeyValuePair<string, string>> DisambiguateTrips(IEnumerable<KeyValuePair<string, string>> trips, string disambiguationMessage, string secondDisambiguationMessage) {
            // Create the first prompt message.
            var userPrompt = new VoiceCommandUserMessage();
            userPrompt.DisplayMessage =
                userPrompt.SpokenMessage = disambiguationMessage;

            // Create a re-prompt message if the user responds with an out-of-grammar response.
            var userReprompt = new VoiceCommandUserMessage();
            userReprompt.DisplayMessage =
                userReprompt.SpokenMessage = secondDisambiguationMessage;

            // Create items for each item. Ideally, should be limited to a small number of items.
            var destinationContentTiles = new List<VoiceCommandContentTile>();
            int i = 1;
            foreach (KeyValuePair<string, string> trip in trips) {
                var destinationTile = new VoiceCommandContentTile();

                // To handle UI scaling, Cortana automatically looks up files with FileName.scale-<n>.ext formats based on the requested filename.
                // See the VoiceCommandService\Images folder for an example.
                destinationTile.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                destinationTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///AdventureWorks.VoiceCommands/Images/GreyTile.png"));

                // The AppContext can be any arbitrary object, and will be maintained for the
                // response.
                destinationTile.AppContext = trip;

                destinationTile.Title = trip.Value + " ";
                destinationTile.TextLine1 = trip.Value;

                destinationContentTiles.Add(destinationTile);
                i++;
            }

            // Cortana will handle re-prompting if the user does not provide a valid response.
            var response = VoiceCommandResponse.CreateResponseForPrompt(userPrompt, userReprompt, destinationContentTiles);

            // If cortana is dismissed in this operation, null will be returned.
            var voiceCommandDisambiguationResult = await
                voiceServiceConnection.RequestDisambiguationAsync(response);
            if (voiceCommandDisambiguationResult != null) {
                return (KeyValuePair<string, string>)voiceCommandDisambiguationResult.SelectedItem.AppContext;
            }

            return new KeyValuePair<string, string>();
        }

        /// <summary>
        /// Search for, and show details related to a single trip, if the trip can be
        /// found. This demonstrates a simple response flow in Cortana.
        /// </summary>
        /// <param name="destination">The destination, expected to be in the phrase list.</param>
        /// <returns></returns>
        private async Task SendCompletionMessageForDestination(string destination) {
            // If this operation is expected to take longer than 0.5 seconds, the task must
            // provide a progress response to Cortana prior to starting the operation, and
            // provide updates at most every 5 seconds.
            string loadingTripToDestination = string.Format(
                       cortanaResourceMap.GetValue("LoadingTripToDestination", cortanaContext).ValueAsString,
                       destination);
            await ShowProgressScreen(loadingTripToDestination);

            // Look for the specified trip. The destination *should* be pulled from the grammar we
            // provided, and the subsequently updated phrase list, so it should be a 1:1 match, including case.
            // However, we might have multiple trips to the destination. For now, we just pick the first.

            List<KeyValuePair<string, string>> trips = new List<KeyValuePair<string, string>>();
            trips.Add(new KeyValuePair<string, string>("key", "value"));
            trips.Add(new KeyValuePair<string, string>("key2", "value2"));
            trips.Add(new KeyValuePair<string, string>("key3", "value3"));

            var userMessage = new VoiceCommandUserMessage();
            var destinationsContentTiles = new List<VoiceCommandContentTile>();
            if (trips.Count() == 0) {
                // In this scenario, perhaps someone has modified data on your service outside of your 
                // control. If you're accessing a remote service, having a background task that
                // periodically refreshes the phrase list so it's likely to be in sync is ideal.
                // This is unlikely to occur for this sample app, however.
                string foundNoTripToDestination = string.Format(
                       cortanaResourceMap.GetValue("FoundNoTripToDestination", cortanaContext).ValueAsString,
                       destination);
                userMessage.DisplayMessage = foundNoTripToDestination;
                userMessage.SpokenMessage = foundNoTripToDestination;
            } else {
                // Set a title message for the page.
                string message = "";
                if (trips.Count() > 1) {
                    message = cortanaResourceMap.GetValue("PluralUpcomingTrips", cortanaContext).ValueAsString;
                } else {
                    message = cortanaResourceMap.GetValue("SingularUpcomingTrip", cortanaContext).ValueAsString;
                }
                userMessage.DisplayMessage = message;
                userMessage.SpokenMessage = message;

                // file in tiles for each destination, to display information about the trips without
                // launching the app.
                foreach (KeyValuePair<string, string> trip in trips) {
                    int i = 1;

                    var destinationTile = new VoiceCommandContentTile();

                    // To handle UI scaling, Cortana automatically looks up files with FileName.scale-<n>.ext formats based on the requested filename.
                    // See the VoiceCommandService\Images folder for an example.
                    destinationTile.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                    destinationTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///AdventureWorks.VoiceCommands/Images/GreyTile.png"));

                    destinationTile.AppLaunchArgument = trip.Key;
                    destinationTile.Title = trip.Key;
                    destinationTile.TextLine1 = trip.Value + " " + i;


                    destinationsContentTiles.Add(destinationTile);
                    i++;
                }
            }

            var response = VoiceCommandResponse.CreateResponse(userMessage, destinationsContentTiles);

            if (trips.Count() > 0) {
                response.AppLaunchArgument = destination;
            }

            await voiceServiceConnection.ReportSuccessAsync(response);
        }

        /// <summary>
        /// Provide a simple response that launches the app. Expected to be used in the
        /// case where the voice command could not be recognized (eg, a VCD/code mismatch.)
        /// </summary>
        private async void LaunchAppInForeground() {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "Öffne App";

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            response.AppLaunchArgument = "";

            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }

        /// <summary>
        /// Handle the completion of the voice command. Your app may be cancelled
        /// for a variety of reasons, such as user cancellation or not providing 
        /// progress to Cortana in a timely fashion. Clean up any pending long-running
        /// operations (eg, network requests).
        /// </summary>
        /// <param name="sender">The voice connection associated with the command.</param>
        /// <param name="args">Contains an Enumeration indicating why the command was terminated.</param>
        private void OnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args) {
            if (this.serviceDeferral != null) {
                this.serviceDeferral.Complete();
            }
        }

        /// <summary>
        /// When the background task is cancelled, clean up/cancel any ongoing long-running operations.
        /// This cancellation notice may not be due to Cortana directly. The voice command connection will
        /// typically already be destroyed by this point and should not be expected to be active.
        /// </summary>
        /// <param name="sender">This background task instance</param>
        /// <param name="reason">Contains an enumeration with the reason for task cancellation</param>
        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason) {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
            if (this.serviceDeferral != null) {
                //Complete the service deferral
                this.serviceDeferral.Complete();
            }
        }
    }
}
