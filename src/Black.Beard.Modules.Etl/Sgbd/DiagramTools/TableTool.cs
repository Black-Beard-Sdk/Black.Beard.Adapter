using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.Modules.Sgbd.Components;
using Bb.Modules.Sgbd.Models;
using Bb.UIComponents.Glyphs;


namespace Bb.Modules.Sgbd.DiagramTools
{
    [ExposeClass(SgbdDiagramFeature.Filter, ExposedType = typeof(DiagramSpecificationBase))]
    public class TableTool : DiagramSpecificationNodeBase
    {

        public TableTool()
            : base(new Guid(Key),
                  "Table structure",
                  "Add a new table",
                  GlyphFilled.TableRows)
        {

            // AddPort(PortAlignment.Right);

            this.SetTypeModel<Table>();
            this.SetTypeUI<TableNode>();

        }


        public override DiagramNode CreateModel(double x, double y, string name, Guid? uuid = null)
        {
            return base.CreateModel(x, y, name, uuid);
        }

        public override CustomizedNodeModel CreateUI(DiagramNode model)
        {
            return base.CreateUI(model);
        }

        public override CustomizedNodeModel CreateUI(double x, double y, string name)
        {
            return base.CreateUI(x, y, name);
        }

        protected override DiagramNode Create()
        {
            return base.Create();
        }



        public override string GetDefaultName()
        {
            return $"table";
        }

        public const string Key = "1127349B-3992-45D3-9536-D6790911D374";

    }

}
