using Bb.ComponentModel.Translations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bb.CustomComponents
{
    public static class PropertyDescriptorExtension
    {

        public static DiagnosticValidatorItem ValidateValue(this PropertyDescriptor descriptor, object value, ITranslateService translator = null)
        {

            var messages = new DiagnosticValidatorItem(descriptor);

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


        public static DiagnosticValidatorItem ValidateInstance(this PropertyDescriptor descriptor, object instance, ITranslateService translator = null)
        {

            var messages = new DiagnosticValidatorItem(descriptor);

            var attributes = descriptor
                .Attributes
                .OfType<Attribute>()
                .ToList();

            object value = descriptor.GetValue(instance);

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

        public static DiagnosticValidator Validate(this object self, ITranslateService translator = null)
        {

            DiagnosticValidator validator = new DiagnosticValidator();
            var properties = TypeDescriptor.GetProperties(self.GetType());
            foreach (PropertyDescriptor property in properties)
            {
                var result = property.ValidateInstance(self, translator);
                if (!result.IsValid)
                    validator.Add(result);
            }

            return validator;

        }



    }


}
