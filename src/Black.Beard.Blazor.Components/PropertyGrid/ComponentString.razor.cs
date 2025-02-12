using Bb.ComponentDescriptors;
using MudBlazor;

namespace Bb.PropertyGrid
{

    public partial class ComponentString
    {

        protected override Task OnInitializedAsync()
        {

            switch (Mask)
            {

                case StringType.Email:
                    inputType = InputType.Email;
                    break;
                case StringType.Number:
                    inputType = InputType.Number;
                    break;

                case StringType.Phone:
                    inputType = InputType.Telephone;
                    break;

                case StringType.Url:
                    inputType = InputType.Url;
                    break;

                case StringType.Color:
                    break;

                case StringType.Date:
                    inputType = InputType.Date;
                    break;

                case StringType.DateTimeLocal:
                    inputType = InputType.DateTimeLocal;

                    break;
                case StringType.Month:
                    inputType = InputType.Month;
                    break;

                case StringType.Time:
                    inputType = InputType.Time;
                    break;

                case StringType.Week:
                    inputType = InputType.Week;
                    break;

                case StringType.DateTime:
                    break;

                case StringType.Text:
                case StringType.Password:
                case StringType.PasswordRepeat:
                case StringType.Hidden:
                case StringType.Search:
                case StringType.File:
                case StringType.Image:
                case StringType.Range:
                case StringType.Select:
                case StringType.Textarea:
                case StringType.Undefined:
                default:
                    break;
            }

            return base.OnInitializedAsync();
        }

        //public string CurrentValue
        //{
        //    get => Property.Value?.ToString() ?? string.Empty;
        //    set
        //    {
        //        if (Property.Value != value)
        //        {
        //            Property.Value = value;
        //            PropertyChange();
        //        }
        //    }
        //}


        private InputType inputType = InputType.Text;

    }

}
