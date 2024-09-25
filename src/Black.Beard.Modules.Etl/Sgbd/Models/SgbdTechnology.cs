using Bb.Diagrams;
using Bb.Generators;
using Bb.Modules.Sgbd.DiagramTools;
using Bb.TypeDescriptors;
using Blazor.Diagrams;
using System.Linq;

namespace Bb.Modules.Sgbd.Models
{

    public class SgbdTechnology
    {


        static SgbdTechnology()
        {

            DynamicTypeDescriptionProvider.Configure<Table>(c =>
            {

                c.RemoveProperties("ControlledSize", "Title", "Selected", "Id", "Locked", "Visible");

                c.Property(c => c.Name, i =>
                {
                    i.PropertyOrder(1)
                    ;
                });

                c.Property(u => u.Group, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.HasPrimaryColumn, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.Links, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.PortLinks, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.Ports, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.Position, i =>
                {
                    i.DisableValidation();
                });

            });

            DynamicTypeDescriptionProvider.Configure<Column>(c =>
            {

                c.Property(c => c.Name, i =>
                {
                    i.PropertyOrder(1)
                    ;
                })
                .Property(c => c.Type, i =>
                {
                    i.PropertyOrder(2)
                    ;
                });
            });


            DynamicTypeDescriptionProvider.Configure<SgbdDiagram>(c =>
            {
                c.Property(u => u.Models, i =>
                {
                    i.DisableBrowsable();
                });

                c.Property(u => u.Relationships, i =>
                {
                    i.DisableBrowsable();
                });

            });

        }


        public Generator GetGenerator(ContextGenerator context)
        {

            if (_generator == null)
            {
                _generator = ConfigureGenerator( new Generator()
                {
                    Context = context,
                });
            }

            return _generator;

        }

        protected virtual Generator ConfigureGenerator(Generator generator)
        {
            
            return generator;

        }

        protected SgbdTechnology(string name, string description)
        {
            this.Name = name;
            this.Description = description;
            _columnTypes = new List<ColumnType>();
        }

        protected ColumnType AddColumnType(string label, string code, ColumbTypeCategory category)
        {
            var type = new ColumnType(label, code, category);
            _columnTypes.Add(type);
            return type;
        }


        public ColumnType DefaultColumnType { get; private set; }

        protected ColumnType IsDefaultColumnType(ColumnType columnType)
        {
            DefaultColumnType = columnType;
            return columnType;
        }

        public IEnumerable<ColumnType> ColumnTypes => _columnTypes.ToArray();

        public string Name { get; }

        public string Description { get; }

        public SgbdTechnologies Parent { get; internal set; }

        private List<ColumnType> _columnTypes;
        private Generator _generator;

    }

}
