using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Wheelmap.Api.Model;
using Wheelmap.Model;
using Wheelmap.Source.Utils;
using Wheelmap.Utils;
using Wheelmap.Utils.Preferences;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Graphics.Imaging;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace Wheelmap.Api.Calls {
    public class IconDownloadRequest {

        private const string ETAG_KEY = nameof(IconDownloadRequest)+"_icons";
        
        // ICON_HEIGHT and ICON_WIDTH depends on the default MapIcon size
        private int ICON_HEIGHT;
        private int ICON_WIDTH;
        
        private Rect destRectIcon;
        private Rect destRectBG;

        WriteableBitmap bitmapStateYes;
        WriteableBitmap bitmapStateNo;
        WriteableBitmap bitmapStateLimited;
        WriteableBitmap bitmapStateUnknown;
        
        public IconDownloadRequest() {
            initIconDimensions();
        }

        public async Task<bool> Query() {
            var assets = await new AssetsRequest().Execute();
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

                    Log.d("Save icons to " + folder.Path);
                    var done = await DownloadAsset(asset, folder);
                    if (done) {
                        try {
                            var fromFolder = await folder.CreateFolderAsync(iconsType, CreationCollisionOption.OpenIfExists);
                            var toFolder = await folder.CreateFolderAsync(Constants.FOLDER_COMBINED_ICONS, CreationCollisionOption.OpenIfExists);
                            bool ret = await PrepareImages(fromFolder, toFolder);
                            if (ret) {
                                ApiPreferences.SetEtag(ETAG_KEY, asset.modified_at + "");
                            }
                            return ret;
                        }catch {
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// changes the bounds of the created icons to match the resolutionscale of the device
        /// this is needed because MapIcons do not scale with the resolution
        /// </summary>
        private void initIconDimensions() {
            ICON_HEIGHT = 33;
            ICON_WIDTH = 29;

            var iconX = 4;
            var iconY = 4;

            if (DeviceUtils.DetectPlatform() == Platform.WindowsPhone) {
                var scaleInt = DeviceUtils.GetResolutionScaleForCurrentViewInPercentage();
                Log.d("Scale = " + scaleInt);
                if (scaleInt != 100) {
                    var scale = scaleInt / 100d;
                    ICON_HEIGHT = (int) Math.Round(ICON_HEIGHT * scale);
                    ICON_WIDTH = (int) Math.Round(ICON_WIDTH * scale);
                    iconX = (int) Math.Round(iconX * scale);
                    iconY = (int) Math.Round(iconY * scale);
                }
            }

            destRectIcon = new Rect(iconX, iconY, ICON_WIDTH - 2*iconX, ICON_WIDTH - 2*iconX);
            destRectBG = new Rect(0, 0, ICON_WIDTH, ICON_HEIGHT);
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
                Log.e(e.Message);
                Log.e(e.StackTrace);
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
                Log.d(folder.Path);
                StorageFile localFile = await folder.CreateFileAsync("temp.zip", CreationCollisionOption.ReplaceExisting);
                BackgroundDownloader downloader = new BackgroundDownloader();
                DownloadOperation download = downloader.CreateDownload(new Uri(asset.url), localFile);
                await HandleDownloadAsync(download, true);
                await ZipHelper.UnZipFileAsync(localFile, folder);

                return true;
            }
            catch (Exception e) {
                Log.e(e.StackTrace);
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
                Log.e("Canceled: " + download.Guid);
            }
        }
    }

    public class AssetsRequest : PagedRequest<AssetsResponse, Asset> {

        protected override string GetUrl(int page) {
            string pageParam = "page=" + page;
            string pageSizeParam = "per_page=" + PAGE_SIZE;
            string url = BuildConfig.API_BASEURL + ApiConstants.END_POINT_ASSETS + "?"
                + ApiConstants.API_KEY_PARAM + "&"
                + pageSizeParam + "&"
                + page;
            return url;
        }
    }


}
