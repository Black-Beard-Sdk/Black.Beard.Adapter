using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Translations;
using Bb.Modules;
using Bb.Pages;
using Bb.UIComponents;
using Bb.UIComponents.Glyphs;


namespace Bb.Loaders
{


    [ExposeClass(UIConstants.LeftMenu, ExposedType = typeof(IInjectBuilder<MenuService>))]
    public class MenuServiceBuilder : InjectBuilder<MenuService>
    {


        public MenuServiceBuilder(Modules.Solutions modules)
        {
            _modules = modules;
        }

        public override object Execute(MenuService service)
        {

            service.Initialize(UIKeys.Menus.LeftMenu, UIKeys.Menus.Modules, menu =>
            {

                menu.WithDisplay(ModuleConstants.Solutions)
                    //.DoActionMatchAll()
                    .WithDividerAfter()
                    //.WithIcon(Glyph.Empty)
                    .MenuStatic(NewModule, m =>
                    {

                        m.WithDisplay(ModuleConstants.Manage)
                            .WithViewPolicies("Admin")
                            //.WithExecute(ActionModules.ExecuteNewModule, true)
                            .NavigateTo<PageModules>()
                        ;

                    })

                    .MenuDynamic(() => _modules.GetModules(), (itemModule, menu) =>
                    {

                        menu.WithDisplay(itemModule.Label)
                            .MenuStatic(itemModule.Uuid, f =>
                            {
                                f.WithDisplay(ModuleConstants.Manage)
                                    .WithViewPolicies("Admin")
                                    .NavigateTo<PageModule>(c => c.MapArgument(() => itemModule.Uuid, d => d.Uuid))
                                    ;

                            })
                            .MenuDynamic(() => itemModule.GetFeatures(), (itemFeature, subMenu) =>
                            {

                                subMenu.WithDisplay(itemFeature.Label)
                                       .NavigateTo(itemFeature.GetRoute())
                                    ;

                            })

                        ;

                    })
                ;

            });


            return 0;
        }

        static readonly Guid NewModule = new("{C8063B0B-B057-4BCB-8629-19D149FE9181}");
        private readonly Modules.Solutions _modules;

    }


    /*
        bool	    {active:bool}	    true, FALSE	                                                                    No
        datetime	{dob:datetime}	    2016-12-31, 2016-12-31 7:32pm	                                                Yes
        decimal	    {price:decimal}	    49.99, -1,000.01	                                                            Yes
        double	    {weight:double}	    1.234, -1,001.01e8	                                                            Yes
        float	    {weight:float}	    1.234, -1,001.01e8	                                                            Yes
        guid	    {id:guid}	        CD2C1638-1638-72D5-1638-DEADBEEF1638, {CD2C1638-1638-72D5-1638-DEADBEEF1638}    No
        int	        {id:int}	        123456789, -123456789	                                                        Yes
        long	    {ticks:long}	    123456789, -123456789	                                                        Yes
     */

}