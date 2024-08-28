using Bb.ComponentDescriptors;
using Bb.ComponentModel.Translations;
using Bb.Configuration.Git;
using Bb.Editors;
using Bb.Modules;
using Bb.UIComponents;
using Bb.Wizards;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Dynamic;
using System.Reflection;


namespace Bb.Pages
{

    public partial class PageModule : ComponentBase, ITranslateHost
    {


        [Parameter]
        public Guid Uuid { get; set; }

        [Inject]
        public IRefreshService RefreshService { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }

        [Inject]
        public ModuleInstances Instances { get; set; }

        [Inject]
        public FeatureInstances FeatureInstances { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            StateHasChanged();
        }

        public ModuleInstance Module => _module ?? (_module = Instances.GetModule(Uuid));



        private void OpenDialogCreateNewFeature()
        {

            var wizardModel = new WizardModel(DialogService)
                  .SetTitle(ModuleConstants.AddANewFeature)
                  .SetModel(new NewFeatureDescription() { Module = Module })
                  .AddPage(ModuleConstants.Description, c =>
                  {
                      c.SetDescription(ModuleConstants.AddNewFeatureSetName)
                       .SetFilterOnPropertyModel("Name", "Description");
                  })
                  .AddPage(ModuleConstants.Type, c =>
                  {
                      c.SetDescription(ModuleConstants.AddNewFeatureSetType)
                       .SetFilterOnPropertyModel("Type");
                  })
                  .ExecuteOnClose(async (wizard, result) =>
                  {
                      if (result == WizardResult.Ok)
                      {
                          var m = wizard.GetModel<NewFeatureDescription>();
                          var module = FeatureInstances.Create(Uuid, m.Type.Value, m.Name, m.Description);
                      }
                      StateHasChanged();
                      RefreshService.CallRequestRefresh(this, UIKeys.Menus.Modules);
                  })
            ;

            wizardModel.Show();

        }


        private async void OpenEdit()
        {

            var act = new EditorResultComponent()
            {
                Cancel = ctx =>
                {
                    return true;
                },
                Validate = ctx =>
                {

                    var diagnosticResult = ctx.ViewObject.Validate(this.TranslationService);
                    if (diagnosticResult.IsValid)
                    {
                        ctx.Mapper.MapTo(ctx.ViewObject, ctx.SelectedObject.GetType(), ctx.SelectedObject);
                        return true;
                    }

                    return false;

                },
                ToClose = c =>
                {
                    if (!c.Result.Canceled)
                    {
                        Instances.Save(Module);
                        StateHasChanged();
                    }
                }
                
            };

            var b = new DialogParameters
            {
                { "SelectedObject",  Module.Sources },
                { "Actions", act
                }
            };

            var options = new DialogOptions()
            {
                MaxWidth = MaxWidth.Medium,
                FullWidth = true,
                CloseOnEscapeKey = false,
                NoHeader = false,
                Position = DialogPosition.Center,
            };

            try
            {

                await InvokeAsync(() =>
                {                    
                    var r = DialogService.ShowAsync<EditorComponent>(ComponentConstants.Edit.Translate(this), b, options);
                });

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private async Task OpenDialogDeleteFeature(CellContext<FeatureInstance> arg)
        {

            FeatureInstance feature = arg.Item;

            var options = new DialogOptions()
            {
                MaxWidth = MaxWidth.Small,
                FullWidth = false,
                CloseOnEscapeKey = true,
                NoHeader = false,
                Position = DialogPosition.Center,
            };

            var cancel = ModuleConstants.Cancel.Translate(this);
            var title = ModuleConstants.Delete.Translate(this);
            var message = ModuleConstants.DoYouWantToDeleteItem.Translate(this, "feature", feature.Label);

            bool? result = await DialogService.ShowMessageBox(title, message, yesText: title + "!", cancelText: cancel, options: options);
            var state = result.HasValue ? result.Value : false;
            if (state)
            {
                FeatureInstances.Remove(feature);
                StateHasChanged();
                RefreshService.CallRequestRefresh(this, UIKeys.Menus.Modules);
            }

        }


        public MudDataGrid<FeatureInstance> Grid { get; set; }


        private Func<FeatureInstance, bool> _quickFilter => x =>
        {

            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Label.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.FeatureSpecification.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;

        };

        private string _searchString;


        private ModuleInstance _module;

    }

}
