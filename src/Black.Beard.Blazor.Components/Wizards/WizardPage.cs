


using Bb.ComponentModel.Translations;

namespace Bb.Wizards
{
    public class WizardPage
    {

        public WizardPage()
        {
        }


        public object Model { get; internal set; }

        public TranslatedKeyLabel Title { get; set; }

        public TranslatedKeyLabel Description { get; set; }

        public WizardModel Parent { get; internal set; }

        public bool IsCurrent { get => Parent.CurrentPage == this; }

        public string[] FilterOnPropertyModel { get; internal set; }


        public WizardPage SetDescription(TranslatedKeyLabel description)
        {
            this.Description = description;
            return this;
        }

        public WizardPage SetFilterOnPropertyModel(params string[] filterOnPropertyModel)
        {
            this.FilterOnPropertyModel = filterOnPropertyModel;
            return this;
        }

        internal bool IsValid()
        {

            //if (Model is IValidatableObject v)
            //{
            //    var context = new ValidationContext(v);
            //    var results = new List<ValidationResult>();
            //    var isValid = Validator.TryValidateObject(Model, context, results, true);
            //    return isValid;
            //}

            return Parent.Wizard?.Validate() ?? false;

        }
    
    }

}
