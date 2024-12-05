using Bb.Commands;
using Bb.TypeDescriptors;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bb.Diagrams
{


    public class Properties
        : List<Property>
        , IRestorableModel
        , INotifyPropertyChanging
        , INotifyPropertyChanged
    {

        [JsonIgnore]
        public Guid Uuid { get; private set; }

        internal void SetUuid(Guid value)
        {
            Uuid = value;
        }

        public Properties()
        {

        }

        public void SetProperty(string name, string? value)
        {

            var toChange = ValueAsDifferentOf(name, value);
            if (toChange)
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

        public bool TryGetValue(string name, out Property? property)
        {
            property = this.FirstOrDefault(c => c.Name == name);
            return property != null;
        }

        public bool ValueAsDifferentOf(string name, string? value)
        {
            return this.Any(c => c.Name == name && c.Value == value);
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

        public bool Restore(object model, RefreshContext context, RefreshStrategy strategy = RefreshStrategy.All)
        {

            bool result = false;

            var m = model as Properties;

            if (strategy.HasFlag(RefreshStrategy.Remove))
            {
                var l = this.ToList();
                foreach (var item in l)
                    if (!m.PropertyExists(item.Name))
                    {
                        OnPropertyChanging(item.Name);
                        Remove(item);
                        OnPropertyChanged(item.Name);
                        result = true;
                    }
            }

            if (strategy.HasFlag(RefreshStrategy.Update))
                foreach (var item in m)
                    if (TryGetValue(item.Name, out var value))
                        if (item.Value != value.Value)
                        {
                            OnPropertyChanging(item.Name);
                            SetProperty(item);
                            OnPropertyChanged(item.Name);
                            result = true;
                        }

            if (strategy.HasFlag(RefreshStrategy.Add))
                foreach (var item in m)
                    if (!this.PropertyExists(item.Name))
                    {
                        OnPropertyChanging(item.Name);
                        Add(item);
                        OnPropertyChanged(item.Name);
                        result = true;
                    }

            if (result)
                context.Add(this.Uuid, null, RefreshStrategy.Update);

            return result;

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

        public override bool Equals(object? obj)
        {

            if (obj is Properties p)
            {
                if (p.Count == this.Count)
                {
                    foreach (var item in this)
                    {
                        if (!p.PropertyExists(item.Name))
                            return false;
                        if (p.GetProperty(item.Name) != item.Value)
                            return false;
                    }
                    return true;
                }
            }

            return base.Equals(obj);
        }

    }

}
