﻿using Bb;
using Bb.Commands;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Translations;
using Bb.Diagrams;
using Bb.PropertyGrid;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Transactions;

namespace Site.Shared
{

    public partial class MainLayout : IDisposable, ITranslateHost
    {

        public MainLayout()
        {

        }


        [EvaluateValidation(false)]
        [Inject]
        public ITranslateService TranslationService { get; set; }


        [Inject]
        public IRefreshService RefreshService { get; set; }

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
        [Inject]
        public IFocusedService<PropertyGridView> PropertyGridFocusedService { get; set; }

        [EvaluateValidation(false)]
        [Inject]
        public IFocusedService<ITransactionManager> TransactionManager { get; set; }


        [Inject]
        public IDialogService DialogService { get; set; }

        protected override Task OnInitializedAsync()
        {
            var result = base.OnInitializedAsync();
            PropertyGridFocusedService.FocusChanged += FocusedService_FocusChanged;
            RefreshService.RefreshRequested += RefreshService_RefreshRequested;
            TransactionManager.FocusChanged += _transactionManager_FocusChanged;

            return result;
        }

        private void RefreshService_RefreshRequested(object sender, RefreshEventArgs arg)
        {
            if (arg.MustRefresh(nameof(MainLayout)))
                StateHasChanged();
            else if (arg.MustRefresh(nameof(PropertyGridView)))
                PropertyGrid?.Refresh();
        }

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

                        await InvokeAsync(() =>
                        {
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

        private void FocusedService_FocusChanged(object? sender, EvaluatorEventArgs<PropertyGridView> e)
        {
            if (this.PropertyGrid != null)
                if (e.Evaluate == null || e.Evaluate(this.PropertyGrid, sender))
                {
                    this.PropertyGrid.SelectedObject = sender;

                    this.PropertyGrid.TransactionFactory = (object o) =>
                    {
                        string text = "Transaction";
                        if (o is string t)
                            text = t;
                        if (TransactionManager != null)
                            return _transactionManager.BeginTransaction(Mode.Recording, text);
                        return null;
                    };

                    // this.PropertyGrid.AddDynamicProperty("TransactionManager", () => _transactionManager);
                }
        }

        private void _transactionManager_FocusChanged(object? sender, EvaluatorEventArgs<ITransactionManager> e)
        {
            _transactionManager = (ITransactionManager)sender;
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
                    if (RefreshService != null)
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


        void DrawerToggle()
        {
            _drawer1pen = !_drawer1pen;
        }

        public bool _drawer1pen = true;


        protected override void OnInitialized()
        {
            StateHasChanged();
        }

        private List<BreadcrumbItem> _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Personal", href: "#"),
            new BreadcrumbItem("Dashboard", href: "#"),
        };

        private PropertyGridView PropertyGrid;
        private MudBlazorAdminDashboard _theme = new();
        private ITransactionManager? _transactionManager;
    }

}
