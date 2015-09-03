using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Model;
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
        public bool RefreshAll;
    }

    public class LocationChangedEvent {
        public Geolocator Sender;
        public PositionChangedEventArgs Args;
    }

    public class FilterChangedEvent {
        public Filter Filter;
    }

    public class UserChangedEvent {
        public User User;
    }
}
