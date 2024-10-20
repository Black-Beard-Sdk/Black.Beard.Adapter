using Bb.ComponentModel.Translations;
using Bb.Diagrams;
using Bb.Modules.Bpms.Models;
using Bb.Modules.Etl.Models;
using Bb.PropertyGrid;
using Bb.TypeDescriptors;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bb.Modules.bpms.Components
{

    public partial class SwimLaneComponent : ComponentBase
    {

        public SwimLaneComponent()
        {

        }

        //[Inject]
        //private IJSRuntime Runtime { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }


        [Inject]
        public IRefreshService RefreshService { get; set; }


        [Parameter]
        public NodeModel? Node { get; set; }

        public BpmsSwimLane? Item => Node as BpmsSwimLane;

        public string? Name
        {
            get => Node?.Title;
            set
            {
                if (Node != null)
                {
                    Node.Title = value;
                    RefreshService.CallRequestRefresh(this, nameof(PropertyGridView));
                }
            }
        }

        public double Height
        {
            get => Item?.Size?.Height - 2d ?? 10d;
        }

        public int GetDecaleY(int w)
        {
            return (int)GetDecale(w) + 11;
        }

        public int GetDecaleX(int w)
        {
            return (int)GetDecale(w) + 16;
        }

        private int GetDecale(int w)
        {
            var ox = (w / 2);
            var oy = (Item.Size.Height / 2);
            var decale = oy - ox;
            return (int)decale;
        }

        public int HalfWidth
        {
            get
            {
                var p = Item.Position;
                var p2 = Item.Source.GetDiagram<Diagram>().GetScreenPoint(p.X, p.Y);
                return (int)p2.X;
            }
        }

        public double Width
        {
            get => Item?.Size?.Width ?? 10d;
        }

        // private ElementReference _element;

    }
}
