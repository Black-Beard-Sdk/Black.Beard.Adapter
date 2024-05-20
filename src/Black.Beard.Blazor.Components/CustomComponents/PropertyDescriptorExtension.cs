using Bb.ComponentModel.Translations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static MudBlazor.CategoryTypes;

namespace Bb.CustomComponents
{


    public static class PropertyDescriptorExtension
    {

        public static DiagnosticValidator Validate(this object self, ITranslateService translator = null)
        {

            DiagnosticValidator validator = new DiagnosticValidator();
            var properties = TypeDescriptor.GetProperties(self);
            foreach (PropertyDescriptor property in properties)
                if (property.ToEvaluate())
                {
                    var result = property.ValidateInstance(self, translator);
                    if (!result.IsValid)
                        validator.Add(result);
                }

            return validator;

        }


        public static DiagnosticValidatorItem ValidateInstance(this PropertyDescriptor descriptor, object instance, ITranslateService translator = null)
        {
            try
            {
                object value = descriptor.GetValue(instance);
                return ValidateValue(descriptor, value, translator);
            }

            catch (Exception ex)
            {
                var messages = new DiagnosticValidatorItem(descriptor);
                messages.Add("Failed to resolve the value." + ex.Message);
                return messages;
            }

        }


        public static DiagnosticValidatorItem ValidateValue(this PropertyDescriptor descriptor, object value, ITranslateService translator = null)
        {

            var messages = new DiagnosticValidatorItem(descriptor)
            {
                Value = value
            };

            var attributes = descriptor
                .Attributes
                .OfType<Attribute>()
                .ToList();

            foreach (Attribute attribute in attributes)
                if (attribute is ValidationAttribute validation)
                    if (!validation.IsValid(value))
                    {

                        var label = descriptor.DisplayName;

                        if (translator != null)
                        {
                            var ll = label.Replace(descriptor.Name, "{0}");
                            label = translator.Translate(ll);
                        }

                        var message = validation.FormatErrorMessage(label);

                        if (translator != null && validation.FormatErrorMessage(string.Empty).IsValidTranslationKey())
                            messages.Add(translator.Translate(message));
                        else
                            messages.Add(message);

                    }

            return messages;

        }


        public static bool ToEvaluate(this PropertyDescriptor self)
        {

            var value = self.Attributes
                .Cast<Attribute>()
                .OfType<EvaluateAttribute>()
                .FirstOrDefault();

            if (value != null)
                return value.ToEvaluate;

            return true;

        }



    }


    public class EvaluateAttribute : Attribute
    {

        public EvaluateAttribute(bool toEvaluate)
        {
            this.ToEvaluate = toEvaluate;
        }

        public bool ToEvaluate { get; }
    }


}
