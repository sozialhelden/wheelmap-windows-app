using NotificationsExtensions.TileContent;
using Windows.UI.Notifications;

namespace Wheelmap.Tiles {
    public class WheelmapTileManager {
        public static void UpdateDefaultPrimaryTile() {
            // create the instance of Tile Updater, which enables you to change the appearance of the calling app's tile         
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            // enables the tile to queue up to five notifications         
            updater.EnableNotificationQueue(true);
            updater.Clear();
            
            updater.Update(createNotification());
            updater.Update(createNotification());
            updater.Update(createNotification());

        }

        private static TileNotification createNotification() {

            var square150x150 = TileContentFactory.CreateTileSquare150x150Image();
            square150x150.Image.Src = "ms-appx:///Assets/App/square_150x150/square.png";

            var wide310x150 = TileContentFactory.CreateTileWide310x150Image();
            wide310x150.Image.Src = "ms-appx:///Assets/App/wide_310x150/wide.png";
            wide310x150.Square150x150Content = square150x150;

            var tileContent = TileContentFactory.CreateTileSquare310x310Image();
            tileContent.Image.Src = "ms-appx:///Assets/App/square_150x150/square.png";
            tileContent.Wide310x150Content = wide310x150;

            return tileContent.CreateNotification();
        }
    }
}
