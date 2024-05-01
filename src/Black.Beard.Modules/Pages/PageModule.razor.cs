using Bb.ComponentModel.Translations;
using Bb.Modules;
using Bb.Modules.Storage;
using Bb.Wizards;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bb.Pages
{

    public partial class PageModule : ComponentBase, ITranslateHost
    {


        public PageModule()
        {

        }

        [Inject]
        public ITranslateService TranslationService { get; set; }

        public UIWizard Wizard { get; set; }

        [Parameter]
        public WizardModel Model { get; set; }

        [Inject]
        public ModuleInstances Instances { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        private void OpenDialog()
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
                  })
            ;

            wizardModel.Show();

        }


    }

}
