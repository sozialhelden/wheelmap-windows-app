using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Model;

namespace Wheelmap_Windows.Utils.Eventbus.Events {
    public class SelectedNodeChangedEvent {
        public Node node;

        public override string ToString() {
            return "SelectedNodeChangedEvent: " + node.ToString();
        }
    }
}
