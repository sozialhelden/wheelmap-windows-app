using System.ComponentModel;

namespace Wheelmap.Source.Utils.Interfaces {
    public interface Selectable : INotifyPropertyChanged {
        bool Selected { get; set; }
    }
}
