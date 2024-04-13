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
                Name = Enum1.Value2,
            };

            var Model = new WizardModel()
            {
                Title = "Add a new module",
            }
            .AddPage(model, "description", c =>
            {
                c.Description = "Specify the name and the description for the new module";
                c.Filter = ["Name", "Description"];
            })
            .AddPage(model, "Type", c =>
            {
                c.Description = "Specify the type of the new module";
                c.Filter = ["Type"];
            })
            ;

            var b = new DialogParameters
            {
                { "Content", Model }
            };

            var options = new DialogOptions() 
            {
                MaxWidth = MaxWidth.Medium,
                FullWidth = true,
                CloseOnEscapeKey = true,
                NoHeader = false,
                Position = DialogPosition.Center,
            };

            var reference  = DialogService.Show<UIWizard>("Options Dialog", b, options);

        }

        [Inject]
        public IDialogService DialogService { get; set; }

    }




}
