using Bb.Commands;
using Bb.ComponentDescriptors;
using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

// https://www.fluentui-blazor.net/
namespace Bb.PropertyGrid
{

    public partial class ComponentFieldBase : ComponentBase, IComponentFieldBase
    {


        public ComponentFieldBase()
        {
            OnFocus = new EventCallback<FocusEventArgs>(this, OnFocused);
        }



        protected EventCallback<FocusEventArgs> OnFocus;

        public virtual async void OnFocused(FocusEventArgs args)
        {
            this.ParentGrid?.SetFocus(this);
        }

        public virtual async void OnFocusedRelay(PropertyGridView sender, ComponentFieldBase component)
        {
            if (component != this)
                this.ParentGrid?.SetFocus(component);
        }

        public string StrategyName => Descriptor?.StrategyName;

        [Parameter]
        public Descriptor? Descriptor
        {
            get => _descriptor;
            set
            {
                _descriptor = value;

                if (_descriptor is PropertyObjectDescriptor property) // Convert enum to local InputType
                    if (Enum.TryParse(typeof(InputType), property.Mask.ToString(), out var valueResult))
                        InputType = (InputType)valueResult;

                PropertyChange();

            }
        }

        public PropertyObjectDescriptor? Property
        {
            get
            {
                if (_descriptor is PropertyObjectDescriptor d)
                    return d;
                return default;
            }
        }

        public PropertyGridView ParentGrid => Descriptor.Ui as PropertyGridView;

        public bool WithGroup => ParentGrid?.WithGroup ?? false;

        /// <summary>
        /// Return a new transaction
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public ITransaction GetTransaction(string label)
        {
            return ParentGrid.StartTransaction(label);
        }

        public virtual string? ValueString { get; set; }

        [Parameter]
        public Variant CurrentVariant { get; set; }

        [Parameter]
        public Margin CurrentMargin { get; set; }

        [Inject]
        public ITranslateService TranslateService { get; set; }

        public bool IsReadOnly
        {
            get
            {

                if (this.Property != null)
                    return Property.ReadOnly;

                return false;

            }
        }

        public bool Changed { get; internal set; }


        public InputType InputType { get; set; }


        protected virtual void PropertyChange()
        {
            this.Changed = true;
            if (Property != null)
            {
                if (Property.PropertyHasChanged != null)
                    Property.PropertyHasChanged(Property);

                StateHasChanged();

            }

        }


        public IMask? TagMask => _tagMask ?? (_tagMask = CreateMask());


        protected virtual IMask? CreateMask()
        {
            if (this.Property != null)
            {

                if (Property?.CreateMask != null)
                    return Property.CreateMask();

                if (!string.IsNullOrEmpty(Property?.PatternString))
                    return new RegexMask(Property.PatternString);

            }
            return null;

        }

        protected bool LastWritingInError = false;
        protected string LastTextInError = string.Empty;

        private IMask? _tagMask;
        private Descriptor? _descriptor;

    }


}
