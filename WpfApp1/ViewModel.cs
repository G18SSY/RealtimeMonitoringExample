using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfApp1.Annotations;

namespace WpfApp1
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual bool SetProperty<T>(ref T store, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(store, value))
                return false;

            store = value;
            OnPropertyChanged(propertyName);

            return true;
        }
    }
}