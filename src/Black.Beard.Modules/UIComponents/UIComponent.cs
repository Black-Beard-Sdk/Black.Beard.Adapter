using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using System.Diagnostics;

namespace Bb.UIComponents
{


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


        public UIComponent? Parent { get; private set; }


        public Guid Uuid { get; set; }


        public Glyph Icon { get; set; }


        public TranslatedKeyLabel Display { get; set; }


        public HashSet<string> Roles { get; }


        public UIComponent WithRoles(params string[] roles)
        {

            foreach (var role in roles)
                Roles.Add(role);

            return this;

        }

        public UIComponent SetIcon(Glyph glyph)
        {
            this.Icon = glyph;
            return this;
        }

        public object Convert(IMenuConverter menuConverter)
        {
            return menuConverter.Convert(this);
        }

        public string Type { get; set; }


        public UIComponent RemoveChild(UIComponent child)
        {

            this._children.Remove(child);
            child.Parent = null;

            return this;

        }

        
        public UIComponentMenu AddMenu(Guid? guid, TranslatedKeyLabel label = null)
        {

            if (guid == null)
                guid = Guid.NewGuid();

            return this.Add(new UIComponentMenu(guid, label));
        }

        
        public T Add<T>(T child)
            where T : UIComponent
        {

            if (child.Parent != null)
                child.Parent.RemoveChild(child);

            child.Parent = this;
            child.Service = this.Service;
            child.Type = this.Type;

            this._children.Add(child);

            return child;

        }

        public IEnumerable<UIComponent> Children { get; }

        public static class Types
        {

            public const string Menu = "Menu";

        }

        private readonly List<UIComponent> _children;

    }


}
