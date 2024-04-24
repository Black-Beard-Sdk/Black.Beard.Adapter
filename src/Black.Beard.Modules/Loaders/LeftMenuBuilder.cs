using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Translations;
using Bb.Modules;
using Bb.UIComponents;
using Bb.UIComponents.Glyphs;


namespace Bb.Loaders
{


    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<IConfigurationBuilder>))]
    public class ConfigurationBuilder : InjectBuilder<IConfigurationBuilder>
    {

        public override object Execute(IConfigurationBuilder service)
        {

            return 0;

        }

    }


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

                menu.WithDisplay(new TranslatedKeyLabel("LeftMenu", "Modules", null, null))
                    .DoActionMatchAll()
                    .WithIcon(GlyphFilled.Home)
                    .MenuStatic(NewModule, m =>
                    {

                        m.WithDisplay("New")
                            .WithViewPolicies("Admin")
                            .WithExecute(ActionModules.ExecuteNewModule, true)
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