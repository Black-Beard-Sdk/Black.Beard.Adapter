
using Bb.TypeDescriptors;
using System.Text.Json;

namespace Bb.Diagrams
{


    public class Properties : List<Property>
    {

        public Properties()
        {
            
        }


        public void SetProperty(string name, string value)
        {
            var property = this.FirstOrDefault(c => c.Name == name);
            if (property == null)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Add(new Property() { Name = name, Value = value });
                    this.Sort((x, y) => x.Name.CompareTo(y.Name));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(value))
                    property.Value = value;
                else
                    Remove(property);
            }
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
                IncludeFields = true,  // You must set this if MyClass.Id and MyClass.Data are really fields not properties.
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

                //var value = container.GetProperty(item.Name);
                if (value != null)
                    SetProperty(item.Name, value.Serialize(options));
                else
                    SetProperty(item.Name, null);

            }

            foreach (var item in _h)
                SetProperty(item, null);

        }


    }

}
