using Bb.Modules;
using Bb.Wizards;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bb.NewModules
{

    public partial class NewModule
    {

        public NewModule()
        {



        }

        private async Task Create()
        {

            var model = new NewModuleDescription()
            {
                
            };

            Wizard
                .SetTitle("Add a new module")
                .SetModel(model)
                .AddPage("description", c =>
                {
                    c.SetDescription("Specify the name and the description for the new module")
                     .SetFilterOnPropertyModel("Name", "Description");
                })
                .AddPage("Type", c =>
                {
                    c.SetDescription("Specify the type of the new module")
                     .SetFilterOnPropertyModel("Type");
                })
            ;

            Wizard.Show();

        }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public WizardModel Wizard { get; set; }

    }




}
