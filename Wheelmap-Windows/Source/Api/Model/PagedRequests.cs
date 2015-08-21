using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Model;

/**
 * collection of all pages requests
 */
namespace Wheelmap_Windows.Api.Model {

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
}
