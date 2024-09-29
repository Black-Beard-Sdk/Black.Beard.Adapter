using Bb.ComponentDescriptors;
using Bb.ComponentModel.Translations;
using Bb.TypeDescriptors;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Bb.Diagrams
{

    public class DiagramDiagnostics : List<DiagramDiagnostic>
    {

        public DiagramDiagnostics()
        {

        }

        public void EvaluateModel(object model)
        {

            if (_currentCtx != null)
                throw new InvalidOperationException("Context already initialized");

            _currentCtx = new Context();
            EvaluateModel(model, _currentCtx);

        }

        private void EvaluateModel(object model, Context ctx)
        {

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (ctx.Contains(model))
                return;

            using (ctx.Add(model))
            {

                if (model is IValidationService v)
                    v.Validate(this);

                var properties = TypeDescriptor.GetProperties(model)
                    .Where(c => c.ToEvaluate())
                    .ToList();

                foreach (PropertyDescriptor item in properties)
                {

                    try
                    {

                        var value = item.GetValue(model);
                        var result = item.ValidateValue(value, this.Translator);

                        foreach (var message in result.Messages)
                            this.AddError(message, item.Name, new TargetSource(model));

                        if (MustEvaluate(result.Value))
                        {

                            if (result.Value is IEnumerable list)
                                foreach (var item2 in list)
                                {
                                    if (MustEvaluate(result.Value))
                                        EvaluateModel(item2, ctx);
                                }

                            else
                                EvaluateModel(result.Value, ctx);

                        }

                    }
                    catch (Exception ex)
                    {
                        this.AddError("Failed to resolve the value." + ex.Message, item.Name, new TargetSource(model));
                    }

                }

            }

        }

        private bool MustEvaluate(object? value)
        {

            if (value == null)
                return false;

            var type = value.GetType();

            if (type.IsEnum)
                return false;

            if (_types.Contains(type))
                return false;

            foreach (var item in _types)
                if (item.IsAssignableFrom(type))
                    return false;

            return true;

        }

        public ITranslateService Translator { get; set; }

        public void AddInfo(string message, string source, TargetSource target)
        {
            this.Add(new DiagramDiagnostic() { Level = DiagnosticLevel.Info, Message = message, Source = source, Target = target });
        }

        public void AddWarning(string message, string source, TargetSource target)
        {
            this.Add(new DiagramDiagnostic() { Level = DiagnosticLevel.Warning, Message = message, Source = source, Target = target });
        }

        public void AddError(string message, string source, TargetSource target)
        {
            this.Add(new DiagramDiagnostic() { Level = DiagnosticLevel.Error, Message = message, Source = source, Target = target });
        }

        private class Context
        {

            public Context()
            {
                this._dic = new Dictionary<object, object>();
            }

            public bool Contains(object model)
            {
                return _dic.ContainsKey(model);
            }

            public IDisposable Add(object model)
            {
                var item = new Disposable(this, model);
                _dic.Add(model, item);
                return item;
            }


            private void Remove(object model)
            {
                _dic.Remove(model);
            }

            private Dictionary<object, object> _dic;


            private class Disposable : IDisposable
            {

                public Disposable(Context ctx, object model)
                {
                    this._ctx = ctx;
                    this._model = model;
                }

                public void Dispose()
                {
                    _ctx.Remove(_model);
                }

                private Context _ctx;
                private object _model;


            }

        }


        private HashSet<Type> _types = new HashSet<Type>()
        {
            //typeof(Delegate),
            typeof(DiagramDiagnostics),
            typeof(MethodInfo),
            typeof(Module),
            typeof(Assembly),
            typeof(Type),
            typeof(string),
            typeof(int),
            typeof(long),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(bool),
            typeof(char),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(sbyte),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(int?),
            typeof(long?),
            typeof(decimal?),
            typeof(double?),
            typeof(float?),
            typeof(bool?),
            typeof(char?),
            typeof(byte?),
            typeof(short?),
            typeof(ushort?),
            typeof(uint?),
            typeof(ulong?),
            typeof(sbyte?),
            typeof(DateTime?),
            typeof(DateTimeOffset?),
            typeof(TimeSpan?),
            typeof(Guid?),
            typeof(Type),
                                                                    };

        private Context _currentCtx;

    }


}
