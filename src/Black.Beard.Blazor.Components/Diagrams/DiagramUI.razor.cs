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
using Bb.Commands;
using System.Transactions;
using Microsoft.JSInterop;

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
            UIDiagram.Refresh();
            StateHasChanged();
        }
        public void ShowGridChanged(MouseEventArgs args)
        {

            _showGrid = !_showGrid;
            UIDiagram.Refresh();
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
                UIDiagram.Options.GridSize = _gridSizeValue;
                UIDiagram.Refresh();
            }
        }

        private void ZoomTextChanged(string key)
        {
            Resize();
        }
        private void Resize()
        {
            var d = _zoomValue / 100d;
            if (UIDiagram.Zoom != d & d > 0)
                UIDiagram.SetZoom(d);
        }
        private void ZoomChanged()
        {
            var z = UIDiagram.Zoom * 100;
            if (_zoomValue != z)
            {
                _zoomValue = (int)z;
                UIDiagram.Refresh();
            }

        }

        #endregion zoom / GridSize

        [EvaluateValidation(false)]
        public DiagramDiagnostics Diagnostics { get; set; }


        //[EvaluateValidation(false)]
        //[Inject]
        //public IJSRuntime JS { get; set; }

        [EvaluateValidation(false)]
        [Inject]
        public ITranslateService TranslationService { get; set; }

        [EvaluateValidation(false)]
        [Inject]
        public IFocusedService<ToolbarList> GlobalBarFocusService { get; set; }

        [EvaluateValidation(false)]
        [Inject]
        public IFocusedService<ITransactionManager> TransactionManager { get; set; }

        [EvaluateValidation(false)]
        [Inject]
        public IFocusedService<PropertyGridView> PropertyGridFocusedService { get; set; }

        [Parameter]
        public Diagram Diagram
        {
            get => _diagram;
            set
            {

                if (_diagram != value)
                {
                    _diagram = value;
                    if (Diagram?.CommandManager != null)
                    {
                        u = Diagram.CommandManager.UndoList;
                        r = Diagram.CommandManager.RedoList;
                        TransactionManager.FocusChange(_diagram.CommandManager);
                    }
                    u.CollectionChanged += U_CollectionChanged;
                    r.CollectionChanged += U_CollectionChanged;
                }
            }
        }

        #region undo / Redo

        public TransactionViewList? UndoList => u;
        public TransactionViewList? RedoList => r;

        public string IconUndo
        {
            get
            {
                if (u.Any())
                    return Icons.Material.Filled.KeyboardArrowDown;
                return null;
            }
        }
        public string IconRedo
        {
            get
            {
                if (r.Any())
                    return Icons.Material.Filled.KeyboardArrowDown;
                return null;
            }
        }

        public bool DisabledUndo
        {
            get
            {
                if (u == null)
                    return true;
                return !u.Any();
            }
        }

        public bool DisabledRedo
        {
            get
            {
                if (r == null)
                    return true;
                return !r.Any();
            }
        }

        public void OnRestoreUndo(MouseEventArgs args, TransactionView cmd)
        {
            Diagram.CommandManager.Undo(cmd.BeforeIndex);
        }

        public void OnRestoreRedo(MouseEventArgs args, TransactionView cmd)
        {
            Diagram.CommandManager.Redo(cmd.Index);
        }

        #endregion undo / Redo

        private void U_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StateHasChanged();
        }


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
        private BlazorDiagram UIDiagram { get; set; } = null!;

        [Parameter]
        public MudExpansionPanel ExpansionDiagnostic { get; set; }

        protected override void OnInitialized()
        {

        }


        protected override Task OnInitializedAsync()
        {

            UIDiagram = CreateDiagram();
            UIDiagram.PointerClick += PointerClick;
            UIDiagram.SelectionChanged += SelectionChanged;
            if (Diagram != null)
            {

                Diagram.Apply(UIDiagram);
                _timer = new Timer(_ =>
                {
                    InvokeAsync(RenderFirstLinks);
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                }, null, 500, 1500);

            }

            return base.OnInitializedAsync();
        }


        #region fix the bug of the diagram for showing the links when the diagram is loaded

        private Timer _timer;
        private Timer _timer2;

        public void RenderFirstLinks()
        {
            _timer2 = new Timer(_ =>
            {
                InvokeAsync(RenderFirstLinks2);
                _timer2.Change(Timeout.Infinite, Timeout.Infinite);
            }, null, 500, 1500);
        }

        public void RenderFirstLinks2()
        {
            Diagram?.Prepare();
            Diagram.SubscribesUIChanges();
        }

        #endregion fix the bug of the diagram for showing the links when the diagram is loaded


        private void PointerClick(Model? model, Blazor.Diagrams.Core.Events.PointerEventArgs args)
        {
            if (model == null)
                Select(Diagram);
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
                        {
                            using (var transaction = this.Diagram.CommandManager.BeginTransaction(Mode.Recording, "", Behavior.RemoveLastTransaction | Behavior.AutoCommit))
                            {
                                link = this.Diagram.CreateLink(toolLink, source, targetAnchor);
                            }
                        }

                        return link?.UILink;

                    },

                    TargetAnchorFactory = (diagram, link, model) =>
                    {
                        var toolLink = ToolBar?.GetLink(link);
                        if (toolLink != null)
                            return model.ConvertToAnchor(toolLink);
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
            ksb.SetShortcut("s", ctrl: false, shift: false, alt: true, Save);
            
            ksb.SetShortcut("z", ctrl: false, shift: false, alt: true, Cancel);
            ksb.SetShortcut("r", ctrl: false, shift: false, alt: true, Redo);

            //ksb.SetShortcut("c", ctrl: true, shift: false, alt: false, Copy);
            //ksb.SetShortcut("x", ctrl: true, shift: false, alt: false, Cut);
            //ksb.SetShortcut("v", ctrl: true, shift: false, alt: false, Past);

            diagram.ZoomChanged += ZoomChanged;

            return diagram;

        }

        private async ValueTask Cancel(Blazor.Diagrams.Core.Diagram diagram)
        {
            var lastCommand = Diagram.CommandManager.UndoList.LastOrDefault();
            if (lastCommand != null)
                Diagram.CommandManager.Undo(lastCommand.BeforeIndex);
        }

        private async ValueTask Redo(Blazor.Diagrams.Core.Diagram diagram)
        {
            var lastCommand = Diagram.CommandManager.RedoList.LastOrDefault();
            if (lastCommand != null)
                Diagram.CommandManager.Redo(lastCommand.Index);
        }

        //private async ValueTask Copy(Blazor.Diagrams.Core.Diagram diagram)
        //{

        //}

        //private async ValueTask Cut(Blazor.Diagrams.Core.Diagram diagram)
        //{

        //}

        //private async ValueTask Past(Blazor.Diagrams.Core.Diagram diagram)
        //{

        //}

        public void Save()
        {
            Save(UIDiagram);
        }

        private async ValueTask Save(Blazor.Diagrams.Core.Diagram diagram)
        {


            if (_session == null)
            {

                var startTime = DateTime.Now.AddSeconds(1);

                _session = BusyService.IsBusyFor(this, "Save", (a) =>
                {

                    // a.Update("Saving...");

                    var diagnostic = new DiagramDiagnostics() { Translator = TranslationService };

                    diagnostic.EvaluateModel(UIDiagram);

                    Diagnostics = diagnostic;
                    if (Diagnostics.Where(c => c.Level == DiagnosticLevel.Error).Any())
                        ExpansionDiagnostic.ExpandAsync();

                    foreach (INodeModel node in UIDiagram.Nodes)
                        node.SynchronizeSource();

                    Diagram.LastDiagnostics = diagnostic;
                    Diagram?.Save(Diagram);

                }).Add((a) =>
                {

                    while(DateTime.Now < startTime)
                        Task.Yield();

                });
            }

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
                    var point1 = UIDiagram.GetRelativeMousePoint(args.ClientX, args.ClientY);
                    var m1 = Diagram.AddModel(dragedItem, point1.X, point1.Y);
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
                    if (UIDiagram != null)
                    {
                        UIDiagram.PointerClick -= PointerClick;
                        UIDiagram.SelectionChanged -= SelectionChanged;
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

                if (Diagram != null)
                {
                    var t = Diagram.GetToolbar();

                    GlobalBarFocusService.FocusChange(Diagram
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

            var result = base.OnAfterRenderAsync(firstRender);

            return result;

        }

        private IBusyService _busyService;
        private string dropClass = "";
        private PropertyGridView PropertyGrid;
        private bool disposedValue;
        private BusySession _session;

        private Diagram _diagram;
        private TransactionViewList? u;
        private TransactionViewList? r;

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
