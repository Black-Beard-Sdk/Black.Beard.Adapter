//using Bb.ComponentModel;
//using Bb.ComponentModel.Attributes;
//using Bb.ComponentModel.Loaders;

//namespace Bb.Loaders
//{


//    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<WebApplicationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
//    public class EtlInitializeWebApplicationBuilder : IInjectBuilder<WebApplicationBuilder>
//    {

//        public EtlInitializeWebApplicationBuilder()
//        {


//        }

//        public Type Type => typeof(WebApplicationBuilder);

//        public string FriendlyName => typeof(EtlInitializeWebApplicationBuilder).Name;

//        public bool CanExecute(WebApplicationBuilder builder)
//        {
//            return true;
//        }

//        public bool CanExecute(object context)
//        {
//            return CanExecute((WebApplicationBuilder)context);
//        }

//        public object Execute(WebApplicationBuilder context)
//        {
//            return null;
//        }

//        public object Execute(object context)
//        {
//            return Execute((WebApplicationBuilder)context);
//        }

//    }

  
//}
