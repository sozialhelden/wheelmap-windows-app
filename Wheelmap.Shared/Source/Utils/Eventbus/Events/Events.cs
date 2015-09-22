using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Model;
using Windows.Devices.Geolocation;

namespace Wheelmap.Utils.Eventbus.Events {

    public class SelectedNodeChangedEvent {

        public object sender;
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
