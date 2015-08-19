using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap_Windows.Utils.Eventbus {

    [AttributeUsage(AttributeTargets.Method)]
    public class Subscribe : System.Attribute {
        public Subscribe() : base() {
        }
    }

}
