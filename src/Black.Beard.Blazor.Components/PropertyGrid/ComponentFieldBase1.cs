using Bb.Commands;
using Bb.ComponentDescriptors;
using Bb.Expressions;

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


        public override string? ValueString
        {
            get
            {

                if (LastWritingInError)
                    return LastTextInError;

                var v = this.Value;

                if (ConvertToString(v, out var result))
                    return result;

                return null;

            }
            set
            {

                if (value == null)
                {
                    this.Value = default(T);
                }
                else
                {
                    if (ConvertFromString(value, out var result))
                    {
                        LastTextInError = string.Empty;
                        LastWritingInError = false;

                        if (!object.Equals(this.Value, result))
                            this.Value = result;
                    }
                    else
                    {
                        LastTextInError = value;
                        LastWritingInError = true;
                    }
                }
            }
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

                        Transaction? transaction = null;
                        if (TransactionManager != null)
                            transaction = TransactionManager
                                .BeginTransaction(Mode.Recording, $"update {Property.Name}");

                        try
                        {
                            Property.Value = Save(value);
                            PropertyChange();
                            transaction?.Commit();
                        }
                        finally
                        {
                            transaction?.Dispose();
                        }
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

                    _value = v.GetType() != this.Property.Type
                        ? v.ToObject(this.Property.Type)
                        : _value = v;

                    var c = object.Equals(v, _value);
                    if (!c)
                        PropertyChange();

                }
                catch (Exception ex)
                {

                }

            }

            return _value;

        }


        protected virtual bool ConvertToString(T? value, out string? valueResult)
        {

            valueResult = null;

            if (value == null)
            {
                valueResult = null;
                return true;
            }

            if (value is string vv)
            {
                valueResult = vv;
                return true;
            }

            try
            {
                valueResult = value.ToObject<string>();
            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }

        protected virtual bool ConvertFromString(string value, out T valueResult)
        {

            valueResult = default(T);

            if (string.IsNullOrEmpty(value))
                return true;

            if (value is T vv)
            {
                valueResult = vv;
                return true;
            }

            try
            {
                valueResult = value.ToObject<T>();
            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }

    }

}
