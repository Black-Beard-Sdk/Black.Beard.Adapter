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
using Bb.ComponentModel.Attributes;
using Bb.Toolbars;
using Blazor.Diagrams.Components.Widgets;

namespace Bb.Diagrams
{



    public partial class DiagramUI : ComponentBase, IDisposable, ITranslateHost
    {


        public DiagramUI()
        {

            this._zoomTextChanged = new EventCallback<string>(this, ZoomTextChanged);
            this._gridSizeUpTextChanged = new EventCallback<string>(this, GridSizeTextChanged);
            this._gridShowChanged = new EventCallback<MouseEventArgs>(this, ShowGridChanged);
            this._gridModeChanged = new EventCallback<MouseEventArgs>(this, GridModeChanged);

        }

        #region zoom / GridSize

        public void GridModeChanged(MouseEventArgs args)
        {
            _showPoint = !_showPoint;
            _gridMode = _showPoint ? GridMode.Line : GridMode.Point;
            Diagram.Refresh();
            StateHasChanged();
        }

        public void ShowGridChanged(MouseEventArgs args)
        {

            _showGrid = !_showGrid;
            Diagram.Refresh();
            StateHasChanged();

        }

        /**
            diagram.Options.AllowPanning     = true;
         */

        private void GridSizeTextChanged(string key)
        {
            //_gridMode = GridMode.Line;

            if (_gridSizeValue > 10)
            {
                Diagram.Options.GridSize = _gridSizeValue;
                this.Diagram.Refresh();
            }
        }

        private void ZoomTextChanged(string key)
        {
            Resize();
        }

        private void Resize()
        {
            var d = ((double)_zoomValue / 100d);
            if (this.Diagram.Zoom != d & d > 0)
                this.Diagram.SetZoom(d);
        }

        private void ZoomChanged()
        {
            var z = this.Diagram.Zoom * 100;
            if (_zoomValue != z)
            {
                _zoomValue = (int)z;
                this.Diagram.Refresh();
            }

        }

        #endregion zoom / GridSize

        [EvaluateValidation(false)]
        public Diagnostics Diagnostics { get; set; }

        [EvaluateValidation(false)]
        [Inject]
        public ITranslateService TranslationService { get; set; }

        [EvaluateValidation(false)]
        [Inject]
        public IFocusedService<ToolbarList> GlobalBarFocusService { get; set; }

        [EvaluateValidation(false)]
        [Inject]
        public IFocusedService<PropertyGridView> PropertyGridFocusedService { get; set; }

        [Parameter]
        public Diagram DiagramModel { get; set; }

        [EvaluateValidation(false)]
        [Inject]
        public IBusyService BusyService
        {
            get => _busyService;
            set
            {
                if (_busyService != null)
                    _busyService.BusyChanged -= _busyService_BusyChanged;
                _busyService = value;
                _busyService.BusyChanged += _busyService_BusyChanged;

            }
        }

        [EvaluateValidation(false)]
        private BlazorDiagram Diagram { get; set; } = null!;

        [Parameter]
        public MudExpansionPanel ExpansionDiagnostic { get; set; }

        protected override void OnInitialized()
        {

            _anchorFactory = new AnchorFactory();

            Diagram = CreateDiagram();
            Diagram.PointerClick += PointerClick;
            Diagram.SelectionChanged += SelectionChanged;
            if (DiagramModel != null)
            {
                _linkFactory = new LinkFactory(DiagramModel.Toolbox);
                DiagramModel.Apply(Diagram);
            }
        }


        private void PointerClick(Model? model, Blazor.Diagrams.Core.Events.PointerEventArgs args)
        {
            if (model == null)
                PropertyGridFocusedService.FocusChange(DiagramModel);
            else
                PropertyGridFocusedService.FocusChange(model);
        }

