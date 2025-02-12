using Bb.ComponentModel.Attributes;
using Bb.ComponentModel;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using System.Diagnostics;
using NLog.Targets;
using NLog;
using Bb.Diagnostics;
using System.Reflection;
using ICSharpCode.Decompiler.Util;
using OpenTelemetry.Exporter;
using static System.Net.WebRequestMethods;

// https://learn.microsoft.com/fr-fr/dotnet/core/diagnostics/observability-prgrja-example
// https://learn.microsoft.com/fr-fr/dotnet/core/diagnostics/metrics-instrumentation#create-a-custom-metric
// https://zipkin.io/pages/quickstart
// docker run -d -p 9411:9411 openzipkin/zipkin
// http://localhost:9411/zipkin/

namespace Bb.Logging.NLog
{


    [ExposeClass(ConstantsCore.Configuration, LifeCycle = IocScopeEnum.Singleton)]
    public class NLogConfiguration
    {

        public bool ExportConsole { get; set; }

        public ExporterConfiguration Zipkin { get; set; }

    }

    public class ExporterConfiguration
    {

        public bool Activated { get; set; }

        public bool Https { get; set; }

        public int Port { get; set; } = 9411;

        // "http://192.168.10.47"
        public string Host { get; set; } = "localhost";


    }


    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<WebApplicationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class ConfigureWebApplicationBuilder : InjectBuilder<WebApplicationBuilder>
    {

        public ConfigureWebApplicationBuilder(ILogger<ConfigureWebApplicationBuilder> logger, IConfiguration configuration, NLogConfiguration nlogConf)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._nlogConf = nlogConf;
        }

        public override object Execute(WebApplicationBuilder builder)
        {

            IEnumerable<string> assemblies = GetSources();


            OtlpExportProtocol protocol = OtlpExportProtocol.HttpProtobuf;
            var services = builder.Services;
            //services.AddResourceMonitoring();

            services.AddOpenTelemetry()
                .ConfigureResource(builder => builder.AddDetector(sp =>
                {
                    return sp.GetRequiredService<IResourceDetector>();
                }))
                .WithTracing(builder =>
                {

                    builder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSqlClientInstrumentation()
                        //.AddWcfInstrumentation()
                        //.AddQuartzInstrumentation()
                        //.AddProcessor()
                        .AddSource(typeof(ActivitySourceItem).Assembly.GetName().Name)
                    ;

                    foreach (var item in assemblies)
                        builder.AddSource(item);

                    if (this._nlogConf.ExportConsole)
                        builder.AddConsoleExporter();

                    //builder.AddOtlpExporter(options =>
                    //     {
                    //         options.Endpoint = new Uri($"{shemeExporter}://{hostExporter}:{portExporter}/v1/traces");
                    //         options.Protocol = protocol;
                    //     });

                    if (this._nlogConf.Zipkin != null && this._nlogConf.Zipkin.Activated)
                        builder.AddZipkinExporter(options =>
                        {
                            var shemeExporter = this._nlogConf.Zipkin.Https ? "https" : "http";
                            var uriExport = new Uri($"{shemeExporter}://{this._nlogConf.Zipkin.Host}:{this._nlogConf.Zipkin.Port}/api/v2/spans");
                            options.Endpoint = uriExport;
                        });

                })
                .WithMetrics(builder =>
                {

                    builder
                        // Metrics provider from OpenTelemetry
                        .AddAspNetCoreInstrumentation()
                        // Metrics provides by ASP.NET Core in .NET 8
                        .AddMeter("Microsoft.AspNetCore.Hosting")
                        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                        //.AddOtlpExporter()

                        ;

                    if (this._nlogConf.ExportConsole)
                        builder.AddConsoleExporter();

                    //if (withZipkin)
                    //    builder.AddZipkinExporter();

                })

                .WithLogging(builder =>
                {

                    //builder
                    //    .AddOtlpExporter()
                    //;

                    if (this._nlogConf.ExportConsole)
                        builder.AddConsoleExporter();

                })

                ;


            //using var activity = activitySource.StartActivity("GreeterActivity");
            //{
            //    // Log a message
            //    _logger.LogInformation("Sending greeting");
            //    // Increment the custom counter
            //    //countGreetings.Add(1);
            //    // Add a tag to the Activity
            //    activity?.SetTag("greeting", "Hello World!");
            //}

            return null;

        }

