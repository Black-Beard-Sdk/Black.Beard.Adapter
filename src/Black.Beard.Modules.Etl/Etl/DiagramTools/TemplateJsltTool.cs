using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Models;

namespace Bb.Modules.Etl
{

    public class TemplateJsltTool : DiagramToolNode
    {

        public TemplateJsltTool()
            : base(Key,
                  Bb.ComponentConstants.Tools,
                  "Template jslt",
                  "transform json",
                  GlyphFilled.Transform)
        {

            AddPort(PortAlignment.Left, PortAlignment.Right);

        }

        public override string GetDefaultName()
        {
            return $"jslt";
        }


        public static Guid Key = new Guid("078884D0-0D27-421E-8332-EA08A8EBAEF5");

    }
}
