using System;

namespace Wheelmap.Utils.Eventbus {

    [AttributeUsage(AttributeTargets.Method)]
    public class Subscribe : System.Attribute {
        public Subscribe() : base() {
        }
    }

}
