using Bb.ComponentDescriptors;
using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components;
using MudBlazor;

// https://www.fluentui-blazor.net/
namespace Bb.PropertyGrid
{

    public partial class ComponentFieldBase : ComponentBase, IComponentFieldBase
    {

        [Parameter]
        public string StrategyName { get; set; }


        [Parameter]
        public PropertyObjectDescriptor? Property
        {
            get => _property;
            set
            {
                _property = value;
                if (Enum.TryParse(typeof(InputType), Property.Mask.ToString(), out var valueResult))
                    InputType = (InputType)valueResult;
                PropertyChange();
            }
        }

        public virtual string? ValueString { get; set; }


        protected string Translate(string key)
        {
            return TranslateService.Translate(key);
        }

        [Parameter]
        public Variant CurrentVariant { get; set; }

        [Parameter]
        public Margin CurrentMargin { get; set; }

        [Inject]
        public ITranslateService TranslateService { get; set; }

        public bool IsReadOnly => Property?.ReadOnly ?? false;

        public bool Changed { get; internal set; }


        public InputType InputType { get; set; }

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


        public IMask? TagMask => _tagMask ?? (_tagMask = CreateMask());


        protected virtual IMask? CreateMask()
        {

            if (Property?.CreateMask != null)
                return Property.CreateMask();

            if (!string.IsNullOrEmpty(Property?.PatternString))
                return new RegexMask(Property.PatternString);

            return null;

        }

        protected bool LastWritingInError = false;
        protected string LastTextInError = string.Empty;

        private IMask? _tagMask;
        private PropertyObjectDescriptor? _property;

    }


}
