using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Api.Model;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.Source.Utils;
using Wheelmap_Windows.Utils;
using Wheelmap_Windows.Utils.Preferences;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace Wheelmap_Windows.Api.Calls {
    public class IconDownloadRequest {

        private const string TAG = "IconDownloadRequest";
        private const string ETAG_KEY = TAG + "_icons";
        
        // ICON_HEIGHT and ICON_WIDTH depends on the default MapIcon size
        private const int ICON_HEIGHT = 33;
        private const int ICON_WIDTH = 29;
        
        private Rect destRectIcon = new Rect(3, 3, ICON_WIDTH - 7, ICON_WIDTH - 7);
        private Rect destRectBG = new Rect(0, 0, ICON_WIDTH, ICON_HEIGHT);

        WriteableBitmap bitmapStateYes;
        WriteableBitmap bitmapStateNo;
        WriteableBitmap bitmapStateLimited;
        WriteableBitmap bitmapStateUnknown;
        
        public async Task<bool> Query() {
            var assets = await new AssetsRequest().Query();
            if (assets?.Count <= 0) {
                // we have some data in the cache
                return ApiPreferences.GetEtag(ETAG_KEY) != null;
            }
            var iconsType = "icons";
            foreach (Asset asset in assets) {
                if (asset.type == iconsType) {
                    string folderName = Constants.FOLDER_MARKER_ICONS;
                    StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                    StorageFolder folder = null;
                    try {
                        // check if folder already exists
                        folder = await local.GetFolderAsync(folderName);
                    }catch { }
                    if (folder != null) {
                        // check if update is needed
                        if ((asset.modified_at+"") == ApiPreferences.GetEtag(ETAG_KEY)) {
                            return true;
                        }
                    }
                    folder = await local.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

                    Log.d(TAG, "Save icons to " + folder.Path);
                    var done = await DownloadAsset(asset, folder);
                    if (done) {
                        var fromFolder = await folder.CreateFolderAsync(iconsType, CreationCollisionOption.OpenIfExists);
                        var toFolder = await folder.CreateFolderAsync(Constants.FOLDER_COMBINED_ICONS, CreationCollisionOption.OpenIfExists);
                        bool ret = await PrepareImages(fromFolder, toFolder);
                        if (ret) {
                            ApiPreferences.SetEtag(ETAG_KEY, asset.modified_at + "");
                        }
                        return ret;
                    }
                }
            }
            return false;
        }

        private async Task LoadBackgroundImages(StorageFolder toFolder) {
            
            var uri = "ms-appx:///Assets/Images/mapmarker/bg_status_yes.png";
            var fileStream = RandomAccessStreamReference.CreateFromUri(new Uri(uri));
            bitmapStateYes = new WriteableBitmap(ICON_WIDTH,ICON_HEIGHT);
            bitmapStateYes.SetSource(await fileStream.OpenReadAsync());
            
            uri = "ms-appx:///Assets/Images/mapmarker/bg_status_no.png";
            fileStream = RandomAccessStreamReference.CreateFromUri(new Uri(uri));
            bitmapStateNo = new WriteableBitmap(ICON_WIDTH, ICON_HEIGHT);
            bitmapStateNo.SetSource(await fileStream.OpenReadAsync());
            
            uri = "ms-appx:///Assets/Images/mapmarker/bg_status_limited.png";
            fileStream = RandomAccessStreamReference.CreateFromUri(new Uri(uri));
            bitmapStateLimited = new WriteableBitmap(ICON_WIDTH, ICON_HEIGHT);
            bitmapStateLimited.SetSource(await fileStream.OpenReadAsync());
            
            uri = "ms-appx:///Assets/Images/mapmarker/bg_status_unknown.png"; ;
            fileStream = RandomAccessStreamReference.CreateFromUri(new Uri(uri));
            bitmapStateUnknown = new WriteableBitmap(ICON_WIDTH, ICON_HEIGHT);
            bitmapStateUnknown.SetSource(await fileStream.OpenReadAsync());

            // save copy

            StorageFile fileYes = await toFolder.CreateFileAsync($"yes_.png", CreationCollisionOption.ReplaceExisting);
            StorageFile fileNo = await toFolder.CreateFileAsync($"no_.png", CreationCollisionOption.ReplaceExisting);
            StorageFile fileLimited = await toFolder.CreateFileAsync($"limited_.png", CreationCollisionOption.ReplaceExisting);
            StorageFile fileUnknown = await toFolder.CreateFileAsync($"unknown_.png", CreationCollisionOption.ReplaceExisting);
            await mergeAndSaveFile(bitmapStateYes, null, fileYes);
            await mergeAndSaveFile(bitmapStateNo, null, fileNo);
            await mergeAndSaveFile(bitmapStateLimited, null, fileLimited);
            await mergeAndSaveFile(bitmapStateUnknown, null, fileUnknown);

        }

        private async Task<bool> PrepareImages(StorageFolder fromFolder, StorageFolder toFolder) {

            try {
                await LoadBackgroundImages(toFolder);
            } catch(Exception e) {
                Log.e(TAG, e.Message);
                Log.e(TAG, e.StackTrace);
                return false;
            }

            var files = await fromFolder.GetFilesAsync();
            foreach (StorageFile file in files) {

                if (file.FileType != ".png" || !file.Name.Contains("@2x")) {
                    continue;
                }
                
                ImageProperties properties = await file.Properties.GetImagePropertiesAsync();
                WriteableBitmap icon = new WriteableBitmap((int)properties.Width, (int)properties.Height);
                icon.SetSource((await file.OpenReadAsync()));
                
                var fileName = file.Name.Replace("@2x", "");
                StorageFile fileYes = await toFolder.CreateFileAsync($"yes_{fileName}", CreationCollisionOption.ReplaceExisting);
                StorageFile fileNo = await toFolder.CreateFileAsync($"no_{fileName}", CreationCollisionOption.ReplaceExisting);
                StorageFile fileLimited = await toFolder.CreateFileAsync($"limited_{fileName}", CreationCollisionOption.ReplaceExisting);
                StorageFile fileUnknown = await toFolder.CreateFileAsync($"unknown_{fileName}", CreationCollisionOption.ReplaceExisting);
                
                await mergeAndSaveFile(bitmapStateYes, icon, fileYes);
                await mergeAndSaveFile(bitmapStateNo, icon, fileNo);
                await mergeAndSaveFile(bitmapStateLimited, icon, fileLimited);
                await mergeAndSaveFile(bitmapStateUnknown, icon, fileUnknown);
                
            }

            return true;
        }
        
        private async Task mergeAndSaveFile(WriteableBitmap background, WriteableBitmap icon, StorageFile saveFile) {
            
            var h = background.PixelHeight;
            var w = background.PixelWidth;
            var merge = new WriteableBitmap(ICON_WIDTH, ICON_HEIGHT);
            merge.Clear(Colors.Transparent);
            merge.Blit(destRectBG, background, new Rect(0, 0, w, h));

            if (icon != null) {
                merge.Blit(destRectIcon, icon, new Rect(0, 0, icon.PixelWidth, icon.PixelHeight));
            } else {
                merge.Blit(destRectBG, background, new Rect(0, 0, w, h));
            }

            IRandomAccessStream saveStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite);
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, saveStream);
            // Get pixels of the WriteableBitmap object 
            Stream pixelStream = merge.PixelBuffer.AsStream();
            byte[] pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);
            // Save the image file with jpg extension                 
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)merge.PixelWidth, (uint)merge.PixelHeight, 96.0, 96.0, pixels);
            await encoder.FlushAsync();

        }

        public async Task<bool> DownloadAsset(Asset asset, StorageFolder folder) {
            try {
                Log.d(TAG, folder.Path);
                StorageFile localFile = await folder.CreateFileAsync("temp.zip", CreationCollisionOption.ReplaceExisting);
                BackgroundDownloader downloader = new BackgroundDownloader();
                DownloadOperation download = downloader.CreateDownload(new Uri(asset.url), localFile);
                await HandleDownloadAsync(download, true);
                await ZipHelper.UnZipFileAsync(localFile, folder);

                return true;
            }
            catch (Exception e) {
                Log.e(TAG, e.StackTrace);
                return false;
            }
        }

        private async Task HandleDownloadAsync(DownloadOperation download, bool start) {
            try {

                if (start) {
                    // Start the download and attach a progress handler.
                    await download.StartAsync().AsTask();
                } else {
                    // The download was already running when the application started, re-attach the progress handler.
                    await download.AttachAsync().AsTask();
                }

                ResponseInformation response = download.GetResponseInformation();

            }
            catch (TaskCanceledException) {
                Log.e(TAG, "Canceled: " + download.Guid);
            }
        }
    }

    public class AssetsRequest : PagedRequest<AssetsResponse, Asset> {

        protected override string GetUrl(int page) {
            string pageParam = "page=" + page;
            string pageSizeParam = "page_size=" + PAGE_SIZE;
            string url = BuildConfig.API_BASEURL + ApiConstants.END_POINT_ASSETS + "?"
                + ApiConstants.API_KEY_PARAM + "&"
                + pageSizeParam + "&"
                + page;
            return url;
        }
    }


}
