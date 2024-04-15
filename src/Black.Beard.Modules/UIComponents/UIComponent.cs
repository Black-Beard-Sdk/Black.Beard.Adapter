using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using ICSharpCode.Decompiler.CSharp.Syntax;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Bb.UIComponents
{


    [DebuggerDisplay("{Display} : {Uuid}")]
    public class UIComponent //: ILocalComponent
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

        public Delegate Execute { get; internal set; }


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





        //public void Register(ILocalSite site)
        //{
        //    lock (_lock)
        //    {
        //        if (!_sites.Contains(site))
        //            _sites.Add(site);
        //    }
        //}

        //public void Dispose()
        //{
        //    lock (_lock)
        //    {
        //        foreach (var item in _sites)
        //        {
        //            item.Dispose();
        //        }
        //    }
        //}

        //public ILocalSite? Site { get; set; }
        //public event EventHandler? Disposed;
        //public event PropertyChangedEventHandler? PropertyChanged;
        //private List<ILocalSite> _sites = new List<ILocalSite>();




        internal readonly List<UIComponent> _children;
        internal volatile object _lock = new object();
    }

}
