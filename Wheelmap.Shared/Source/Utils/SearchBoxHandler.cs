using Windows.UI.Xaml.Controls;

namespace Wheelmap.Utils {
    public class SearchBoxHandler {

        public SearchBox SearchBox { protected set; get; }

        public SearchBoxHandler(SearchBox searchBox) {
            SearchBox = searchBox;

            SearchBox.QueryChanged += SearchBox_QueryChanged;
            SearchBox.QuerySubmitted -= SearchBox_QuerySubmitted;
            SearchBox.QuerySubmitted += SearchBox_QuerySubmitted;

            DataHolder.Instance.PropertyChanged += DataHolder_PropertyChanged;
            SearchBox.QueryText = DataHolder.Instance.QueryString ?? "";
        }

        private void DataHolder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(DataHolder.Instance.QueryString)) {
                if (SearchBox.QueryText != DataHolder.Instance.QueryString) {
                    SearchBox.QueryText = DataHolder.Instance.QueryString ?? "";
                }
            }
        }

        private void SearchBox_QueryChanged(SearchBox sender, SearchBoxQueryChangedEventArgs args) {
            requestUpdate();
        }

        private void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args) {
            requestUpdate();
        }

        private void requestUpdate() {
            DataHolder.Instance.QueryString = SearchBox.QueryText;
        }
        
    }
}
