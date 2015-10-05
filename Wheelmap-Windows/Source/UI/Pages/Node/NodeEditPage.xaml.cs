using System;
using System.Collections.Generic;
using System.Linq;
using Wheelmap.Model;
using Wheelmap.UI.Pages.Base;
using Wheelmap.Utils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Wheelmap.Extensions;
using Windows.UI.Xaml.Media.Imaging;
using Wheelmap.Api.Calls;
using Wheelmap.Source.UI.Pages.Status;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;

namespace Wheelmap.Source.UI.Pages.Node {

    public sealed partial class NodeEditPage : BasePage {

        public override string Title {
            get {
                if (node == null) {
                    return "TITLE_NEW".t();
                } else {
                    return "TITLE_EDIT".t();
                }
            }
        }

        Model.Node node;
        List<Category> categories = new List<Category>();
        List<NodeType> nodeTypes = new List<NodeType>();

        Model.Status? wheelchairStatus;
        Model.Status? wheelchairWCStatus;
        BasicGeoposition? position;

        public NodeEditPage() {
            this.InitializeComponent();
            scrollViewer.HideVerticalScrollBarsIfContentFits();
            categoryComboBox.SelectionChanged += CategoryComboBox_SelectionChanged;
            saveButton.Content = "SAVE".t(R.File.NODE);
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count() <= 0) {
                initNodeTypeComboBox(null);
                return;
            }
            var category = categories[categoryComboBox.SelectedIndex];
            initNodeTypeComboBox(category);
        }

        public override void OnNewParams(object args) {
            base.OnNewParams(args);
            node = args as Model.Node;
            if (node != null) {
                position = new BasicGeoposition {
                    Latitude = node.lat,
                    Longitude = node.lon
                };
                positionTextBox.Text = String.Format("POSITION".t(R.File.NODE), position?.Latitude, position?.Longitude);
            }
            titleTextBlock.Text = Title;
            initCategoryComboBox();
            initStatus(node);
            nameTextBox.Text = node?.name ?? "";

            streetTextBox.Text = node?.street ?? "";
            houseNumberTextBox.Text = node?.housenumber ?? "";
            plzTextBox.Text = node?.postcode ?? "";
            cityTextBox.Text = node?.city ?? "";
            websiteTextBox.Text = node?.website ?? "";
            commentTextBox.Text = node?.wheelchairDescription ?? "";

        }

        private void initCategoryComboBox() {
            categoryComboBox.Items.Clear();
            categories.Clear();
            foreach (var c in DataHolder.Instance.Categories.Values) {
                categories.Add(c);
                categoryComboBox.Items.Add(c.localizedName);
            }
            categoryComboBox.SelectedItem = node?.category?.localizedName;
            
            initNodeTypeComboBox(node?.category);
        }

        /**
         * initialize the nodetype combobox to show only types for the given category
         */
        private void initNodeTypeComboBox(Category c) {
            if (c == null) {
                nodeTypes.Clear();
                nodeTypeComboBox.Items.Clear();
                return;
            }
            nodeTypes.Clear();
            nodeTypeComboBox.Items.Clear();
            foreach (var type in DataHolder.Instance.NodeTypes) {
                if (type.category_id == c.Id) {
                    nodeTypes.Add(type);
                    nodeTypeComboBox.Items.Add(type.localizedName);
                }
            }
            nodeTypeComboBox.SelectedItem = node?.nodeType?.localizedName;
        }

        private void initStatus(Model.Node node) {

            wheelchairStatus = Stati.From(node?.wheelchairStatus);
            wheelchairWCStatus = Stati.From(node?.wheelchairToiletStatus);
            updateStati();
        }

        private void updateStati() {
            statusBgBorder.Background = new SolidColorBrush(wheelchairStatus.Value.GetColor());
            statusTextBlock.Text = wheelchairStatus?.GetLocalizedMessage();
            statusImage.Source = new BitmapImage(new
                          Uri(wheelchairStatus?.GetImage(), UriKind.RelativeOrAbsolute));

            statusToiletBgBorder.Background = new SolidColorBrush(wheelchairWCStatus.Value.GetColor());
            statusToiletTextBlock.Text = wheelchairWCStatus?.GetLocalizedToiletMessage();
            statusToiletImage.Source = new BitmapImage(new
                          Uri(wheelchairWCStatus?.GetImage(), UriKind.RelativeOrAbsolute));
        }

