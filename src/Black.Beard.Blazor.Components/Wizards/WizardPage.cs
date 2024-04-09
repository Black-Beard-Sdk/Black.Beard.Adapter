using Bb.ComponentModel.Accessors;
using System.ComponentModel.DataAnnotations;


namespace Bb.Wizards
{
    public class WizardPage
    {

        public WizardPage()
        {
        }

        public string Title { get; set; }


        public object Model { get; internal set; }


        public string Description { get; set; }


        public Bb.Wizards.WizardModel Parent { get; internal set; }


        public bool IsCurrent { get => Parent.CurrentPage == this; }
        public string[] Filter { get; internal set; }

        internal bool IsValid()
        {

            if (Model is IValidatableObject v)
            {
                var context = new ValidationContext(v);
                var results = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(Model, context, results, true);
                return isValid;
            }
            
            return Parent.Wizard.Validate();

        }
    }

}
