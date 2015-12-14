using System.Collections.Generic;

namespace Wheelmap.Model {

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

        public SerializableFilter toSerializableFilter() {
            return SerializableFilter.FromFilter(this);
        }
    }

    /// <summary>
    /// workaround to save the filters as json in database
    /// https://msdn.microsoft.com/en-us/library/dn600634(v=vs.110).aspx
    /// IEnumerable<T> is not deserializable => Store ISet as Arrays
    /// this class should only be used to store or read filters
    /// </summary>
    public class SerializableFilter {
        public Status[] FilterdStati = new Status[0];
        public Status[] FilterdWcStati = new Status[0];
        public string[] FilteredCategoryIdentifier = new string[0];

        public Filter ToFilter() {
            Filter filter = new Filter();

            foreach (Status status in FilterdStati) {
                if (!filter.FilterdStati.Contains(status)) {
                    filter.FilterdStati.Add(status);
                }
            }
            
            foreach (Status status in FilterdWcStati) {
                if (!filter.FilterdWcStati.Contains(status)) {
                    filter.FilterdWcStati.Add(status);
                }
            }
            
            foreach (string identifier in FilteredCategoryIdentifier) {
                if (!filter.FilteredCategoryIdentifier.Contains(identifier)) {
                    filter.FilteredCategoryIdentifier.Add(identifier);
                }
            }

            return filter;
        }
        
        public static SerializableFilter FromFilter(Filter filter) {
            if (filter == null) {
                return null;
            }

            SerializableFilter serializableFilter = new SerializableFilter();
            serializableFilter.FilterdStati = new Status[filter.FilterdStati.Count];
            int i = 0;
            foreach (Status status in filter.FilterdStati) {
                serializableFilter.FilterdStati[i] = status;
                i++;
            }

            serializableFilter.FilterdWcStati = new Status[filter.FilterdWcStati.Count];
            i = 0;
            foreach (Status status in filter.FilterdWcStati) {
                serializableFilter.FilterdWcStati[i] = status;
                i++;
            }

            serializableFilter.FilteredCategoryIdentifier = new string[filter.FilteredCategoryIdentifier.Count];
            i = 0;
            foreach (string identfier in filter.FilteredCategoryIdentifier) {
                serializableFilter.FilteredCategoryIdentifier[i] = identfier;
                i++;
            }

            return serializableFilter;
        }
        
    }
}
