using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Api.Calls;
using Wheelmap.Extensions;
using Wheelmap.Model;
using Wheelmap.Utils.Eventbus;
using Wheelmap.Utils.Eventbus.Events;
using Wheelmap.Utils.Preferences;
using Windows.UI.Xaml;

namespace Wheelmap.Utils {

    /**
     * core component to store all queried data from the api for other components
     */
    public sealed class DataHolder : INotifyPropertyChanged {
        
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


        private List<Node> _Nodes = new List<Node>();

        public List<Node> Nodes {

            get {
                return _Nodes;
            }

            // filter list and notify all other components using the Bussystem
            set {
                HadData = true;
                _Nodes = value;
                _FilterdNodes = Filter.FilterNodes(_Nodes);
                var e = new NewNodesEvent() {
                    nodes = _FilterdNodes,
                    RefreshAll = true
                };
                BusProvider.DefaultInstance.Post(e);
            }

        }

        /**
         * indicates if dataholder ever had nodes
         * this is important to fetch data at the app start for the current user position
         */
        public bool HadData { get; set; } = false;

        public Filter Filter = new Filter();
        public void RefreshFilter() {
            _FilterdNodes = Filter.FilterNodes(_Nodes);

            var filterChangedEvent = new FilterChangedEvent {
                Filter = Filter
            };
            BusProvider.DefaultInstance.Post(filterChangedEvent);

            var e = new NewNodesEvent() {
                nodes = _FilterdNodes,
                RefreshAll = false
            };
            Prefs.SaveFilter(Filter);
            BusProvider.DefaultInstance.Post(e);
            
        }

        private List<NodeType> _nodeTypes = new List<NodeType>();
        public List<NodeType> NodeTypes {
            get {
                return _nodeTypes;  
            }
            set {
                _nodeTypes = value;
                NodeTypeById.Clear();
                foreach(NodeType type in value) {
                    NodeTypeById.Add(type.Id, type);
                }
            }
        }
        
        public IDictionary<int, NodeType> NodeTypeById = new Dictionary<int, NodeType>();
        
        public Dictionary<string, Category> Categories = new Dictionary<string, Category>();

        private string _QueryString = null;
        public string QueryString {
            get {
                return _QueryString;
            }
            set {
                _QueryString = value;

                if (DataHolder.Instance.QueryString?.Length >= 3) {
                    // query new nodes
                    new NodeSearchRequest(DataHolder.Instance.QueryString).Execute().ContinueWithOnDispatcher(Window.Current.Content.Dispatcher, task => {
                        DataHolder.Instance.Nodes = task.Result;
                    });
                }
                
                NotifyPropertyChanged(nameof(QueryString));
            }
        }

        private int _IsRequestRunning = 0;
        public bool IsRequestRunning {
            get {

                return _IsRequestRunning > 0;

            }
            set {

                if (value) {
                    _IsRequestRunning++;
                } else {
                    _IsRequestRunning--;
                }

                if (_IsRequestRunning <= 0) {
                    _IsRequestRunning = 0;
                }

                if (_IsRequestRunning <= 1) {
                    NotifyPropertyChanged(nameof(IsRequestRunning));
                }
                Log.d(this,"Requests: "+ _IsRequestRunning);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        
        private DataHolder() {
            BusProvider.DefaultInstance.Register(this);
            Filter = Prefs.RestoreFilter() ?? new Filter();
            Nodes = Model.Nodes.QueryAllDistinct();
        }
        
        [Subscribe]
        public void OnSelectedNodeChanged(SelectedNodeChangedEvent e) {
            SelectedNode = e.node;
        }
        
    }
}
