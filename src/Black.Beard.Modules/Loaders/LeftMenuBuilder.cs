using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using Bb.UIComponents.Glyphs;


namespace Bb.Loaders
{

    [ExposeClass(UIConstants.LeftMenu, ExposedType = typeof(IInjectBuilder<UIService>))]
    public class LeftMenuBuilder : IInjectBuilder<UIService>
    {

        public LeftMenuBuilder()
        {

        }

        public Type Type => typeof(UIService);


        public string FriendlyName => GetType().Name;


        public bool CanExecute(object context)
        {
            return CanExecute((UIService)context);
        }

        public bool CanExecute(UIService service)
        {
            return true;
        }

        public object Execute(UIService service)
        {

            service.Initialize(UIKeys.Menus.LeftMenu, UIKeys.Menus.Home, menu =>
            {

                menu.WithDisplay(new TranslatedKeyLabel("LeftMenu", "Home", null, null))
                    .SetActionMatchAll()
                    .SetIcon(GlyphFilled.Home)
                    .Menu(NewModule, m =>
                    {
                        m.WithDisplay("New module")
                         .SetExecute(Actions.ExecuteNewModule)
                        ;
                    })
                ;

            });


            return 0;
        }

        public object Execute(object context)
        {
            return Execute((UIService)context);
        }

        static readonly Guid NewModule = new("{C8063B0B-B057-4BCB-8629-19D149FE9181}");
        static readonly Guid guidConnectors = new("{C8063B0B-B057-4BCB-8629-19D149FE9881}");

    }


}