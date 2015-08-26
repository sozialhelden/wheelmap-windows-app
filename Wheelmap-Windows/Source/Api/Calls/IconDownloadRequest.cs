using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Api.Model;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.Source.Utils;
using Wheelmap_Windows.Utils;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace Wheelmap_Windows.Api.Calls {
    public class IconDownloadRequest {

        private const string TAG = "IconDownloadRequest";

        public async Task<bool> Query() {
            var assets = await new AssetsRequest().Query();
            foreach (Asset asset in assets) {
                if (asset.type == "marker") {
                    string folderName = Constants.FOLDER_MARKER_ICONS;
                    StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                    StorageFolder folder = null;
                    try {
                        // check if folder already exists
                        folder = await local.GetFolderAsync(folderName);
                    } catch { }
                    if (folder != null) {
                        // TODO check modified at
                        return true;
                    }
                    folder = await local.CreateFolderAsync(folderName, CreationCollisionOption.ReplaceExisting);
                    var done = await DownloadAsset(asset, folder);
                    if (done) {
                        return true;
                    }
                }
            }
            return false;
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
                + BuildConfig.API_KEY_PARAM + "&"
                + pageSizeParam + "&"
                + page;
            return url;
        }
    }


}
