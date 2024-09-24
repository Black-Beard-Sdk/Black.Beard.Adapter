using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.Modules.Sgbd.Components;
using Bb.Modules.Sgbd.Models;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Models;


namespace Bb.Modules.Sgbd.DiagramTools
{

    [ExposeClass(SgbdDiagramFeature.Filter, ExposedType = typeof(DiagramToolBase))]
    public class TableTool : DiagramToolNode
    {

        public TableTool()
            : base(new Guid(Key),
                  Bb.ComponentConstants.Tools,
                  "Table structure",
                  "Add a new table",
                  GlyphFilled.TableRows)
        {

            // AddPort(PortAlignment.Right);

            this.SetTypeModel<Table>();
            this.SetTypeUI<TableNode>();

        }

        public override string GetDefaultName()
        {
            return $"table";
        }

        public const string Key = "1127349B-3992-45D3-9536-D6790911D374";

    }

}
