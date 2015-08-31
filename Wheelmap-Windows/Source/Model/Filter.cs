using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Extensions;

namespace Wheelmap_Windows.Model {

    public class Filter {

        public ISet<Status> FilterdStati = new SortedSet<Status>();
        public ISet<long> FilteredCategoryIds = new SortedSet<long>();

        public List<Node> FilterNodes(ICollection<Node> items) {
            List<Node> newList = new List<Node>();

            foreach(Node n in items) {
                if (FilterdStati.Contains(Stati.From(n.wheelchairStatus))) {
                    continue;
                }
                if (FilteredCategoryIds.Contains(n.category.id)) {
                    continue;
                }
                newList.Add(n);
            }
            return newList;
        }
    }
}
