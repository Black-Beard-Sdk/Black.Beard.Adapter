using Bb.CustomComponents;
using Bb.PropertyGrid;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static MudBlazor.CategoryTypes;


namespace Bb.Wizards
{

    // https://www.fluentui-blazor.net/Icon
    public partial class UIWizard
    {

        /// <summary>
        /// Constructs an instance of a wizard
        /// </summary>
        public UIWizard()
        {

        }

        private WizardModel _content;

        [Parameter]
        public WizardModel Content
        {
            get => _content;
            set
            {
                _content = value;
                if (value != null)
                    _content.Wizard = this;                
                this.StateHasChanged();
            }
        }

        private bool visible = true;

        public DialogOptions dialogOptions { get; set; }

        public PropertyGridView CurrentPropertyGridView { get; set; }

        public object CurrentModel => Content.CurrentPage.Model;

        public IEnumerable<WizardPage> Pages => Content.Pages;


        public bool PropertyFilter(PropertyObjectDescriptor property)
        {
            if (Content.CurrentPage != null)
            {
                var filter = Content.CurrentPage.Filter;
                if (filter != null && filter.Length > 0)
                    return filter.Contains(property.Name);
            }
            return true;
        }

        /// <summary>
        /// Show a button for stopping the wizard when unfinished
        /// </summary>
        [Parameter]
        public bool AllowCancel { get; set; }

        /// <summary>
        /// Show a button for navigating to the previous step
        /// </summary>
        [Parameter] public bool AllowPrevious { get; set; }


        [Inject]
        public IDialogService DialogService { get; set; }

        [CascadingParameter] MudDialogInstance MudDialog { get; set; }


        public void PropertyHasChanged(PropertyObjectDescriptor obj)
        {
            this.StateHasChanged();
        }

        internal async Task Cancel()
        {
            _content.Wizard = null; ;
            MudDialog.Cancel();
        }

        internal async Task GoToPreviousStep()
        {
            Content.Previous();
            if (CurrentPropertyGridView != null)
                CurrentPropertyGridView.SelectedObject = CurrentModel;
            this.StateHasChanged();
        }

        internal async Task GoToNextStep()
        {
            Content.Next();
            if (CurrentPropertyGridView != null)
                CurrentPropertyGridView.SelectedObject = CurrentModel;
            this.StateHasChanged();
        }

        internal async Task Apply()
        {
            _content.Wizard = null; ;
            MudDialog.Close(DialogResult.Ok(true));
            this.StateHasChanged();
        }

        internal bool Validate()
        {
            if (this.CurrentPropertyGridView != null)
            {
                var result = this.CurrentPropertyGridView.Validate();
                return result.IsValid;
            }

            return false;
        }

        public bool DisableCanCancel => false;
        public bool DisableCanPrevious { get => Content.DisableCanPrevious(); }
        public bool DisableCanNext { get => Content.DisableCanNext(); }
        public bool DisableCanValidate { get => Content.DisableCanValidate(); }

    }


}
