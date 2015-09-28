using NotificationsExtensions.TileContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace Wheelmap.Tiles {
    public class WheelmapTileManager {
        public static void UpdateDefaultPrimaryTile() {
            // create the instance of Tile Updater, which enables you to change the appearance of the calling app's tile         
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            // enables the tile to queue up to five notifications         
            updater.EnableNotificationQueue(true);
            updater.Clear();
            
            var tileContent = TileContentFactory.CreateTileWide310x150Image();
            tileContent.Image.Src = "ms-appx:///Assets/App/wide_310x150/wide.png";
            tileContent.RequireSquare150x150Content = false;
            updater.Update(tileContent.CreateNotification());

            tileContent = TileContentFactory.CreateTileWide310x150Image();
            tileContent.Image.Src = "ms-appx:///Assets/App/wide_310x150/wide.png";
            tileContent.RequireSquare150x150Content = false;
            updater.Update(tileContent.CreateNotification());

        }
    }
}
