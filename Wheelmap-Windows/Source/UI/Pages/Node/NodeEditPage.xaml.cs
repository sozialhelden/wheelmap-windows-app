using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.UI.Pages.Base;
using Wheelmap_Windows.Utils;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Wheelmap_Windows.Extensions;
using Windows.UI.Xaml.Media.Imaging;

namespace Wheelmap_Windows.Source.UI.Pages.Node {
    
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


            titleTextBlock.Text = Title;
            initCategoryComboBox();
            initStatus(node);
            nameTextBox.Text = node?.name ?? "";

            streetTextBox.Text = node?.street ?? "";
            houseNumberTextBox.Text = node?.housenumber ?? "";
            positionTextBox.Text = node?.postcode ?? "";
            cityTextBox.Text = node?.city ?? "";
            websiteTextBox.Text = node?.website ?? "";

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
                if (type.category_id == c.id) {
                    nodeTypes.Add(type);
                    nodeTypeComboBox.Items.Add(type.localizedName);
                }
            }
            nodeTypeComboBox.SelectedItem = node?.nodeType?.localizedName;
        }

        private void initStatus(Model.Node node) {

            var status = Stati.From(node?.wheelchairStatus);
            statusBgBorder.Background = new SolidColorBrush(status.GetColor());
            statusTextBlock.Text = status.GetLocalizedMessage();
            statusImage.Source = new BitmapImage(new
                          Uri(status.GetImage(), UriKind.RelativeOrAbsolute));

            var toiletStatus = Stati.From(node?.wheelchairToiletStatus);
            statusToiletBgBorder.Background = new SolidColorBrush(toiletStatus.GetColor());
            statusToiletTextBlock.Text = toiletStatus.GetLocalizedToiletMessage();
            statusToiletImage.Source = new BitmapImage(new
                          Uri(toiletStatus.GetImage(), UriKind.RelativeOrAbsolute));

        }

        private bool CheckIfAllRequiredFieldsAreValid() {

            if (categoryComboBox.SelectedItem == null) {
                return false;
            }

            if (nodeTypeComboBox.SelectedItem == null) {
                return false;
            }

            if (nameTextBox.Text.Trim().Length <= 0) {
                return false;
            }

            if (wheelchairStatus == null) {
                return false;
            }
            
            // TODO check for position
            
            return true;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) {

            if (!CheckIfAllRequiredFieldsAreValid()) {
                return;
            }

            bool createNewNode = node == null;
            if (node == null) {
                node = new Model.Node();
            }

            node.category = categories[categoryComboBox.SelectedIndex];
            node.nodeType = nodeTypes[nodeTypeComboBox.SelectedIndex];
            node.name = nameTextBox.Text.Trim();
            node.wheelchairDescription = commentTextBox.Text.Trim();

            node.street = streetTextBox.Text.Trim();
            node.postcode = plzTextBox.Text.Trim();
            node.city = cityTextBox.Text.Trim();
            node.phone = phoneNumberTextBox.Text.Trim();
            node.website = websiteTextBox.Text.Trim();
            
        }

        private void positionTextBox_Tapped(object sender, TappedRoutedEventArgs e) {
            Log.d(this, "positionTextBox_Tapped");
        }
    }
}
