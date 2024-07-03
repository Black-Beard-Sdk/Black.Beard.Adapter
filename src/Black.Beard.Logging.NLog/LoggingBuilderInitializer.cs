using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Microsoft.Extensions.Logging;

namespace Bb.Logging.NLog
{


    /// <summary>
    /// Load configuration from git
    /// </summary>
    /// <example>
    /// If you want to load configuration from git, you must add variables in your environment
    /// GitRemoteUrl : the url of the git repository
    /// GitUserName : the user name
    /// GitEmail : the user email
    /// GitPassword : the user password
    /// GitBranch : the branch to use. by default the main branch is used.
    /// </example>
    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<ILoggingBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class LoggingBuilderInitializer : IInjectBuilder<ILoggingBuilder>
    {

        public string FriendlyName => typeof(LoggingBuilderInitializer).Name;

        public Type Type => typeof(LoggingBuilderInitializer);

        public LoggingBuilderInitializer()
        {

        }

        public object Execute(object context)
        {
            return Execute((ILoggingBuilder)context);
        }

        public bool CanExecute(object context)
        {
            return CanExecute((ILoggingBuilder)context);
        }

        public bool CanExecute(ILoggingBuilder context)
        {
            return true;
        }

        public object Execute(ILoggingBuilder context)
        {
            context.ClearProviders();
            context.SetMinimumLevel(LogLevel.Trace);
            return context;
        }

    }

}
