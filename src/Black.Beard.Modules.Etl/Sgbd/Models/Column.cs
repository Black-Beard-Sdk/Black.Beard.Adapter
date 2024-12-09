using Bb.ComponentModel.Attributes;
using Bb.TypeDescriptors;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bb.Modules.Sgbd.Models
{


    public class Column : INotifyPropertyChanged, IDynamicDescriptorInstance
    {

        public Column()
        {
            this._container = new DynamicDescriptorInstanceContainer(this);
        }

        [JsonIgnore]
        [Browsable(false)]
        public Table Table { get; set; }

        [Browsable(false)]
        public Guid Id { get; set; }

        [MaxLength(128, ErrorMessage = DatasComponentConstants.ValueCantBeExceed)]
        [RegularExpression(SqlserverConstants.NameConstraint, ErrorMessage = DatasComponentConstants.ColumnValidationMessage)]
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


        public SgbdDiagram? Diagram() => Table?.Source.GetDiagram<SgbdDiagram>();

        public SgbdTechnology? GetTechnology() => Diagram()?.GetTechnology();

        public ColumnType GetColumnType()
        {
            var types = GetTechnology()?.ColumnTypes;
            var result = types.Where(c => c.Code == Type).FirstOrDefault();
            return result;
        }



        #region IDynamicDescriptorInstance

        public object GetProperty(string name) => this._container.GetProperty(name);

        public void SetProperty(string name, object value) => this._container.SetProperty(name, value);

        #endregion IDynamicDescriptorInstance


        #region INotifyPropertyChanged

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged



        private bool isPrimary;
        private string _name;
        private string _type;
        private readonly DynamicDescriptorInstanceContainer _container;

    }


}