        /**
         * checks if all required field are set to save this node
         */
        private bool CheckIfAllRequiredFieldsAreValid() {

            if (categoryComboBox.SelectedItem == null) {
                return false;
            }

            if (nodeTypeComboBox.SelectedItem == null) {
                return false;
            }

            if (wheelchairStatus == null) {
                return false;
            }

            if (wheelchairWCStatus == null) {
                return false;
            }

            if (position == null) {
                return false;
            }

            return true;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) {

            if (!CheckIfAllRequiredFieldsAreValid()) {
                return;
            }

            bool createNewNode = node == null;
            if (node == null) {
                node = new Model.Node() {
                    NodeTag = NodeTag.COPY
                };
            } else {
                node = node.CreateCopyIfNeeded();
            }
            node.DirtyState = DirtyState.DIRTY_ALL;

            node.category = categories[categoryComboBox.SelectedIndex];
            node.nodeType = nodeTypes[nodeTypeComboBox.SelectedIndex];
            node.name = nameTextBox.Text.Trim();
            node.wheelchairDescription = commentTextBox.Text.Trim();

            node.street = streetTextBox.Text.Trim();
            node.postcode = plzTextBox.Text.Trim();
            node.city = cityTextBox.Text.Trim();
            node.housenumber = houseNumberTextBox.Text.Trim();
            node.phone = phoneNumberTextBox.Text.Trim();
            node.website = websiteTextBox.Text.Trim();
            node.wheelchairStatus = wheelchairStatus.Value.ToApiString();
            node.wheelchairToiletStatus = wheelchairWCStatus.Value.ToApiString();
            node.lat = position.Value.Latitude;
            node.lon = position.Value.Longitude;

            progressBar.Visibility = Visibility.Visible;
            new NodeEditRequest(node).Execute().ContinueWithOnDispatcher(Dispatcher, task => {
                progressBar.Visibility = Visibility.Collapsed;
                if (task.Result.IsOk) {
                    node.DirtyState = DirtyState.CLEAN;
                    Nodes.Save(node);
                    GoBack();
                } else {

                    // show error dialog with messages from backend

                    var content = "";
                    foreach (var key in task.Result.error) {
                        foreach (string value in key.Value) {
                            content += "\u00B7 " + value + "\n";
                        }
                    }

                    MessageDialog dialog = new MessageDialog(content);
                    dialog.ShowAsync().Forget();

                }
            }, 2000);
        }

        private void positionTextBox_Tapped(object sender, TappedRoutedEventArgs e) {

            PositionChooserDialogPageArgs args = null;
            if (position != null) {
                args = new PositionChooserDialogPageArgs {
                    lat = position.Value.Latitude,
                    lon = position.Value.Longitude
                };
            }

            PositionChooserDialogPage.ShowInDialog(args, (position) => {
                this.position = position;
                positionTextBox.Text = String.Format("POSITION".t(R.File.NODE), position.Latitude, position.Longitude);
            });
        }
        
        private void status_Tapped(object sender, TappedRoutedEventArgs e) {
            ChangeStatusDialogPage.ShowInDialog(new ChangeStatusDialogPageArgs { status = wheelchairStatus ?? Model.Status.UNKNOWN, isInWcMode = false }, (status) => {
                wheelchairStatus = status;
                updateStati();
            });
        }

        private void statusToilet_Tapped(object sender, TappedRoutedEventArgs e) {
            ChangeStatusDialogPage.ShowInDialog(new ChangeStatusDialogPageArgs { status = wheelchairWCStatus ?? Model.Status.UNKNOWN, isInWcMode = true }, (status) => {
                wheelchairWCStatus = status;
                updateStati();
            });
        }
        
    }
}
