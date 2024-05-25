using Bb.Diagrams;
using Bb.Generators;
using Bb.Modules.Sgbd.DiagramTools;
using Bb.TypeDescriptors;
using Bb.UIComponents;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security;
using System.ComponentModel;
using Blazor.Diagrams;
using Bb.CustomComponents;

namespace Bb.Modules.Sgbd.Models
{

    public class SgbdTechnology
    {


        static SgbdTechnology()
        {

            var config = DynamicTypeDescriptionProvider.Configuration;
            config.Add<Table>(c =>
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

            config.Add<Column>(c =>
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


            config.Add<SgbdDiagram>(c =>
            {
                c.Property(u => u.Models, i =>
                {
                    i.DisableBrowsable();
                });

                c.Property(u=> u.Relationships, i =>
                {
                    i.DisableBrowsable();
                });

            });


            config.Add<BlazorDiagram>(c =>
            {
              
                c.Property(u => u.SuspendRefresh, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u=> u.SuspendSorting, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.Container, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.Controls, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.Options, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.Pan, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.Zoom, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.OrderedSelectables, i =>
                {
                    i.DisableValidation();
                });


            }); 
            
        }


        public Generator GetGenerator(ContextGenerator context)
        {

            if (_generator == null)
            {

                _generator = new Generator()
                {
                    Context = context,
                }

                .AddRazorTemplate<SgbdDiagram>(".sql", c =>
                {

                    c.Configure(d =>
                    {

                    });

                    c.GetModels(
                        c => c.Models.OfType<DiagramNode>().Where(c => c.Type == new Guid(TableTool.Key)),
                        c => c.Name,
                        c => "Tables"
                        );

                    c.WithTemplateFromResource("Bb.Modules.Sgbd.Templates.Table.cshtml");

                });

            }

            return _generator;

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
