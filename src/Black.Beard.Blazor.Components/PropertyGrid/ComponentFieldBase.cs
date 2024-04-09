using Bb.ComponentModel.Translations;
using Bb.CustomComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.ComponentModel;


// https://www.fluentui-blazor.net/
namespace Bb.PropertyGrid
{


    public partial class ComponentFieldBase : ComponentBase
    {



        [Parameter]
        public PropertyObjectDescriptor? Property
        {
            get => _property;
            set
            {
                _property = value;
                PropertyChange();
            }
        }


        public bool IsReadOnly => Property?.ReadOnly ?? false;

        public bool Changed { get; internal set; }

        [Parameter]
        public Color LabelColor { get; set; } = Color.Neutral;


        [Parameter]
        public Typography LabelTypography { get; set; } = Typography.Body;


        [Parameter]
        public HorizontalAlignment LabelHorizontalAlignment { get; set; } = HorizontalAlignment.Left;





        protected virtual void PropertyChange()
        {
            this.Changed = true;
            if (_property != null)
            {

                if (_property.PropertyHasChanged != null)
                    _property.PropertyHasChanged(_property);

                StateHasChanged();

            }

        }

        private PropertyObjectDescriptor? _property;

    }


}
