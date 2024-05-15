using Bb.ComponentModel.Translations;
using Bb.Modules.Sgbd.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel;
using static ICSharpCode.Decompiler.IL.Transforms.Stepper;

namespace Bb.Modules.Sgbd.Components
{
    public partial class TableIndex : ComponentBase, IDisposable, ITranslateHost
    {


        [Parameter]
        public Table Table { get; set; }

        [Parameter]
        public TableNode Parent { get; set; }

        [Parameter]
        public Bb.Modules.Sgbd.Models.Index Index { get; set; }



        [Inject]
        public IFocusedService FocusedService { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }


        public void Dispose()
        {
            if (Index != null)
                Index.PropertyChanged -= ReRender;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Index.PropertyChanged += ReRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _shouldRender = false;
            await base.OnAfterRenderAsync(firstRender);
        }

        private void ReRender(object? sender, PropertyChangedEventArgs e)
        {
            _shouldRender = true;
            StateHasChanged();
        }

        public void OnClick(MouseEventArgs eventArgs)
        {
            FocusedService.FocusChange(this.Index);
        }

        public void OnKeyDown(KeyboardEventArgs e)
        {
            if (e.Code == "Enter")
            {
                _editName = false;
                _shouldRender = true;
                StateHasChanged();
            }
        }

        public void LostFocus()
        {

            _editName = false;
            _shouldRender = true;
            StateHasChanged();

        }

        public void DelIndex()
        {
            Table.RemoveIndex(Index);
            _editName = false;
            _shouldRender = true;
            StateHasChanged();
            Parent.Refresh();
        }

        public void AutoAddColumn(MouseEventArgs eventArgs)
        {
            AddColumn();
        }

        public ColumnIndex AddColumn()
        {

            string title = TranslationService.Translate(DatasComponentConstants.Column) + " ";
            int count = 1;
            while (Index.Columns.Any(c => c.Name == (title + count.ToString())))
                count++;

            var column = new ColumnIndex()
            {
                Name = title + count.ToString(),
                Id = Guid.NewGuid()
            };

            var diagram = this.Index.Table.Source.Diagram as SgbdDiagram;
            var t = diagram.GetTechnology();
            if (t != null)
            {

            }

            Index.AddColumn(column);

            Refresh();
            
            return column;

        }

        public void Refresh()
        {
            this.Parent.Refresh();
            _shouldRender = true;
            StateHasChanged();
        }

        protected override bool ShouldRender() => _shouldRender;

        private bool _shouldRender = true;
        private bool _editName;

    }
}
