using Bb.ComponentModel.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bb.Modules.Sgbd.Models
{


    public class Column : INotifyPropertyChanged
    {

        public Column()
        {
            
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        [JsonIgnore]
        [Browsable(false)]
        public Table Table { get; set; }    

        [Browsable(false)]
        public Guid Id { get; set; }

        [Description("Column name")]
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

        [Description("Column type")]
        [ListProvider(typeof(ListProviderColumnTypeTechnologies))]
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

        [Browsable(false)]
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

        [Description("Column is nullable")]
        public bool Nullable { get; set; }

        public string DefaultValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool isPrimary;
        private string _name;
        private string _type;

    }


}
