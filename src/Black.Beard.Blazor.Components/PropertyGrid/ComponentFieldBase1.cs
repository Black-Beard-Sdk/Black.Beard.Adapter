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

                if (Descriptor != null)
                {
                    var r = Load();
                    if (r != null)
                        return (T)r;
                }

                return default(T);

            }
            set
            {

                if (this.Property != null)
                    if (!object.Equals(Descriptor.Value, value))
                    {

                        Transaction? transaction = null;
                        if (TransactionManager != null)
                            transaction = TransactionManager
                                .BeginTransaction(Mode.Recording, $"update {Property.Name}");

                        try
                        {
                            Descriptor.Value = Save(value);
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

            if (this.Property != null)
            {
                var step = Property.Step;
                var result = Convert.ChangeType(step, typeof(T));

                if (object.Equals(result, 0))
                    result = 1;

                return (T)result;

            }

            return default(T);

        }

        public T? GetMinimum()
        {

            if (this.Property != null)
            {
                var step = Property.Minimum;
                var result = Convert.ChangeType(step, typeof(T));

                if (object.Equals(result, 0))
                    result = 1;

                return (T)result;
            }

            return default(T);

        }

        public T? GetMaximum()
        {

            if (this.Property != null)
            {
                var step = Property.Maximum;
                var result = Convert.ChangeType(step, typeof(T));

                if (object.Equals(result, 0))
                    result = 1;

                return (T)result;
            }

            return default(T);

        }

        public string Validate(T o)
        {

            if (this.Property != null)
            {

                string? lastError = Descriptor.ErrorText;
                Descriptor.ErrorText = null;

                var messages = new List<string>();
                if (!Descriptor.Validate(o, out var result))
                    Descriptor.ErrorText = result.Message;

                var newError = !string.IsNullOrEmpty(Descriptor.ErrorText);

                if (!this.Changed || newError != Descriptor.InError || lastError != Descriptor.ErrorText)
                {
                    Descriptor.InError = newError;
                    ValidationHasChanged();
                }

                return Descriptor.ErrorText;

            }

            return null;

        }

        private void ValidationHasChanged()
        {

            if (this.Descriptor != null)
            {

                var p = Descriptor;

                if (Property.UIPropertyValidationHasChanged != null)
                    Property.UIPropertyValidationHasChanged(this);

                if (p.ValidationHasChanged != null)
                    p.ValidationHasChanged(Descriptor);

                if (Property.Parent != null)
                    Descriptor.Parent.ValidationChanged(this);

            }

        }

        public virtual object Save(object item)
        {
            return item;
        }

        public virtual object? Load()
        {

            object _value = null;

            if (Descriptor != null)
            {

                try
                {

                    var v = Descriptor.Value;
                    if (v == null)
                        return null;

                    _value = v.GetType() != Descriptor.Type
                        ? v.ToObject(Descriptor.Type)
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
