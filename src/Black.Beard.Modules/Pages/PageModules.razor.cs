using Bb.ComponentModel.Translations;
using Bb.Modules;
using Bb.UIComponents;
using Bb.Wizards;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;


namespace Bb.Pages
{

    public partial class PageModules : ComponentBase, ITranslateHost
    {


        public PageModules()
        {

        }

        [Inject]
        public IRefreshService RefreshService { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }

        [Inject]
        public ModuleInstances Instances { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        public MudDataGrid<ModuleInstance> Grid { get; set; }

        private void OpenDialogCreateNewModule()
        {

            var wizardModel = new WizardModel(DialogService)
                  .SetTitle(ModuleConstants.AddANewModule)
                  .SetModel(new NewModuleDescription())
                  .AddPage(ModuleConstants.Description, c =>
                  {
                      c.SetDescription(ModuleConstants.AddNewModuleSetName)
                       .SetFilterOnPropertyModel("Name", "Description");
                  })
                  .AddPage(ModuleConstants.Type, c =>
                  {
                      c.SetDescription(ModuleConstants.AddNewModuleSetType)
                       .SetFilterOnPropertyModel("Type");
                  })
                  .ExecuteOnClose(async (wizard, result) =>
                  {
                      if (result == WizardResult.Ok)
                      {
                          var m = wizard.GetModel<NewModuleDescription>();
                          var module = Instances.Create(m.Type.Value, m.Name, m.Description);
                      }
                      StateHasChanged();
                      RefreshService.CallRequestRefresh(this, UIKeys.Menus.Modules);
                  })
            ;

            wizardModel.Show();

        }

        private async Task OpenDialogDeleteModule(CellContext<ModuleInstance> arg)
        {

            ModuleInstance module = arg.Item;

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
            var message = ModuleConstants.DoYouWantToDeleteItem.Translate(this, "module", module.Label);

            bool? result = await DialogService.ShowMessageBox(title, message, yesText: title + "!", cancelText: cancel, options: options);
            var state = result.HasValue ? result.Value : false;
            if (state)
            {
                Instances.Remove(module);
                StateHasChanged();
                RefreshService.CallRequestRefresh(this, UIKeys.Menus.Modules);
            }

        }


        private Func<ModuleInstance, bool> _quickFilter => x =>
        {
            
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Label.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.ModuleSpecification.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;

        };

        private string _searchString;

    }

}
