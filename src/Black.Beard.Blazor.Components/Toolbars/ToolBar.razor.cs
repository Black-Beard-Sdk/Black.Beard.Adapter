using Bb.ComponentModel.Translations;
using Bb.Diagrams;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Bb.Toolbars
{

    public partial class ToolBar : ComponentBase, ITranslateHost, IDisposable
    {

        public ToolBar()
        {

        }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public bool Visible { get; set; }

        public ToolbarList Bars
        {
            get
            {
                if (_bars == null)
                    Bars = new ToolbarList(Guid.NewGuid(), this.Name)
                    {
                        Component = this,
                    };
                return _bars;
            }
            set
            {
                if (_bars != null)
                    _bars.CollectionChanged -= Bars_CollectionChanged;

                if (value != null)
                {
                    _bars = value;
                    _bars.Component = this;
                    _bars.CollectionChanged += Bars_CollectionChanged;
                }
            }
        }


        [Inject]
        public ITranslateService TranslationService { get; set; }

        [Inject]
        public IFocusedService<ToolbarList> GlobalBarFocusService { get; set; }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {

            if (firstRender)
            {
                GlobalBarFocusService.FocusChanged += FocusBar_FocusChanged;
            }

            return base.OnAfterRenderAsync(firstRender);
        }

        private void FocusBar_FocusChanged(object? sender, EvaluatorEventArgs<ToolbarList> e)
        {

            if (e.Evaluate == null || e.Evaluate(this.Bars, sender))
            { }

            Visible = !this.Bars.IsEmpty;

        }

        private void Bars_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Visible = !this.Bars.IsEmpty;
            StateHasChanged();
        }

        public Tool CurrentDragStarted { get; private set; }

        public Tool CurrentClicked { get; set; }

        public async Task DragStart(DragEventArgs args, Tool item)
        {
            this.CurrentDragStarted = item;
        }

        public void Dispose()
        {

            GlobalBarFocusService.FocusChanged -= FocusBar_FocusChanged;

        }

        private ToolbarList _bars;

    }

}
