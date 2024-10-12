using Bb.ComponentModel.Translations;
using Bb.UserInterfaces;
using Microsoft.AspNetCore.Components;

namespace Bb.Menus
{

    public partial class MenuNav : ITranslateHost, IDisposable
    {

        public MenuNav()
        {

        }

        [Parameter]
        public ServerMenu Menus { get; set; }

        [Inject]
        public IRefreshService RefreshService { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            RefreshService.RefreshRequested += RefreshService_RefreshRequested;
        }

        private void RefreshService_RefreshRequested(object sender, RefreshEventArgs arg)
        {
            if (arg.MustRefresh(Menus.Uuid))
                StateHasChanged();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    RefreshService.RefreshRequested -= RefreshService_RefreshRequested;

                }

                disposedValue = true;
            }
        }      

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool disposedValue;

    }

}
