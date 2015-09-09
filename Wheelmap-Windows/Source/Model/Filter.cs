using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Extensions;

namespace Wheelmap_Windows.Model {

    public class Filter {

        public ISet<Status> FilterdStati = new SortedSet<Status>();
        public ISet<Status> FilterdWcStati = new SortedSet<Status>();
        public ISet<string> FilteredCategoryIdentifier = new SortedSet<string>();

        public List<Node> FilterNodes(ICollection<Node> items) {

            List<Node> newList = new List<Node>();

            foreach(Node n in items) {
                if (FilterdStati.Contains(Stati.From(n.wheelchairStatus))) {
                    continue;
                }
                if (FilterdWcStati.Contains(Stati.From(n.wheelchairToiletStatus))) {
                    continue;
                }
                if (n.category?.identifier != null && FilteredCategoryIdentifier.Contains(n.category.identifier)) {
                    continue;
                }
                newList.Add(n);
            }
            return newList;
        }
    }
}
