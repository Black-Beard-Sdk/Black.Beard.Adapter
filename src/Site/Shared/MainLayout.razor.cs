using Bb;
using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using Bb.UserInterfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Site.Shared
{

    public partial class MainLayout : IDisposable
    {


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

        [Inject]
        public IDialogService DialogService { get; set; }



        private async void _busyService_BusyChanged(object? sender, BusyEventArgs e)
        {
            switch (e.Source.BusyStatus)
            {
                case BusyEnum.New:
                    var b = new DialogParameters
                    {
                        { "Session", e.Source }
                    };

                    var options = new DialogOptions()
                    {
                        MaxWidth = MaxWidth.Small,
                        FullWidth = true,
                        CloseOnEscapeKey = false,
                        NoHeader = true,
                        Position = DialogPosition.Center,
                    };

                    try
                    {

                        await InvokeAsync(() => {
                            var r = DialogService.ShowAsync<BusyComponent>("Simple Dialog", b, options);
                        });

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }


                    break;

                case BusyEnum.Started:
                    break;
                case BusyEnum.Running:
                    break;
                case BusyEnum.Completed:
                    break;
                default:
                    break;
            }                               

        }


        private bool _busyVisible = false;

        private IBusyService _busyService;
        private bool disposedValue;

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

        // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
        // ~MainLayout()
        // {
        //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }



        private MudBlazorAdminDashboard _theme = new();

        public bool _drawerOpen = true;

        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        protected override void OnInitialized()
        {
            StateHasChanged();
        }

        private List<BreadcrumbItem> _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Personal", href: "#"),
            new BreadcrumbItem("Dashboard", href: "#"),
        };

    }

}
