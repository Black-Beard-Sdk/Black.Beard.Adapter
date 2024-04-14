using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bb.UIComponents
{


    public class MenuConverter : IMenuConverter
    {

        public MenuConverter(ITranslateService translateService, GuardMenuProvider? guardProvider, IServiceProvider serviceProvider)
        {
            this._guardProvider = guardProvider;
            this._translateService = translateService;
            this._serviceProvider = serviceProvider;
        }

        public object Convert(UIComponent c)
        {

            var menu = new DynamicServerMenu(c.Children.Count())
            {
                Uui = c.Uuid,
                
                Display = c.Display != null 
                    ? this._translateService.Translate(c.Display) 
                    : string.Empty,

                Type = c.Type,
                Icon = c.Icon.Value,
                EnabledGuard = true,
                ViewGuard = true,
                ServiceProvider = _serviceProvider,

            };


            if (c is UIComponentMenu u)
            {

                foreach (GuardContainer guard in u.ViewGuards)
                    if (!_guardProvider.Get(guard))
                    {
                        menu.ViewGuard = false;
                        break;
                    }

                foreach (GuardContainer guard in u.EnabledGuards)
                    if (!_guardProvider.Get(guard))
                    {
                        menu.EnabledGuard = false;
                        break;
                    }

                if (u.KeyboardArrowDown)
                    menu.KeyboardArrowDown = Glyphs.GlyphFilled.KeyboardArrowDown.Value;

                if (u.Action != null)
                    menu.Action = u.Action;

                else if (c.Execute != null)
                {
                    menu.SetExecute(c.Execute);
                }
                else
                    menu.Action = new ActionReference()
                    {
                        HRef = string.Empty,
                        Match = NavLinkMatch.All,
                    };
            }

            menu.Roles.AddRange(c.Roles);

            foreach (var item in c.Children.OfType<UIComponentMenu>())
                menu.Add((DynamicServerMenu)Convert(item));

            return menu;

        }


        private readonly GuardMenuProvider? _guardProvider;
        private readonly ITranslateService _translateService;
        private readonly IServiceProvider _serviceProvider;
    }


}
