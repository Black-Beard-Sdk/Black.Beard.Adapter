using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using ICSharpCode.Decompiler.CSharp.Syntax;
using Microsoft.AspNetCore.Components.Routing;

namespace Bb.UIComponents
{

    public class UIComponentMenu : UIComponent
    {

        public UIComponentMenu(Guid? uuid, TranslatedKeyLabel? display = null)
            : base(uuid, display)
        {

            KeyboardArrowDown = true;
            _viewGuards = new List<GuardContainer>();
            _enabledGuards = new List<GuardContainer>();
        }

        public bool KeyboardArrowDown { get; private set; }


        public ActionReference? Action { get; private set; }


        public IEnumerable<GuardContainer> ViewGuards { get => _viewGuards; }

        public IEnumerable<GuardContainer> EnabledGuards { get => _enabledGuards; }

        public UIComponent SetKeyboardArrowDown(bool activated)
        {
            KeyboardArrowDown = activated;
            return this;
        }

        public UIComponentMenu SetViewGuard<TIGuardMenu>(Func<TIGuardMenu, bool> evaluator)
            where TIGuardMenu : IGuardMenu
        {

            var guard = new GuardContainer<TIGuardMenu>(evaluator)
            {

            };

            this._viewGuards.Add(guard);

            return this;

        }

        public UIComponentMenu SetEnabledGuard<TIGuardMenu>(Func<TIGuardMenu, bool> evaluator)
           where TIGuardMenu : IGuardMenu
        {

            var guard = new GuardContainer<TIGuardMenu>(evaluator)
            {

            };

            this._enabledGuards.Add(guard);

            return this;

        }

        public UIComponentMenu SetAction(NavLinkMatch match, Type type)
        {
            this.Action = this.Service.GetAction(match, type);
            return this;
        }

        public UIComponentMenu SetAction(NavLinkMatch match, string route)
        {
            this.Action = this.Service.GetAction(match, route);
            return this;
        }

        public UIComponentMenu SetActionMatchAll()
        {

            this.Action = new ActionReference()
            {
                Match = NavLinkMatch.All,
                HRef = string.Empty,
            };

            return this;


        }

        private readonly List<GuardContainer> _viewGuards;
        private readonly List<GuardContainer> _enabledGuards;


    }



}


