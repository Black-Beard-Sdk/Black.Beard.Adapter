//using Bb.ComponentModel;
//using Bb.ComponentModel.Attributes;
//using Bb.ComponentModel.Loaders;

//namespace Bb.Logging.NLog
//{

//    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<Initializer>), LifeCycle = IocScopeEnum.Transiant)]
//    [Priority(1)]
//    public class ConfigLoggerInitializer : IInjectBuilder<Initializer>
//    {

//        public string FriendlyName => typeof(NLogInitializer).Name;

//        public Type Type => typeof(ConfigLoggerInitializer);

//        public bool CanExecute(Initializer context) => context.CanExecuteModule(FriendlyName);

//        public bool CanExecute(object context) => CanExecute((Initializer)context);

//        public object Execute(object context) => Execute((Initializer)context);

//        public object Execute(Initializer context)
//        {

//            return null;
//        }


//    }

//}