        private static IEnumerable<string> GetSources()
        {

            Dictionary<string, ActivitySourceItem> items;
            var ass = Assembly.GetEntryAssembly();
            var file = ass.Location.AsFile().Directory.Combine(ass.GetName().Name + ".activities.json").AsFile();
            if (file.Exists)
                items = file.LoadFromFileAndDeserialize<List<ActivitySourceItem>>().ToDictionary(c => c.AssemblyName);
            else
                items = [];


            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
                if (!item.IsDynamic)
                {
                    var f = item.Location.AsFile();
                    f.Refresh();
                    var assemblyName = item.GetName().Name;
                    if (!items.TryGetValue(assemblyName, out var s) || s.FileSize != f.Length)
                        Append(item.Location.FindActivitySourceCreations(), items, f.Length, assemblyName);

                }

            // Save
            var s2 = items.Values.ToList();
            s2.SortBy(c => c.AssemblyName);
            file.FullName.SerializesAndSave(s2);

            // prepare the list to return
            HashSet<string> _h = new HashSet<string>();
            foreach (var item in items)
                foreach (var item2 in item.Value.SourceNames)
                    _h.Add(item2);

            return _h;

        }

        private static void Append(IEnumerable<ActivitySourceInstance> source, Dictionary<string, ActivitySourceItem> dic, long fileSize, string assemblyName)
        {

            HashSet<ActivitySourceItem> _impacted = new HashSet<ActivitySourceItem>();

            var items = source.ToList();
            if (!items.Any())
            {
                if (dic.TryGetValue(assemblyName, out var p))
                    p.FileSize = fileSize;

                else
                    dic.Add(assemblyName, new ActivitySourceItem()
                    {
                        AssemblyName = assemblyName,
                        FileSize = fileSize,
                    });
            }

            foreach (ActivitySourceInstance item in items)
            {

                if (dic.TryGetValue(assemblyName, out var item2))
                {
                    item2.FileSize = fileSize;
                    item2.SourceNames.Clear();
                }
                else
                    dic.Add(assemblyName, item2 = new ActivitySourceItem()
                    {
                        AssemblyName = assemblyName,
                        FileSize = fileSize,
                    });

                _impacted.Add(item2);
                item2.SourceNames.Add(item.SourceName);

            }

            foreach (var item in _impacted)
                item.SourceNames.Sort();

        }

        static ActivitySource activitySource = new ActivitySource(
            typeof(ActivitySourceItem).Assembly.GetName().Name,
            typeof(ActivitySourceItem).Assembly.GetName().Version.ToString());

        private ILogger<ConfigureWebApplicationBuilder> _logger;
        private readonly IConfiguration _configuration;
        private readonly NLogConfiguration _nlogConf;

        private class ActivitySourceItem
        {

            public ActivitySourceItem()
            {
                SourceNames = new List<string>();
            }

            public string AssemblyName { get; set; }

            public long FileSize { get; set; }

            public List<string> SourceNames { get; set; }

        }

    }


    [ExposeClass(ConstantsCore.Service, ExposedType = typeof(IResourceDetector), LifeCycle = IocScopeEnum.Transiant)]
    public class MyResourceDetector : IResourceDetector
    {

        public MyResourceDetector(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        public Resource Detect()
        {
            return ResourceBuilder.CreateEmpty()
                .AddEnvironmentVariableDetector()
                .AddService(serviceName: this.webHostEnvironment.ApplicationName)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["host.environment"] = this.webHostEnvironment.EnvironmentName
                })
                .Build();
        }

        private readonly IWebHostEnvironment webHostEnvironment;

    }


    [Target("OpenTelemetry")]
    public sealed class OpenTelemetryTarget : TargetWithLayout
    {

        public OpenTelemetryTarget()
        {
            _tracer = TracerProvider.Default.GetTracer("NLog");
        }

        protected override void Write(LogEventInfo logEvent)
        {

            var logMessage = this.Layout.Render(logEvent);

            using (var span = _tracer.StartSpan(logEvent.LoggerName))
            {
                span.SetAttribute("log.level", logEvent.Level.Name);
                span.SetAttribute("log.message", logMessage);
            }

        }

        private readonly Tracer _tracer;

    }
}
