using Bb.ComponentModel.Translations;
using Bb.CustomComponents;
using Bb.PropertyGrid;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace Bb.Wizards
{

    // https://www.fluentui-blazor.net/Icon
    public partial class UIWizard : ITranslateHost
    {

        /// <summary>
        /// Constructs an instance of a wizard
        /// </summary>
        public UIWizard()
        {

        }




        [Parameter]
        public WizardModel Content
        {
            get => _content;
            set
            {
                _content = value;
                if (value != null)
                {
                    _content.Wizard = this;
                }
                this.StateHasChanged();
            }
        }

        [Inject]
        public ITranslateService TranslationService { get; set; }
        
        [Inject]
        public IDialogService DialogService { get; set; }

        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }


        /// <summary>
        /// Show a button for navigating to the previous step
        /// </summary>
        [Parameter] 
        public bool AllowPrevious { get; set; }

        public DialogOptions dialogOptions { get; set; }

        public PropertyGridView CurrentPropertyGridView { get; set; }

        public object CurrentModel => Content.CurrentPage.Model;

        public WizardPage CurrentPage => Content.CurrentPage;

        public IEnumerable<WizardPage> Pages => Content.Pages;


        public bool PropertyFilter(PropertyObjectDescriptor property)
        {
            if (Content.CurrentPage != null)
            {
                var filter = Content.CurrentPage.FilterOnPropertyModel;
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

        public void PropertyHasChanged(PropertyObjectDescriptor obj)
        {
            this.StateHasChanged();
        }

        internal async Task Cancel()
        {
            _content.Wizard = null;
            MudDialog?.Cancel();
            _content.Exit(WizardResult.Cancel);
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

            _carousel.MoveTo(Content.Index);

            this.StateHasChanged();
        }

        internal async Task Apply()
        {
            _content.Wizard = null; ;
            MudDialog?.Close(DialogResult.Ok(true));
            this.StateHasChanged();

            _content.Exit(WizardResult.Ok);

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


        private bool visible = true;
        private WizardModel _content;
        private MudCarousel<WizardPage> _carousel;

    }



}
