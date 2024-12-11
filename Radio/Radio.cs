using System.ComponentModel;

namespace Radio
{
    public class Radio : INotifyPropertyChanged
    {
        private string _name = "";
        public required string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string _url = "";
        public required string Url
        {
            get { return _url; }
            set
            {
                if (value != _url)
                {
                    _url = value;
                    OnPropertyChanged(nameof(Url));
                }
            }
        }

        private string? _description;
        public string? Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
