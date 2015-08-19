using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap_Windows.Utils.Eventbus {
    public class BusProvider {
        private static EventBus _instance;
        public static EventBus DefaultInstance {
            get {
                if (_instance == null) {
                    _instance = new EventBus();
                }

                return _instance;
            }
        }

        private BusProvider() {
        }
        
    }
}
