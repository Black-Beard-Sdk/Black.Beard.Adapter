using Microsoft.AspNetCore.Components;
using Blazor.Diagrams;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;
using Blazor.Diagrams.Core.Behaviors;
using Microsoft.AspNetCore.Components.Web;
using Bb.PropertyGrid;
using Blazor.Diagrams.Core.Models.Base;
using Bb.ComponentModel.Translations;
using MudBlazor;

namespace Bb.Diagrams
{

    public partial class DiagramUI : ComponentBase, IDisposable, ITranslateHost
    {



        public Diagnostics Diagnostics { get; set; }


        [Inject]
        public ITranslateService TranslationService { get; set; }

        [Inject]
        public IFocusedService FocusedService { get; set; }

        [Parameter]
        public Diagram DiagramModel { get; set; }


        [Parameter]
        public MudExpansionPanel ExpansionDiagnostic { get; set; }

        public ToolboxList Toolbox { get => _toolboxList ?? (_toolboxList = new ToolboxList(DiagramModel.Specifications)); }


        private BlazorDiagram Diagram { get; set; } = null!;


        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                //PropertyGrid.PropertyFilter = (p) =>
                //{
                //    if (p.Browsable)
                //        return true;
                //    return false;
                //};

            }

        }

        protected override void OnInitialized()
        {

            _linkFactory = new LinkFactory(DiagramModel.Specifications);
            _anchorFactory = new AnchorFactory();

            Diagram = CreateDiagram();
            Diagram.PointerClick += PointerClick;
            Diagram.SelectionChanged += SelectionChanged;
            FocusedService.FocusChanged += FocusedService_FocusChanged;
            DiagramModel.Apply(Diagram);
        }

        private void FocusedService_FocusChanged(object? sender, EventArgs e)
        {
            this.PropertyGrid.SelectedObject = sender;
        }

        private void PointerClick(Model? model, Blazor.Diagrams.Core.Events.PointerEventArgs args)
        {
            if (model == null)
                FocusedService.FocusChange(DiagramModel);
            else
                FocusedService.FocusChange(model);
        }

        private void SelectionChanged(SelectableModel model)
        {
            if (model != null)
                FocusedService.FocusChange(model);
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

            var diagnostic = new Diagnostics() { Translator = TranslationService };

            diagnostic.EvaluateModel(Diagram);

            Diagnostics = diagnostic;
            if (Diagnostics.Where(c => c.Level == DiagnosticLevel.Error).Any())
                ExpansionDiagnostic.Expand();

            foreach (var node in Diagram.Nodes)
                if (node is CustomizedNodeModel model)
                    model.SynchronizeSource();

            DiagramModel.LastDiagnostics = diagnostic;
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

        private PropertyGridView PropertyGrid;
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FocusedService.FocusChanged -= FocusedService_FocusChanged;
                    // TODO: supprimer l'état managé (objets managés)
                }

                // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
                // TODO: affecter aux grands champs une valeur null
                disposedValue = true;
            }
        }

        // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
        // ~DiagramUI()
        // {
        //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }


}
