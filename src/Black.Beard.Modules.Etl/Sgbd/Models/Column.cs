using System;
using System.ComponentModel;

namespace Bb.Modules.Sgbd.Models
{

    public class Column : INotifyPropertyChanged
    {

        public Column()
        {
            
        }


        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Guid Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(_name));
                }
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        public bool Primary
        {
            get => isPrimary;
            set
            {
                if (isPrimary != value)
                {

                    isPrimary = true;
                    OnPropertyChanged(nameof(Primary));
                }
            }
        }

        public bool Nullable { get; set; }

        public bool DefaultValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool isPrimary;
        private string _name;
        private string _type;

    }

}
