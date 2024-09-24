using Bb.ComponentModel.Translations;
using Bb.Modules.Sgbd.Models;
using Bb.PropertyGrid;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel;


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
        public IFocusedService<PropertyGridView> FocusedService { get; set; }

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
       
        public void DelColumn()
        {
            Table.RemoveColumn(Column);
            _shouldRender = true;
            StateHasChanged();
            Parent.Refresh();
        }

        private bool _shouldRender = true;

    }

}
