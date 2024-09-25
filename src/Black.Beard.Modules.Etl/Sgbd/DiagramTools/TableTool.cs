using Bb.Diagrams;
using Bb.Modules.Sgbd.Components;
using Bb.Modules.Sgbd.Models;
using Bb.UIComponents.Glyphs;


namespace Bb.Modules.Sgbd.DiagramTools
{


    public class TableTool : DiagramToolNode
    {

        public TableTool()
            : base(Key,
                  ComponentConstants.Tools,
                  "Table structure",
                  "Add a new table",
                  GlyphFilled.TableRows)
        {
            this.SetTypeModel<Table>();
            this.SetTypeUI<TableNode>();
        }

        public override string GetDefaultName()
        {
            return $"table";
        }

        public static Guid Key = new Guid("1127349B-3992-45D3-9536-D6790911D374");

    }

}
