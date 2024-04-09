using Bb.CustomComponents;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bb.PropertyGrid
{

    public partial class ComponentString
    {

        protected override Task OnInitializedAsync()
        {

            switch (this.Property.Mask)
            {
                    
                case StringType.Email:
                    inputType = InputType.Email;
                    break;
                case StringType.Number:
                    inputType = InputType.Number;
                    break;

                case StringType.Telephone:
                    inputType = InputType.Telephone;
                    break;

                case StringType.Url:
                    inputType = InputType.Url;
                    break;

                case StringType.Color:
                    inputType = InputType.Color;
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
                
                case StringType.Undefined:
                default:
                    break;

            }

            return base.OnInitializedAsync();
        }

        public string CurrentValue
        {
            get => Property.Value?.ToString() ?? string.Empty;
            set
            {
                if (Property.Value != value)
                {
                    Property.Value = value;
                    PropertyChange();
                }
            }
        }


        private InputType inputType = InputType.Text;

    }

}
