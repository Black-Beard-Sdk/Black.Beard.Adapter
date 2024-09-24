using Bb.ComponentModel.Translations;
using Bb.Modules.Sgbd.Models;
using Bb.PropertyGrid;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace Bb.Modules.Sgbd.Components
{
    
    public partial class TableColumnIndex : ComponentBase, IDisposable, ITranslateHost
    {



        public Table Table  => Index.Table;

        [Parameter]
        public Models.Index Index { get; set; }

        [Parameter]
        public TableIndex Parent { get; set; }

        [Parameter]
        public ColumnIndex Column { get; set; }


        [Inject]
        public IFocusedService<PropertyGridView> FocusedService { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }

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
            Index.RemoveColumn(Column);
            _shouldRender = true;
            StateHasChanged();
            Parent.Refresh();
        }

        private bool _shouldRender = true;

    }

}
