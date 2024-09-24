using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Models;

namespace Bb.Modules.Etl
{

    [ExposeClass(EtlDiagramFeature.Filter, ExposedType = typeof(DiagramToolBase))]
    public class FolderTool : DiagramToolNode
    {

        public FolderTool()
            : base(new Guid(Key),
                  Bb.ComponentConstants.Tools,
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

}
