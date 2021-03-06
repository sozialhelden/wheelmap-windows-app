﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Wheelmap.Api.Calls;
using Wheelmap.Model;
using Wheelmap.Tiles;
using Wheelmap.Utils;

namespace Wheelmap.Source.UI.Pages.Splashscreen {
    public sealed partial class ExtendedSplashPage {

        public async Task<bool> LoadData() {
            
            try {
                WheelmapTileManager.UpdateDefaultPrimaryTile();
            } catch (Exception e) {
                // ignore error because this is not essential for the app
                Log.e(e);
            }

            try {
                bool success = true;
                success &= await readCategories();
                success &= await readNodeTypes();
                success &= await new IconDownloadRequest().Query();
                return success;
            } catch (Exception e){
                Log.e(e);
                return false;
            }

        }

        private async Task<bool> readNodeTypes() {
            var nodeTypes = await new NodeTypeRequest().Execute();
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
            var categories = await new CategoryRequest().Execute();
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
