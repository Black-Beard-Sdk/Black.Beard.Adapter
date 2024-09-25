using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.Modules.bpms.Components;
using Bb.Modules.Bpms.Models;
using Bb.Modules.Etl.Models;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Geometry;

namespace Bb.Modules.Bpms
{

    public class SwimLaneTool : DiagramToolNode
    {

        public SwimLaneTool()
            : base(Key,
                  Bb.ComponentConstants.Tools,
                  "swimlane",
                  "Append a new swimlane",
                  GlyphFilled.Service)
        {
            this.SetTypeModel<BpmsSwimLane>();
            this.SetTypeUI<SwimLaneComponent>();
            this.IsControlled(true);
            this.IsLocked(false);
        }

        protected override void CustomizeNode(IDiagramNode node, Diagram diagram)
        {

            double y = 0;
            double x = 0d;

            IDiagramNode last = null;

            foreach (var item in diagram.Models)
                if (item != node)
                    if (item.Position.Y >= y)
                    {
                        y = item.Position.Y;
                        last = item;
                    }

            if (last != null)
                y = last.Position.Y + height + spacing;

            node.Position = new Position() { X = x, Y = y };
            node.Size = new Size(2000, height);

        }

        public override string GetDefaultName()
        {
            return $"swimlane";
        }

        public static Guid Key = new Guid("6A317FDD-F2D6-4BA1-9A08-071AA11C414B");

        private double spacing = 1d;
        private double height = 300d;

    }

}
