using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using ICSharpCode.Decompiler.CSharp.Syntax;
using System;
using System.Diagnostics;
using System.Reflection.Emit;

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


        public static T Add<T>(this T self, T child)
            where T : UIComponent
        {

            if (child.Parent != null)
                child.Parent.RemoveChild(child);

            child.Parent = self;
            child.Service = self.Service;
            child.Type = self.Type;

            self._children.Add(child);

            return child;

        }


        public static UIComponentMenu AddMenu(this UIComponentMenu self, Guid? guid, TranslatedKeyLabel label = null)
        { 
            if (guid == null)
                guid = Guid.NewGuid();
            return self.Add(new UIComponentMenu(guid, label));
        }


        public static UIComponent RemoveChild<T>(this T self, UIComponent child)
            where T : UIComponent
        {
            self._children.Remove(child);
            child.Parent = null;
            return self;

        }


    }


    [DebuggerDisplay("{Display} : {Uuid}")]
    public class UIComponent
    {

        public UIComponent(Guid? uuid, TranslatedKeyLabel? display = null)
        {
            this.Uuid = uuid ?? Guid.NewGuid();
            this.Children = this._children = new List<UIComponent>();
            this.Display = display;
            this.Roles = new HashSet<string>();
            this.Icon = Glyph.Empty;
            this.Type = string.Empty;
            this.Parent = null;

        }


        public UIService? Service { get; internal set; }


        public UIComponent? Parent { get; internal set; }


        public Guid Uuid { get; set; }


        public Glyph Icon { get; set; }


        public TranslatedKeyLabel Display { get; set; }


        public HashSet<string> Roles { get; }


        public object Convert(IMenuConverter menuConverter)
        {
            return menuConverter.Convert(this);
        }


        public string Type { get; set; }


        public IEnumerable<UIComponent> Children { get; }


        internal readonly List<UIComponent> _children;

        internal volatile object _lock = new object();

    }


    public static class UITypes
    {

        public const string Menu = "Menu";

    }

}
