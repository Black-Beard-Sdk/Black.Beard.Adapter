using MudBlazor;

namespace Bb.PropertyGrid
{

    public partial class ComponentPassword
    {

        protected override Task OnInitializedAsync()
        {

            return base.OnInitializedAsync();

        }


        void ButtonClick()
        {
            if (isShow)
            {
                isShow = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                isShow = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }

        private InputType PasswordInput = InputType.Password;
        private bool isShow;
        private string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    }


}
