using Bb.Wizards;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Bb.ComponentModel.Attributes;
using Bb.Modules;

namespace Bb.NewModules
{

    public partial class NewModule
    {

        public NewModule()
        {



        }

        private async Task Create()
        {

            var model = new NewModuleDescription();

            var Model = new WizardModel()
            {
                Title = "Add a new module",
            }
            .AddPage(model, "description", c =>
            {
                c.Description = "Specify the name and the description of the new module";
                c.Filter = ["Name", "Description"];
            })
            .AddPage(model, "Type", c =>
            {
                c.Description = "Specify the type of the new module";
                c.Filter = ["Type"];
            })
            ;

            var dialog = await DialogService.ShowDialogAsync<UIWizard>(Model, new DialogParameters()
            {
                Height = "400px",
                Width = "600px",
                Title = $"Updating the {Model.Title} sheet",
                PreventDismissOnOverlayClick = true,
                PreventScroll = true,
            });

            var result = await dialog.Result;
            if (!result.Cancelled && result.Data != null)
            {
                var dialogData = (WizardModel)result.Data;
            }

        }

        [Inject]
        public IDialogService DialogService { get; set; }

    }




}
