using System.ComponentModel;

namespace VNCOverlay
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private string portNumber;

        public string PortNumber
        {
            get => portNumber;
            set
            {
                if (portNumber != value)
                {
                    portNumber = value;
                    OnPropertyChanged(nameof(PortNumber));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
