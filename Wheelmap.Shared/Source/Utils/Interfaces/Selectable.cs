using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap.Source.Utils.Interfaces {
    public interface Selectable : INotifyPropertyChanged {
        bool Selected { get; set; }
    }
}
