using RazorEngine.Templating;
using System.Text;
using RazorEngine.Configuration;
using System.Reflection;
using System.Xml.Linq;

namespace Bb.Generators
{
    public class RazorTemplateDocument<T> : TemplateDocument<T>
    {

        public RazorTemplateDocument()
        {
            this._uuid = Guid.NewGuid().ToString();
            _config = new TemplateServiceConfiguration()
            {

            };
        }

        public bool TemplateIsConfigured => _template != null;


        public RazorTemplateDocument<T> WithTemplateFile(string file)
        {
            _template = new LoadedTemplateSource(file.LoadFromFile(), file);
            return this;
        }


        public RazorTemplateDocument<T> WithTemplate(string templatepayload, string filename = null)
        {
            _template = new LoadedTemplateSource(templatepayload, filename ?? "templateFile");
            return this;
        }

        public RazorTemplateDocument<T> WithTemplateFromResource(string resourceName)
        {

            var r = ResourceHelper.GetResources(resourceName).ToList();

            if (r.Count > 1)
                throw new Exception($"Too many resource found for {resourceName}");

            else if (r.Count == 0)
                throw new Exception($"Resource {resourceName} not found");

            var item = r.First();

            var templatepayload = item.GetResource();

            _template = new LoadedTemplateSource(templatepayload, item.ResourceName);
            return this;
        }


        public RazorTemplateDocument<T> Configure(Action<TemplateServiceConfiguration> initializer)
        {
            initializer(_config);
            return this;
        }


        protected override IEnumerable<GeneratedDocument> Generate<TModel>(IEnumerable<TModel> models, ContextGenerator ctx)
        {

            _config.AllowMissingPropertiesOnDynamic = true;
            _config.Debug = true;
            _config.Language = RazorEngine.Language.CSharp;
            _config.EncodedStringFactory = new RazorEngine.Text.RawStringFactory();
            //_config.ConfigureCompilerBuilder = builder =>
            //{

            //    //builder.SetCompilerServiceFactory(new Bb.RazorEngine.Compilers.CSharp.CSharpCompilerServiceFactory());
            //};

            var service = RazorEngineService.Create(_config);

            bool first = true;
            ITemplateKey key = null;

            foreach (var model in models)
            {
                StringBuilder body = null;
                try
                {

                    if (first)
                    {
                        key = service.GetKey(_template.TemplateFile);
                        service.AddTemplate(key, new LoadedTemplateSource(_template.Template));
                    }

                    body = new StringBuilder();
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (TextWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
                        {
                            if (first)
                                service.RunCompile(key, streamWriter, model: model);
                            else
                                service.Run(key, streamWriter, model: model);

                            streamWriter.Flush();
                        }

                        body = new StringBuilder(Encoding.UTF8.GetString(memoryStream.ToArray()).Trim());

                    }
                }
                catch (Exception ew)
                {

                }

                if (body != null)
                {
                    var result = new GeneratedDocument(this, ctx)
                    {
                        Filename = _functionName?.Invoke(model) ?? model.GetType().Name,                        
                        RelativePath = _functionRelativPath?.Invoke(model) ?? string.Empty,
                        Content = body
                    };

                    result.Filename = result.Filename.MakeValidFileName();

                    result.Filename += ("." + this.Extension);

                    yield return result;
                }

                first = false;

            }

        }

        public string Template { get; set; }


        private readonly string _uuid;
        private readonly TemplateServiceConfiguration _config;
        private LoadedTemplateSource _template;
    }

}
