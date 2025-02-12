using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;
using NLog;
using NLog.Targets;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace Bb.Logging.NLog
{


    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<Initializer>), LifeCycle = IocScopeEnum.Transiant)]
    [Priority(1)]
    public class NLogInitializer : InjectBuilder<Initializer>
    {

        public NLogInitializer(ILogger<NLogInitializer> logger)
        {
            _logger = logger;
            _tracer = TracerProvider.Default.GetTracer("ConfigLoggerInitializer");
        }
   
        public override object Execute(Initializer context)
        {
            //Trace.Listeners.Add(new NLogTraceListener());
            Loggers.InitializeLogger();
            _logger.LogInformation("NLog initialized");
            return null;
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ILogger<NLogInitializer> _logger;
        private readonly Tracer _tracer;

    }

}
