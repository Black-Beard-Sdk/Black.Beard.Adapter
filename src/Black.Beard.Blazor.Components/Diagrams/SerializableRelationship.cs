using Bb.ComponentModel.Accessors;
using Bb.ComponentModel.Attributes;
using Bb.TypeDescriptors;
using ICSharpCode.Decompiler.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Bb.Diagrams
{


    /// <summary>
    /// diagram link object specialized for serialization
    /// </summary>
    public class SerializableRelationship
        : INotifyPropertyChanging
        , INotifyPropertyChanged
        , IKey
    {

        /// <summary>
        /// 
        /// </summary>
        public SerializableRelationship()
        {
            Properties = new Properties();
            _realProperties = this.GetType().GetAccessors(AccessorStrategyEnum.Direct);
            _options = new JsonSerializerOptions
            {
                Converters = { new DynamicDescriptorInstanceJsonConverter() },
                // Other options as required
                IncludeFields = true,  // You must set this if MyClass.Id and MyClass.Data are really fields and not a property.
                WriteIndented = true
            };
        }


        /// <summary>
        /// Unique identifier of the link
        /// </summary>
        [Required]
        public Guid Uuid
        {
            get => _uid;
            set
            {
                if (_uid != value)
                {
                    OnPropertyChanging(nameof(Uuid));
                    _uid = value;
                    OnPropertyChanged(nameof(Uuid));
                }
            }
        }

        /// <summary>
        /// Name / label of the link
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    OnPropertyChanging(nameof(Name));
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Type of node
        /// </summary>
        [Required]
        public Guid Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    OnPropertyChanging(nameof(Type));
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        /// <summary>
        /// unique id of the source object
        /// </summary>
        public Guid Source
        {
            get => _source;
            set
            {
                if (_source != value)
                {
                    OnPropertyChanging(nameof(Source));
                    _source = value;
                    OnPropertyChanged(nameof(Source));
                }
            }
        }

        /// <summary>
        /// Unique id of the target object
        /// </summary>
        public Guid Target
        {
            get => _target;
            set
            {
                if (_target != value)
                {
                    OnPropertyChanging(nameof(Target));
                    _target = value;
                    OnPropertyChanged(nameof(Target));
                }
            }
        }

        #region Dynamic properties

        [EvaluateValidation(false)]
        public Properties Properties
        {
            get => _properties;
            set
            {
                if (_properties != null)
                    _properties.Merge(value);

                else
                {
                    OnPropertyChanging(nameof(Properties));
                    _properties = value;
                    OnPropertyChanged(nameof(Properties));
                    _properties.PropertyChanging += _properties_PropertyChanging;
                    _properties.PropertyChanged += _properties_PropertyChanged;
                }
            }
        }

        private void _properties_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void _properties_PropertyChanging(object? sender, PropertyChangingEventArgs e)
        {
            PropertyChanging?.Invoke(this, e);
        }

        public void SetProperty(string name, object? value)
        {

            if (_realProperties.TryGetValue(name, out var accessor))
                accessor.SetValue(this, value);

            else
            {
                var valueString = value?.Serialize(_options);
                Properties.SetProperty(name, valueString);
            }

        }

        public object? GetProperty(string name)
        {

            if (_realProperties.TryGetValue(name, out var accessor))
                return accessor.GetValue(this);

            return Properties.GetProperty(name);
        }

        public virtual void CopyFrom(DynamicDescriptorInstanceContainer container)
        {

            var properties = container.Properties()
                .Where(c => !c.IsReadOnly)
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var item in properties)
            {
                var v = item.GetValue(container.Instance);
                SetProperty(item.Name, v);
            }

        }

        #endregion Dynamic properties

        [JsonIgnore]
        public Diagram Diagram { get; internal set; }


        #region OnChange

        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion OnChange


        private readonly AccessorList _realProperties;
        protected JsonSerializerOptions _options;
        private Guid _uid;
        private Guid _type;
        private string _name;
        private Guid _source;
        private Guid _target;
        private Properties _properties;
    }

}
