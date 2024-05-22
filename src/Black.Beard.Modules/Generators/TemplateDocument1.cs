using RazorEngine;
using System.Globalization;
using ICSharpCode.Decompiler.IL;

namespace Bb.Generators
{

    public abstract class TemplateDocument<T> : TemplateDocument
    {


        public override IEnumerable<GeneratedDocument> Generate<TModel>(TModel model, ContextGenerator ctx)
        {

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model is T t)
                foreach (var item in function(t, ctx))
                    yield return item;

            else
                throw new ArgumentException($"The model is not of type {typeof(T).Name}");

        }


        public TemplateDocument<T> GetModels<TModel>(Func<T, IEnumerable<TModel>> models, Func<TModel, string> nameFunction, Func<TModel, string> relativPathFunction = null)
        {

            function = (T model, ContextGenerator ctx) =>
            {

                List<GeneratedDocument> result = new List<GeneratedDocument>();

                var list = models(model).ToList();

                foreach (var item2 in this.Generate(list, ctx))
                    result.Add(item2);

                return result;
            };

            _functionName = (object model) => nameFunction((TModel)model);
            if (relativPathFunction != null)
                _functionRelativPath = (object model) => relativPathFunction((TModel)model);


            return this;

        }


        public override bool ModelIsConfigured  => function != null;

        public override bool FunctionNameIsConfigured  => _functionName != null;

        protected abstract IEnumerable<GeneratedDocument> Generate<TModel>(IEnumerable<TModel> models, ContextGenerator ctx);

        internal protected Func<object, string> _functionName;
        internal protected Func<object, string> _functionRelativPath;

        private Func<T, ContextGenerator, List<GeneratedDocument>> function;


    }

}
