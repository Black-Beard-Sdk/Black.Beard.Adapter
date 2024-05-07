using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Blazor.Diagrams;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Behaviors;
using MudBlazor;
using Blazor.Diagrams.Core.Events;
using Blazor.Diagrams.Core.Models.Base;
using Microsoft.AspNetCore.Components.Web;

namespace Bb.Diagrams
{

    public partial class DiagramUI : ComponentBase
    {



        [Parameter]
        public Diagram DiagramModel { get; set; }


        public ToolboxList Toolbox { get => _toolboxList ?? (_toolboxList = new ToolboxList(DiagramModel.Specifications)); }

        private BlazorDiagram Diagram { get; set; } = null!;

        protected override void OnInitialized()
        {

            Diagram = CreateDiagram();

            var specModel = DiagramModel.Specifications.OfType<DiagramSpecificationModelBase>().First(c => c.Kind == ToolKind.Node);
            var node1 = DiagramModel.AddModel(specModel, 50, 50);
            var node2 = DiagramModel.AddModel(specModel, 100, 200, "Node 2");

            var specLink = DiagramModel.Specifications.OfType<DiagramSpecificationRelationshipBase>().First(c => c.Kind == ToolKind.Link);
            DiagramModel.AddLink(specLink, node1.GetPort(PortAlignment.Right), node2.GetPort(PortAlignment.Left), "Link 1");


            DiagramModel.Apply(Diagram);

            //var sourceAnchor = new ShapeIntersectionAnchor(firstNode);

        }

        private BlazorDiagram CreateDiagram()
        {
            var options = new BlazorDiagramOptions
            {
                AllowMultiSelection = true,
                Zoom =
                {
                    Enabled = true,
                    Minimum = 0.1f,
                    Maximum = 4f,
                    ScaleFactor = 1.1f,
                },
                Links =
                {
                    DefaultRouter = new NormalRouter(),
                    DefaultPathGenerator = new SmoothPathGenerator()
                },
                AllowPanning = true,
                GridSnapToCenter = true,
                GridSize = 20,
                //Virtualization =
                //{
                //    Enabled = true, 
                //    OnNodes = true, 
                //    OnLinks = true,
                //    OnGroups = true,
                //}
            };

            var diagram = new BlazorDiagram(options);

            var ksb = diagram.GetBehavior<KeyboardShortcutsBehavior>();
            ksb.SetShortcut("s", ctrl: false, shift: true, alt: false, SaveToMyServer);

            return diagram;

        }

        public void Save()
        {
            SaveToMyServer(Diagram);
        }

        private async ValueTask SaveToMyServer(Blazor.Diagrams.Core.Diagram diagram)
        {


        }


        string dropClass = "";
        private async Task HandleDragEnter()
        {
            var dragItem = _toolbox.LastDrag;
            if (dragItem == null) return;
            //    dropClass = "no-drop";
            dropClass = "can-drop";
        }

        private async Task HandleDragLeave()
        {
            dropClass = "";
        }

        private async Task HandleDrop(DragEventArgs args)
        {
            dropClass = "";
            var dragItem = _toolbox.LastDrag as DiagramSpecificationModelBase;
            if (dragItem == null) return;
            var point = Diagram.GetRelativePoint(args.ClientX, args.ClientY);
            DiagramModel.AddModel(dragItem.Uuid, point.X, point.Y);
            StateHasChanged();

            _toolbox.Tools.EnsureCategoryIsShown(dragItem);

        }

        private Toolbox _toolbox;
        ToolboxList _toolboxList;

    }



    public class CustomizedNodeModel : NodeModel
    {


        public CustomizedNodeModel(DiagramItemBase source)
            : this(source.Uuid.ToString(), new Point(source.Position.X, source.Position.Y))
        {
            this.Source = source;
            this.Title = source.Name;
            foreach (var port in source.Ports)
                AddPort(new PortModel(port.Uuid.ToString(), this, port.Alignment));
        }

        public CustomizedNodeModel(Point? position = null)
      : base(position)
        {

        }

        public CustomizedNodeModel(string id, Point? position = null)
            : base(id, position)
        {

        }

        public override void SetPosition(double x, double y)
        {
            base.SetPosition(x, y);
        }

        public DiagramItemBase Source { get; }

    }

}