        private void SelectionChanged(SelectableModel model)
        {
            if (model != null)
                PropertyGridFocusedService.FocusChange(model);
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
                    Maximum = 3f,
                    ScaleFactor = 1.1f,
                },
                Links =
                {
                    DefaultRouter = new NormalRouter(),
                    DefaultPathGenerator = new SmoothPathGenerator(),
                    Factory = (diagram, source, targetAnchor) =>
                    {
                        var tool = ToolBar.CurrentClicked?.Tag as DiagramToolRelationshipBase;
                        return _linkFactory.CreateLinkModel(tool, diagram, source, targetAnchor);
                    },

                    TargetAnchorFactory = (diagram, link, model) =>
                    {
                        var tool = ToolBar.CurrentClicked?.Tag as DiagramToolRelationshipBase;
                        return _anchorFactory.CreateLinkModel(tool, diagram, link, model);
                    },
                },
                AllowPanning = true,
                GridSnapToCenter = true,
                GridSize = _zoomValue,
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

            diagram.ZoomChanged += ZoomChanged;

            return diagram;
        }

        public void Save()
        {
            SaveToMyServer(Diagram);
        }

        private async ValueTask SaveToMyServer(Blazor.Diagrams.Core.Diagram diagram)
        {

            if (_session == null)
                _session = BusyService.IsBusyFor(this, "Save", (a) =>
                {

                    // a.Update("Saving...");

                    var diagnostic = new Diagnostics() { Translator = TranslationService };

                    diagnostic.EvaluateModel(Diagram);

                    Diagnostics = diagnostic;
                    if (Diagnostics.Where(c => c.Level == DiagnosticLevel.Error).Any())
                        ExpansionDiagnostic.Expand();

                    foreach (INodeModel node in Diagram.Nodes)
                        node.SynchronizeSource();

                    DiagramModel.LastDiagnostics = diagnostic;
                    DiagramModel?.Save(DiagramModel);

                });

        }



        public ToolBar? ToolBar { get; set; }

        private async Task HandleDragEnter()
        {
            if (ToolBar != null)
            {
                if (ToolBar.CurrentDragStarted != null)
                {
                    dropClass = "can-drop";
                    return;
                }
            }
        }

        private async Task HandleDragLeave()
        {
            dropClass = "";
        }

        private async Task HandleDrop(DragEventArgs args)
        {

            dropClass = string.Empty;
            if (ToolBar != null)
            {
                var i = ToolBar.CurrentDragStarted;
                if (i != null && i.Tag is DiagramToolNode dragedItem)
                {
                    var point1 = Diagram.GetRelativePoint(args.ClientX, args.ClientY);
                    var m1 = DiagramModel.AddModel(dragedItem, point1.X, point1.Y);
                    StateHasChanged();
                    return;
                }
            }

        }


        private void _busyService_BusyChanged(object? sender, BusyEventArgs e)
        {
            if (e.Source == _session)
            {
                if (e.Source.BusyStatus == BusyEnum.Completed)
                    _session = null;
                // StateHasChanged();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Diagram != null)
                    {
                        Diagram.PointerClick -= PointerClick;
                        Diagram.SelectionChanged -= SelectionChanged;
                    }

                    //if (PropertyGridFocusedService != null)
                    //    PropertyGridFocusedService.FocusChanged -= FocusedService_FocusChanged;

                    if (_busyService != null)
                        _busyService.BusyChanged -= _busyService_BusyChanged;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {

                var t = DiagramModel.GetToolbar();

                this.GlobalBarFocusService.FocusChange(this.DiagramModel
                    , (a, b) => true
                    , (a, b) =>
                {
                    this.ToolBar = a.Component as ToolBar;
                    var diagram = (Diagram)b;
                    a.ApplyChange(diagram.GetToolbar());
                });

            }

        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }

        private IBusyService _busyService;
        private string dropClass = "";
        private LinkFactory _linkFactory;
        private AnchorFactory _anchorFactory;
        private PropertyGridView PropertyGrid;
        private bool disposedValue;
        private BusySession _session;


        private int _zoomValue = 100;
        private int _gridSizeValue = 20;
        private bool _showPoint = false;
        private bool _showGrid = true;
        private GridMode _gridMode = GridMode.Point;
        private readonly EventCallback<string> _zoomTextChanged;
        private readonly EventCallback<string> _gridSizeUpTextChanged;
        private readonly EventCallback<MouseEventArgs> _gridShowChanged;
        private readonly EventCallback<MouseEventArgs> _gridModeChanged;
    }


}
