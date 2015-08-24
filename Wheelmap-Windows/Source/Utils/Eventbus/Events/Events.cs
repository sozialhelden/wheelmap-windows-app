using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Wheelmap_Windows.Utils.Eventbus.Events {

    public class SelectedNodeChangedEvent {
        public Model.Node node;

        public override string ToString() {
            return "SelectedNodeChangedEvent: " + node.ToString();
        }
    }

    public class NewNodesEvent {
        public List<Model.Node> nodes;
    }

    public class LocationChangedEvent {
        public Geolocator Sender;
        public PositionChangedEventArgs Args;
    }
}
