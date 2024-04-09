using Bb.NewModules;
using Bb.Wizards;

namespace Bb.Adapter.Pages
{


    public partial class NewModulePage
    {

        public NewModulePage()
        {
                
        }

        override protected void OnInitialized()
        {

            base.OnInitialized();

        }


        protected override void OnAfterRender(bool firstRender)
        {

            base.OnAfterRender(firstRender);

        }


        private NewModule? WizardNewModule { get; set; }


    }

}
