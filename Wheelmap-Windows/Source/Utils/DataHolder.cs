using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.Utils.Eventbus;
using Wheelmap_Windows.Utils.Eventbus.Events;

namespace Wheelmap_Windows.Utils {

    /**
     * core compontent to store all queried data from the api for other components
     */
    public sealed class DataHolder {
        
        private static DataHolder _instance;
        public static DataHolder Instance {
            get {
                if (_instance == null) {
                    _instance = new DataHolder();
                }
                return _instance;
            }
        }

        public static void Init() {
            var ignore = Instance;
        }

        public Node SelectedNode;

        private List<Node> _FilterdNodes;
        public List<Node> FilterdNodes {
            get {
                if (_FilterdNodes == null) {
                    _FilterdNodes = Filter.FilterNodes(Nodes);
                }
                return _FilterdNodes;
            }
        }


        private List<Node> _Nodes;

        public List<Node> Nodes {

            get {
                return _Nodes;
            }

            // filter list and notify all other components using the Bussystem
            set {
                _Nodes = value;
                _FilterdNodes = Filter.FilterNodes(_Nodes);
                var e = new NewNodesEvent();
                e.nodes = _FilterdNodes;
                BusProvider.DefaultInstance.Post(e);
            }

        }

        public Filter Filter = new Filter();

        private List<NodeType> _nodeTypes = new List<NodeType>();
        public List<NodeType> NodeTypes {
            get {
                return _nodeTypes;  
            }
            set {
                _nodeTypes = value;
                NodeTypeById.Clear();
                foreach(NodeType type in value) {
                    NodeTypeById.Add(type.id, type);
                }
            }
        }
        
        public IDictionary<int, NodeType> NodeTypeById = new Dictionary<int, NodeType>();
        
        public Dictionary<string, Category> Categories = new Dictionary<string, Category>();

        private DataHolder() {
            BusProvider.DefaultInstance.Register(this);
        }
        
        [Subscribe]
        public void OnSelectedNodeChanged(SelectedNodeChangedEvent e) {
            SelectedNode = e.node;
        }
        
    }
}
