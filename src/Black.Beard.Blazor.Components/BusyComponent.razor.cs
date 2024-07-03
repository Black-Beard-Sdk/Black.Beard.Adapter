using Bb.ComponentModel.Translations;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bb
{

    public partial class BusyComponent : ComponentBase, ITranslateHost, IDisposable
    {

        public BusyComponent()
        {

        }

        protected override void OnAfterRender(bool firstRender)
        {

            base.OnAfterRender(firstRender);

            if (firstRender)
                Session.Run();

        }

        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }


        [Inject]
        public ITranslateService TranslationService { get; set; }


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

        [Parameter]
        public BusySession Session
        {
            get => _session;
            set
            {
                _session = value;
            }
        }


        private async void Closing()
        {

            await InvokeAsync(() =>
            {
                MudDialog.Close(DialogResult.Cancel());
                StateHasChanged();

            });

        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_busyService != null)
                        _busyService.BusyChanged -= _busyService_BusyChanged;

                }

                // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
                // TODO: affecter aux grands champs une valeur null
                disposedValue = true;
            }
        }


        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        private async void _busyService_BusyChanged(object? sender, BusyEventArgs e)
        {

            var busyVisible = e.Source.BusyStatus == BusyEnum.Completed;
            if (busyVisible)
            {
                await InvokeAsync(() =>
                {

                    MudDialog.Close(DialogResult.Ok(true));
                    StateHasChanged();
                });
            }
            else
            {

                await InvokeAsync(() =>
                {

                    MudDialog.Close(DialogResult.Ok(true));
                    StateHasChanged();
                });

            }

        }



        private IBusyService _busyService;
        private bool disposedValue;
        private BusySession _session;

    }

}
