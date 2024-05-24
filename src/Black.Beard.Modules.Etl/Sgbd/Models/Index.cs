using Bb.ComponentModel.Attributes;
using Bb.TypeDescriptors;
using Blazor.Diagrams.Core.Models;
using MudBlazor;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bb.Modules.Sgbd.Models
{


    public class Index : INotifyPropertyChanged, IDynamicDescriptorInstance
    {

        public Index()
        {
            _columnIndices = new List<ColumnIndex>();
            this._container = new DynamicDescriptorInstanceContainer(this);
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        [JsonIgnore]
        [Browsable(false)]
        public Table Table { get; set; }

        [Browsable(true)]
        public bool Primary { get; set; }

        [Browsable(false)]
        public Guid Id { get; set; }

        [MaxLength(128, ErrorMessage = DatasComponentConstants.ValueCantBeExceed)]
        [RegularExpression(SqlserverConstants.NameConstraint, ErrorMessage = DatasComponentConstants.IndexValidationMessage)]
        [Description("index name")]
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

        [Browsable(false)]
        public List<ColumnIndex> Columns
        {
            get => _columnIndices ?? (_columnIndices = new List<ColumnIndex>());
            set
            {

                if (_columnIndices == null)
                    _columnIndices = new List<ColumnIndex>();

                foreach (ColumnIndex index in value)
                    AddColumn(index);

            }
        }

        public void AddColumn(ColumnIndex column)
        {
            if (!_columnIndices.Contains(column))
            {
                _columnIndices.Add(column);
                column.Index = this;
                column.PropertyChanged += Column_PropertyChanged;
            }
        }

        public void RemoveColumn(ColumnIndex column)
        {
            if (_columnIndices.Contains(column))
            {
                _columnIndices.Remove(column);
                column.Index = null;
                column.PropertyChanged -= Column_PropertyChanged;
            }
        }

        private void Column_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public object GetProperty(string name)
        {
            return this._container.GetProperty(name);
        }


        public void SetProperty(string name, object value)
        {
            this._container.SetProperty(name, value);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private List<ColumnIndex> _columnIndices;
        private DynamicDescriptorInstanceContainer _container;
        private string _name;

    }



    public class ColumnIndex : INotifyPropertyChanged, IDynamicDescriptorInstance
    {

        public ColumnIndex()
        {
            this._container = new DynamicDescriptorInstanceContainer(this);
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        [JsonIgnore]
        [Browsable(false)]
        public Index Index { get; set; }


        [ListProvider(typeof(ListProviderSelectColumn))]
        [Browsable(true)]
        [Description("Column identifier")]
        public Guid Id
        {
            get => _id;
            set
            {
                if (_id != value)
                { 
                if (Index != null && value != Guid.Empty)
                {
                    var a = Index.Table.Columns.FirstOrDefault(c => c.Id == value);
                    if (a != null)
                    {
                        _id = a.Id;
                        Name = a.Name;
                        
                    }
                    else
                    {
                        _id = Guid.Empty;
                        Name = null;
                    }
                    
                }
                else
                    _id = Guid.Empty;

                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Id));
                    }
            }
        }

        [Browsable(false)]
        [Description("Column name")]
        public string Name { get; set; }

        public object GetProperty(string name)
        {
            return this._container.GetProperty(name);
        }


        public void SetProperty(string name, object value)
        {
            this._container.SetProperty(name, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Guid _id;
        private readonly DynamicDescriptorInstanceContainer _container;


    }


}
