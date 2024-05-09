using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Blazor.Diagrams;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Behaviors;
using MudBlazor;
using Blazor.Diagrams.Core.Events;
using Blazor.Diagrams.Core.Models.Base;
using Microsoft.AspNetCore.Components.Web;
using Blazor.Diagrams.Core.Options;
using ICSharpCode.Decompiler.Metadata;
using Blazor.Diagrams.Core;

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

            _linkFactory = new LinkFactory(DiagramModel.Specifications);
            _anchorFactory = new AnchorFactory();

            Diagram = CreateDiagram();
            DiagramModel.Apply(Diagram);
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
                    DefaultPathGenerator = new SmoothPathGenerator(),
                    Factory = (diagram, source, targetAnchor) => _linkFactory.CreateLinkModel(Toolbox.CurrentLink, diagram, source, targetAnchor),
                    TargetAnchorFactory = (diagram, link, model) => _anchorFactory.CreateLinkModel(Toolbox.CurrentLink, diagram, link, model),
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

            foreach (var node in Diagram.Nodes)
                if (node is CustomizedNodeModel model)
                    model.Synchronize();

            DiagramModel?.Save(DiagramModel);

        }


        private async Task HandleDragEnter()
        {
            var dragItem = _toolbox.CurrentDragStarted;
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
            var dragItem = _toolbox.CurrentDragStarted as DiagramSpecificationNodeBase;
            if (dragItem == null) return;
            var point = Diagram.GetRelativePoint(args.ClientX, args.ClientY);
            DiagramModel.AddModel(dragItem, point.X, point.Y);
            StateHasChanged();

            _toolbox.Tools.EnsureCategoryIsShown(dragItem);

        }

        private string dropClass = "";
        private Toolbox _toolbox;
        private ToolboxList _toolboxList;
        private LinkFactory _linkFactory;
        private AnchorFactory _anchorFactory;
    }

}
