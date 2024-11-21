using Bb.TypeDescriptors;
using System.ComponentModel;
using System.Text.Json;

namespace Bb.Diagrams
{


    public class Properties : List<Property>
        , INotifyPropertyChanging
        , INotifyPropertyChanged
    {

        public Properties()
        {

        }

        public void SetProperty(string name, string? value)
        {
            var property = this.FirstOrDefault(c => c.Name == name);
            if (property == null)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (SetValue(name, value))
                        this.Sort((x, y) => x.Name.CompareTo(y.Name));
                }
            }
            else
            {
                if (!SetValue(name, value))
                {
                    OnPropertyChanging(name);
                    Remove(property);
                    OnPropertyChanged(name);
                }
            }
        }

        private bool SetValue(string name, string? value)
        {

            if (!string.IsNullOrEmpty(value))
            {
                OnPropertyChanging(name);
                Add(new Property() { Name = name, Value = value });
                OnPropertyChanged(name);
                return true;
            }

            return false;

        }

        private void SetProperty(Property item)
        {
            SetProperty(item.Name, item.Value);
        }


        public string? GetProperty(string name)
        {
            var property = this.FirstOrDefault(c => c.Name == name);
            if (property != null)
                return property.Value;
            return null;
        }


        public bool PropertyExists(string name)
        {
            return this.Any(c => c.Name == name);
        }


        public void CopyFrom(DynamicDescriptorInstanceContainer container)
        {

            var options = new JsonSerializerOptions
            {
                Converters = { new DynamicDescriptorInstanceJsonConverter() },
                // Other options as required
                IncludeFields = true,  // You must set this if MyClass.Id and MyClass.Data are really fields and not a property.
                WriteIndented = true
            };

            HashSet<string> _h = new HashSet<string>(this.Select(c => c.Name));

            var properties = container.Properties()
                .Where(c => !c.IsReadOnly)
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var item in properties)
            {

                if (_h.Contains(item.Name))
                    _h.Remove(item.Name);

                var value = item.GetValue(container.Instance);
                SetProperty(item.Name, value?.Serialize(options));

            }

            foreach (var item in _h)
                SetProperty(item, null);

        }


        #region OnChange

        protected void OnPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void Merge(Properties value)
        {
            foreach (var item in value)
                SetProperty(item);
        }

        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;


        #endregion OnChange


    }

}
