using System.ComponentModel;

namespace PowerSet.Utils
{
    internal class ObservableChanged<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableChanged(T v) => val = v;

        private T val;
        public T Val
        {
            get => val;
            set
            {
                if (val.Equals(value))
                    return;

                val = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Val)));
            }
        }
    }
}
