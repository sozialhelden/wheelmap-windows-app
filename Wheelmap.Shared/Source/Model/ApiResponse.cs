﻿using System.Collections.Generic;
using Wheelmap.Model;

/**
 * collection of all pages requests
 */
namespace Wheelmap.Api.Model {

    /**
     * base response for all PagedRequests
     */
    public abstract class PagedResponse<T> {

        public Conditions conditions;
        public Meta meta;

        public abstract T[] GetItems();
    }

    public class NodesResponse : PagedResponse<Node>{
        
        public Node[] nodes;

        public override Node[] GetItems() => nodes;
    }

    public class CategoryResponse : PagedResponse<Category> {
    
        public Category[] categories;

        public override Category[] GetItems() => categories;

    }

    public class PhotosResponse : PagedResponse<Photo> {
        public Photo[] photos;

        public override Photo[] GetItems() => photos;
    }

    public class AssetsResponse : PagedResponse<Asset> {

        public Asset[] assets;

        public override Asset[] GetItems() => assets;
    }

    public class NodeTypeResponse : PagedResponse<NodeType> {

        public NodeType[] node_types;

        public override NodeType[] GetItems() => node_types;

    }

    public class UserAuthenticateResponse {
        public User user;
    }

    public class NodeEditResponse {
        public string message;
        public Dictionary<string, string[]> error;

        public NodeEditResponse(bool ok = false) {
            if (ok) {
                message = "OK";
            }
        }

        public bool IsOk {
            get {
                return "OK".Equals(message);
            }
        }
    }
}
