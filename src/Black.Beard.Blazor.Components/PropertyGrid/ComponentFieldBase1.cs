using Bb.ComponentDescriptors;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace Bb.PropertyGrid
{


    public partial class ComponentFieldBase<T> : ComponentFieldBase, IComponentFieldBase<T>
    {

        public ComponentFieldBase()
        {

        }

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        public T? Value
        {
            get
            {

                if (Property != null)
                {
                    var r = Load();
                    if (r != null)
                        return (T)r;
                }

                return default(T);

            }
            set
            {
                
                if (Property != null)
                    if (!object.Equals(Property.Value, value))
                    {
                        Property.Value = Save(value);
                        PropertyChange();
                    }

            }

        }

        public T? GetStep()
        {

            var step = this.Property.Step;
            var result = Convert.ChangeType(step, typeof(T));

            if (object.Equals(result, 0))
                result = 1;

            return (T)result;

        }

        public T? GetMinimum()
        {

            var step = this.Property.Minimum;
            var result = Convert.ChangeType(step, typeof(T));

            if (object.Equals(result, 0))
                result = 1;

            return (T)result;

        }

        public T? GetMaximum()
        {

            var step = this.Property.Maximum;
            var result = Convert.ChangeType(step, typeof(T));

            if (object.Equals(result, 0))
                result = 1;

            return (T)result;

        }

        public string Validate(T o)
        {

            if (this.Property != null)
            {

                string? lastError = Property.ErrorText;
                Property.ErrorText = null;

                var messages = new List<string>();
                if (!Property.Validate(o, out var result))
                    Property.ErrorText = result.Message;

                var newError = !string.IsNullOrEmpty(Property.ErrorText);

                if (!this.Changed || newError != Property.InError || lastError != Property.ErrorText)
                {
                    Property.InError = newError;
                    ValidationHasChanged();
                }

                return Property.ErrorText;

            }

            return null;

        }

        private void ValidationHasChanged()
        {

            if (this.Property != null)
            {

                var p = this.Property;

                if (p.UIPropertyValidationHasChanged != null)
                    p.UIPropertyValidationHasChanged(this);

                if (p.PropertyValidationHasChanged != null)
                    p.PropertyValidationHasChanged(this.Property);

                if (p.Parent != null)
                    this.Property.Parent.ValidationHasChanged(this);

            }

        }

        public virtual object Save(object item)
        {
            return item;
        }

        public virtual object? Load()
        {

            object _value = null;

            if (Property != null)
            {

                try
                {

                    var v = Property.Value;
                    if (v == null)
                        return null;

                    var c = object.Equals(v, _value);

                    if (this.Property.KindView == PropertyKingView.Date.ToString() || this.Property.KindView == PropertyKingView.DateOffset.ToString())
                    {
                        if (Property.IsNullable)
                        {
                            var o = Convert.ChangeType(v, Property.SubType, CultureInfo.CurrentCulture);
                            _value = (T)Activator.CreateInstance(this.Property.Type, new object[] { o });
                        }
                        else
                        {
                            _value = (T)Convert.ChangeType(v, this.Property.Type, CultureInfo.CurrentCulture);
                        }
                    }
                    else
                    {
                        if (Property.IsNullable)
                        {
                            var o = Convert.ChangeType(v, Property.SubType);
                            _value = (T)Activator.CreateInstance(this.Property.Type, new object[] { o });
                        }
                        else
                        {
                            _value = (T)Convert.ChangeType(v, this.Property.Type);
                        }
                    }
                    if (c)
                        PropertyChange();
                }
                catch (Exception ex)
                {

                }

            }

            return _value;

        }

    }

}
