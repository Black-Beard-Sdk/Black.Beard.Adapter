using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.UIComponents;
using Bb.UIComponents.Glyphs;


namespace Bb.Loaders
{

    [ExposeClass(UIConstants.LeftMenu)]
    public class LeftMenuBuilder : IInjectBuilder<UIService>
    {

        public LeftMenuBuilder()
        {

        }

        public Type Type => typeof(UIService);


        public string FriendlyName => GetType().Name;


        public object Run(UIService service)
        {

            var guidHome = UIService.Guids.Home;
            var guidConfigurations = UIService.Guids.Configurations;

            service.GetMenuOrCreate(UIService.LeftMenu, guidHome, "p:LeftMenu,k:Home,l:en-us, d:Home")
                .SetActionMatchAll()
                .SetIcon(GlyphFilled.Home)
                ;                    

            return 0;

        }

        public object Run(object context)
        {
            return Run((UIService)context);
        }

        public bool CanExecute(object context)
        {
            return CanExecute((UIService)context);
        }

        public bool CanExecute(UIService service)
        {
            return true;
        }

        public object Execute(UIService context)
        {
            return 0;
        }

        static readonly Guid guidConnectors = new("{C8063B0B-B057-4BCB-8629-19D149FE9881}");

    }

}