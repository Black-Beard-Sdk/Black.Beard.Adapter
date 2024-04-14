using Bb.Modules;
using Bb.UIComponents;
using Bb.Wizards;


namespace Bb.Loaders
{

    public static class Actions
    {


        public static async Task ExecuteNewModule(EventContext context, WizardModel wizard, ModuleInstances instances)
        {


            var model = new NewModuleDescription()
            {

            };

            wizard.SetTitle("Add a new module")
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
                  .ExecuteOnClose(async (wizard, result) =>
                  {

                      if (result == WizardResult.Ok)
                      {

                          var model = wizard.GetModel<NewModuleDescription>();

                          var module = instances.Create(model.Type.Value, model.Name, model.Description);

                          module.Save();

                      }

                  })
            ;

            wizard.Show();

        }



    }


}