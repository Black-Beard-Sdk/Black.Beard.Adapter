using Bb.ComponentModel.Translations;
using Bb.Modules.Sgbd.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel;
using static ICSharpCode.Decompiler.IL.Transforms.Stepper;


namespace Bb.Modules.Sgbd.Components
{
    public partial class TableColumn : IDisposable, ITranslateHost
    {


        [Parameter]
        public Table Table { get; set; }

        [Parameter]
        public TableNode Parent { get; set; }

        [Parameter]
        public Column Column { get; set; }



        [Inject]
        public IFocusedService FocusedService { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }



        public bool HasLinks => Table.GetPort(Column)?.Links.Count > 0;

        public void Dispose()
        {
            Column.PropertyChanged -= ReRender;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Column.PropertyChanged += ReRender;
        }

        protected override bool ShouldRender() => _shouldRender;

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
            FocusedService.FocusChange(this.Column);
        }


        public void OnEditName(MouseEventArgs eventArgs)
        {

            _editName = true;
            _shouldRender = true;
            StateHasChanged();

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

        public void DelColumn()
        {
            Table.RemoveColumn(Column);
            _editName = false;
            _shouldRender = true;
            StateHasChanged();
            Parent.Refresh();
        }

        private bool _shouldRender = true;
        private bool _editName;

    }

}
