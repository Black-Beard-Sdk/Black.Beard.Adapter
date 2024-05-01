using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Translations;
using Bb.Modules;
using Bb.UIComponents;
using Bb.UIComponents.Glyphs;


namespace Bb.Loaders
{


    [ExposeClass(UIConstants.LeftMenu, ExposedType = typeof(IInjectBuilder<MenuService>))]
    public class MenuServiceBuilder : InjectBuilder<MenuService>
    {


        public MenuServiceBuilder(ModuleInstances modules)
        {
            _modules = modules;
        }

        public override object Execute(MenuService service)
        {

            service.Initialize(UIKeys.Menus.LeftMenu, UIKeys.Menus.Modules, menu =>
            {

                menu.WithDisplay(ModuleConstants.Modules)
                    //.DoActionMatchAll()
                    .WithDividerAfter()
                    //.WithIcon(Glyph.Empty)
                    .MenuStatic(NewModule, m =>
                    {

                        m.WithDisplay(ModuleConstants.ManageModules)
                            .WithViewPolicies("Admin")
                            //.WithExecute(ActionModules.ExecuteNewModule, true)
                            .WithNavigate("/pages/PageModule")
                        ;

                    })

                    .MenuDynamic(() => _modules.GetModules(), (item, menu) =>
                    {

                        menu.WithDisplay(item.Label)

                        ;

                    })
                ;

            });


            return 0;
        }

        static readonly Guid NewModule = new("{C8063B0B-B057-4BCB-8629-19D149FE9181}");
        private readonly ModuleInstances _modules;

    }


}