namespace Bb.Generators
{


    public class Generator
    {


        public Generator()
        {
            this.Templates = new List<TemplateDocument>();
        }


        public List<TemplateDocument> Templates { get; set; }


        public ContextGenerator Context { get; set; }


        public Generator AddRazorTemplate<T>(string extension, Action<RazorTemplateDocument<T>> initializer)
        {

            var template = new RazorTemplateDocument<T>()
            {
                Extension = extension.Trim('.').Trim().Trim('.'),
            };

            initializer(template);

            if (!template.TemplateIsConfigured)
                throw new InvalidOperationException("template is not set");

            this.AddTemplate(template);

            return this;

        }


        public Generator AddTemplate(TemplateDocument template)
        {

            if (template == null)
                throw new ArgumentNullException(nameof(template));

            if (!template.ModelIsConfigured )
                throw new InvalidOperationException("model is not set");

            if (!template.FunctionNameIsConfigured)
                throw new InvalidOperationException("FunctionName is not set");

            this.Templates.Add(template);

            return this;
        }

        public IEnumerable<GeneratedDocument> Generate(object model)
        {

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            foreach (var item in this.Templates)
                foreach (var item2 in item.Generate(model, Context))
                    yield return item2; 

        }

    }


}
