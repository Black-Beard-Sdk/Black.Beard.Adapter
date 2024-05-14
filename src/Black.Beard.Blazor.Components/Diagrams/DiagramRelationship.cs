
namespace Bb.Diagrams
{

    public class DiagramRelationship
    {

        public DiagramRelationship()
        {
            Properties = new List<Property>();
        }

        public Guid Uuid { get; set; }

        public string Name { get; set; }

        public Guid Type { get; set; }

        public Guid Source { get;  set; }

        public Guid Target { get;  set; }


        public List<Property> Properties { get; set; }

        public void SetProperty(string name, string value)
        {
            var property = Properties.FirstOrDefault(c => c.Name == name);
            if (property == null)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Properties.Add(new Property() { Name = name, Value = value });
                    this.Properties.Sort((x, y) => x.Name.CompareTo(y.Name));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(value))
                    property.Value = value;
                else
                    Properties.Remove(property);
            }
        }

        public string? GetProperty(string name)
        {
            var property = Properties.FirstOrDefault(c => c.Name == name);
            if (property != null)
                return property.Value;
            return null;
        }

    }

}
