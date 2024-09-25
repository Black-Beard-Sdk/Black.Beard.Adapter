using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Models;

namespace Bb.Modules.Etl
{

    public class FolderTool : DiagramToolNode
    {

        public FolderTool()
            : base(Key,
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


        public static Guid Key = new Guid("D9342824-E4CD-49E5-8BFB-410C7068F095");

    }

}
