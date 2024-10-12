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
using Blazor.Diagrams.Core.Models;

namespace Bb.Diagrams
{



    public partial class DiagramUI : ComponentBase, IDisposable, ITranslateHost, IEventArgInterceptor<PropertyObjectDescriptorEventArgs>
    {


        public DiagramUI()
        {

            _zoomTextChanged = new EventCallback<string>(this, ZoomTextChanged);
            _gridSizeUpTextChanged = new EventCallback<string>(this, GridSizeTextChanged);
            _gridShowChanged = new EventCallback<MouseEventArgs>(this, ShowGridChanged);
            _gridModeChanged = new EventCallback<MouseEventArgs>(this, GridModeChanged);

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
                Diagram.Refresh();
            }
        }

        private void ZoomTextChanged(string key)
        {
            Resize();
        }
        private void Resize()
        {
            var d = _zoomValue / 100d;
            if (Diagram.Zoom != d & d > 0)
                Diagram.SetZoom(d);
        }
        private void ZoomChanged()
        {
            var z = Diagram.Zoom * 100;
            if (_zoomValue != z)
            {
                _zoomValue = (int)z;
                Diagram.Refresh();
            }

        }

        #endregion zoom / GridSize

        [EvaluateValidation(false)]
        public DiagramDiagnostics Diagnostics { get; set; }

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
            Diagram = CreateDiagram();
            Diagram.PointerClick += PointerClick;
            Diagram.SelectionChanged += SelectionChanged;
            if (DiagramModel != null)
                DiagramModel.Apply(Diagram);
        }


        //private static System.Timers.Timer _timer;
        protected override Task OnInitializedAsync()
        {
            //StartTimer();
            return base.OnInitializedAsync();
        }

        //public void StartTimer()
        //{
        //    _timer = new System.Timers.Timer(1000);
        //    _timer.Elapsed += CountDownTimer;
        //    _timer.Enabled = true;
        //}

        //public void CountDownTimer(Object source, System.Timers.ElapsedEventArgs e)
        //{
        //    _timer.Enabled = false;
        //    DiagramModel?.Prepare();
        //    StateHasChanged();
        //}

        private void PointerClick(Model? model, Blazor.Diagrams.Core.Events.PointerEventArgs args)
        {
            if (model == null)
                Select(DiagramModel);
            else
                Select(model);
        }

        private void SelectionChanged(SelectableModel model)
        {
            Select(model);
        }


        private void Select(object model)
        {

            if (model != null)
                PropertyGridFocusedService.FocusChange
                (
                    model,
                    (propertyGrid, sender) => true,
                    (propertyGrid, sender) =>
                    {
                        propertyGrid.Raise(this);

                    }

                );

        }

        public void Invoke(object sender, PropertyObjectDescriptorEventArgs eventArgs)
        {
            if (eventArgs.Instance is NodeModel n)
                n.Refresh();
        }


        private BlazorDiagram CreateDiagram()
        {

            var options = new BlazorDiagramOptions
            {
                AllowMultiSelection = true,
                Zoom =
                {
                    Enabled = false,
                    Minimum = 0.1f,
                    Maximum = 3f,
                    ScaleFactor = 1.1f,
                },
                Links =
                {

                    DefaultRouter = new OrthogonalRouter(),
                    DefaultPathGenerator = new StraightPathGenerator(),
                    EnableSnapping = true,

                    //SnappingRadius = 10,
                    //DefaultColor = "#000000",
                    //DefaultSelectedColor = "#ff0000",
                    //RequireTarget = true,

                    Factory = (diagram, source, targetAnchor) =>
                    {

                        LinkProperties link = null;

                        var toolLink = ToolBar?.GetLink(source);
                        if (toolLink != null)
                            link = this.DiagramModel.CreateLink(toolLink, source, targetAnchor);

                        return link?.UILink;

                    },

                    TargetAnchorFactory = (diagram, link, model) =>
                    {
                        var toolLink = ToolBar?.GetLink(link);
                        if (toolLink != null)
                            return link.ConvertToAnchor(toolLink);
                        return null;
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

            // options.Constraints

            var diagram = new BlazorDiagram(options);

            var ksb = diagram.GetBehavior<KeyboardShortcutsBehavior>();
            ksb.SetShortcut("s", ctrl: false, shift: true, alt: false, Save);

            diagram.ZoomChanged += ZoomChanged;

            return diagram;

        }

        public void Save()
        {
            Save(Diagram);
        }

        private async ValueTask Save(Blazor.Diagrams.Core.Diagram diagram)
        {

            if (_session == null)
                _session = BusyService.IsBusyFor(this, "Save", (a) =>
                {

                    // a.Update("Saving...");

                    var diagnostic = new DiagramDiagnostics() { Translator = TranslationService };

                    diagnostic.EvaluateModel(Diagram);

                    Diagnostics = diagnostic;
                    if (Diagnostics.Where(c => c.Level == DiagnosticLevel.Error).Any())
                        ExpansionDiagnostic.ExpandAsync();

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
                if (ToolBar.CurrentDragStarted != null)
                {
                    dropClass = "can-drop";
                    return;
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
                    var point1 = Diagram.GetRelativeMousePoint(args.ClientX, args.ClientY);
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

                if (DiagramModel != null)
                {
                    var t = DiagramModel.GetToolbar();

                    GlobalBarFocusService.FocusChange(DiagramModel
                        , (a, b) => true
                        , (a, b) =>
                    {
                        ToolBar = a.Component as ToolBar;
                        var diagram = (Diagram)b;
                        a.ApplyChange(diagram.GetToolbar());
                    });
                }

            }

        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }

        private IBusyService _busyService;
        private string dropClass = "";
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
