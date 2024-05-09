using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Models;

namespace Bb.Modules.Etl.DiagramTools
{


    [ExposeClass(EtlDiagramFeature.Filter, ExposedType = typeof(DiagramSpecificationBase))]
    public class WebServiceTool : DiagramSpecificationNodeBase
    {

        public WebServiceTool()
            : base(new Guid(Key),
                  "WebService",
                  "Append a new WebService",
                  GlyphFilled.Service)
        {

            AddPort(PortAlignment.Left, PortAlignment.Right);

        }

        public override string GetDefaultName()
        {
            return $"ws";
        }


        public const string Key = "802E6C01-E521-40CB-AE28-B35375974F21";

    }

    [ExposeClass(EtlDiagramFeature.Filter, ExposedType = typeof(DiagramSpecificationBase))]
    public class FolderTool : DiagramSpecificationNodeBase
    {

        public FolderTool()
            : base(new Guid(Key),
                  "Parse folder",
                  "Parse folder and filter file",
                  GlyphFilled.Folder)
        {

            AddPort(PortAlignment.Right);

        }

        public override string GetDefaultName()
        {
            return $"folder";
        }


        public const string Key = "D9342824-E4CD-49E5-8BFB-410C7068F095";

    }

    [ExposeClass(EtlDiagramFeature.Filter, ExposedType = typeof(DiagramSpecificationBase))]
    public class TemplateJsltTool : DiagramSpecificationNodeBase
    {

        public TemplateJsltTool()
            : base(new Guid(Key),
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


        public const string Key = "078884D0-0D27-421E-8332-EA08A8EBAEF5";

    }
}
