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
    public class LoggingBuilderInitializer : InjectBuilder<ILoggingBuilder>
    {     

        public override object Execute(ILoggingBuilder context)
        {
            context.ClearProviders();
            context.SetMinimumLevel(LogLevel.Trace);
            return context;
        }

    }

}
