using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Api.Calls;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.Utils;
using Wheelmap_Window.Extensions;

namespace Wheelmap_Windows.Source.UI.Pages.Splashscreen {
    public sealed partial class ExtendedSplashPage {

        public async Task<bool> LoadData() {
            bool success = true;
            success &= await readCategories();
            success &= await readNodeTypes();
            success &= await new IconDownloadRequest().Query();
            return success;
        }

        private async Task<bool> readNodeTypes() {
            var nodeTypes = await new NodeTypeRequest().Query();
            if (nodeTypes == null || nodeTypes.Count <= 0) {
                nodeTypes = Database.Instance.Table<NodeType>().ToList();
            } else {
                Database.Instance.Table<NodeType>().Delete(x => true);
                Database.Instance.InsertAll(nodeTypes);
            }
            if (nodeTypes == null || nodeTypes.Count <= 0) {
                return false;
            }
            DataHolder.Instance.NodeTypes = nodeTypes;
            return true;
        }

        private async Task<bool> readCategories() {
            var categories = await new CategoryRequest().Query();
            if (categories != null || categories.Count <= 0) {
                Database.Instance.Table<Category>().Delete(x => true);
                Database.Instance.InsertAll(categories);
            } else {
                categories = Database.Instance.Table<Category>().ToList();
                if (categories == null || categories.Count() <= 0) {
                    return false;
                }
            }

            DataHolder.Instance.Categories.Clear();
            foreach (Category c in categories) {
                DataHolder.Instance.Categories.Add(c.identifier, c);
            }
            return true;
        }

    }
}
