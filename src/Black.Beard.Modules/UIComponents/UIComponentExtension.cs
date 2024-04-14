using Bb.ComponentModel.Translations;
using System.Linq.Expressions;

namespace Bb.UIComponents
{
    public static class UIComponentExtension
    {


        public static T WithDisplay<T>(this T self, TranslatedKeyLabel display)
            where T : UIComponent
        {
            self.Display = display;
            return self;
        }


        public static T SetIcon<T>(this T self, Glyph glyph)
            where T : UIComponent
        {
            self.Icon = glyph;
            return self;
        }


        public static T WithRoles<T>(this T self, params string[] roles)
            where T : UIComponent
        {

            foreach (var role in roles)
                self.Roles.Add(role);

            return self;

        }


        public static T Menu<T>(this T self, Guid guid, Action<UIComponentMenu> action)
            where T : UIComponent
        {

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (guid == Guid.Empty)
                throw new ArgumentNullException(nameof(guid));

            UIComponentMenu m = null;

            var item = self._children.Where(c => c.Uuid == guid).FirstOrDefault();
            if (item != null)
                m = (UIComponentMenu)item;

            else
                self._children.Add(m = new UIComponentMenu(guid, null) { Parent = self, });

            action(m);

            return self;

        }


        public static UIComponent RemoveChild<T>(this T self, UIComponent child)
            where T : UIComponent
        {
            self._children.Remove(child);
            child.Parent = null;
            return self;

        }


        public static T SetExecute<T>(this T self, Delegate  action)
            where T : UIComponent
        {
            self.Execute = action;
            return self;
        }


    }

}
