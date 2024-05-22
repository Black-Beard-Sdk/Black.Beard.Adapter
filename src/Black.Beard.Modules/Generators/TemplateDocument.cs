using RazorEngine;
using System.Globalization;
using ICSharpCode.Decompiler.IL;

namespace Bb.Generators
{

    public abstract class TemplateDocument
    {

        public string Extension { get; set; }

        public abstract IEnumerable<GeneratedDocument> Generate<TModel>(TModel model, ContextGenerator ctx);

        public abstract bool ModelIsConfigured { get; }

        public abstract bool FunctionNameIsConfigured { get; }

    }

}
