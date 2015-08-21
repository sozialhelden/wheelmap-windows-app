using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Utils.Eventbus;
using Wheelmap_Windows.Utils.Eventbus.Events;

namespace Wheelmap_Windows.Utils {

    public class DataHolder {
        
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

        public Model.Node SelectedNode;
        public List<Model.Node> Nodes;

        public Dictionary<string, Model.Category> Categories = new Dictionary<string, Model.Category>();

        private DataHolder() {
            BusProvider.DefaultInstance.Register(this);
        }
        
        [Subscribe]
        public void OnNewNodes(NewNodesEvent e) {
            Nodes = e.nodes;
        }

        [Subscribe]
        public void OnSelectedNodeChanged(SelectedNodeChangedEvent e) {
            SelectedNode = e.node;
        }
    }
}
