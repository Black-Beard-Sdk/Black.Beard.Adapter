using Bb.ComponentModel;
using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using Bb.UIComponents.Guards;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace Bb.UserInterfaces
{

    [DebuggerDisplay("{Display}")]
    public class ServerMenu : IEnumerable<ServerMenu>
    {

        public ServerMenu(IServiceProvider serviceProvider)
            : base()
        {

            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            this.ServiceProvider = serviceProvider;
            this._list = new List<ServerMenu>();
            //this.Action = ActionReference.Empty;
            this.ViewGuard = new GuardBlock(serviceProvider);
            this.EnabledGuard = new GuardBlock(serviceProvider);
        }

        public Guid Uui { get; set; }

        public virtual bool IsEmpty { get => _list.Count == 0; }

        public TranslatedKeyLabel Display { get; set; }

        public GuardBlock ViewGuard { get; }

        public GuardBlock EnabledGuard { get; }


        public bool DividerAfter
        {
            get
            {
                return _diviserAfter && (Parent?._list.IndexOf(this) < Parent?._list.Count);
            }
        }

        public bool IsLast { get => Parent?._list.IndexOf(this) == Parent?._list.Count - 1; }

        public bool IsFirst { get => Parent?._list.IndexOf(this) == 0; }

        public bool IsAlone { get => Parent?._list.Count == 1; }

        public string? KeyboardArrowDown { get; set; }

        public ActionReference? Action { get; set; }

        public EventCallback<MouseEventArgs> OnClick { get; private set; }

        public IServiceProvider ServiceProvider { get; }

        public ServerMenu Parent { get; set; }
        public object? Model { get; private set; }

        private void OnClickImpl(MouseEventArgs e)
        {



            IServiceScope scope = null;
            var service = this.ServiceProvider;
            if (_scoped)
            {
                scope = service.CreateScope();
                service = scope.ServiceProvider;
            }

            var ctx = new EventContext(e, this, scope, service);


            List<object> args = new List<object>();
            var parameters = _action.Method.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                if (p.ParameterType == typeof(MouseEventArgs))
                    args.Add(e);
                else if (p.ParameterType == typeof(EventContext))
                    args.Add(ctx);
                else
                    args.Add(service.GetService(p.ParameterType));
            }

            this._action.DynamicInvoke(args.ToArray());



        }

        public virtual void Dispose()
        {

        }

        public IEnumerator<ServerMenu> GetEnumerator()
        {
            return GetEnumeratorImpl();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumeratorImpl();
        }



        protected virtual IEnumerator<ServerMenu> GetEnumeratorImpl()
        {

            List<ServerMenu> list = new List<ServerMenu>();

            foreach (var sub in _list)
                if (sub is DynamicServerMenu m)
                    m.Builder(sub, list);
                else
                    list.Add(sub);

            return list.GetEnumerator();

        }


        public ServerMenu MenuStatic(Guid guid, Action<ServerMenu> action)
        {

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (guid == Guid.Empty)
                throw new ArgumentNullException(nameof(guid));

            var m = new ServerMenu(this.ServiceProvider)
            {
                Uui = guid,
                Parent = this,
            };
            action(m);

            _list.Add(m);

            return this;

        }

        public ServerMenu MenuDynamic<T>(Func<IEnumerable<T>> items, Action<T, ServerMenu> action)
        {

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var m = new DynamicServerMenu(this.ServiceProvider)
            {
                Uui = Guid.Empty,
                Parent = this,
                Builder = (m, s) =>
                {
                    foreach (var item in items())
                    {
                        var menu = new ServerMenu(m.ServiceProvider)
                        {
                            Uui = Guid.NewGuid(),
                            Parent = m,
                            Model = item,
                        };
                        action(item, menu);
                        s.Add(menu);
                    }
                }
            };

            _list.Add(m);

            return this;

        }

        public ServerMenu WithExecute(Delegate? action, bool scoped)
        {
            if (action != null)
            {
                this._action = action;
                this._scoped = scoped;
                OnClick = EventCallback.Factory.Create<MouseEventArgs>(this, OnClickImpl);
            }
            return this;
        }

        public ServerMenu DoAction(ActionReference? actionReference)
        {
            if (actionReference != null)
                this.Action = actionReference;
            return this;
        }

        public ServerMenu WithIcon(Glyph glyph)
        {
            _iconIsDefined = true;
            this._icon = glyph.Value;
            return this;
        }

        public ServerMenu WithIcon(string glyph)
        {
            _iconIsDefined = true;
            this._icon = glyph;
            return this;
        }

        public bool HasImage
        {
            get
            {
                return _iconIsDefined && !string.IsNullOrEmpty(this.Icon);
            }
        }

        public string Icon
        {
            get
            {

                if (!_iconIsDefined && _list.Count > 0)
                    return @Icons.Material.TwoTone.ArrowDropDown;

                return _icon;

            }
        }


        public ServerMenu WithDividerAfter(bool value = true)
        {
            this._diviserAfter = value;
            return this;
        }

        public ServerMenu SetKeyboardArrowDown(string glyph)
        {
            if (string.IsNullOrEmpty(glyph))
                throw new ArgumentNullException(nameof(glyph));

            this.KeyboardArrowDown = glyph;
            return this;
        }

        public ServerMenu WithDisplay(TranslatedKeyLabel display)
        {
            if (display == null)
                throw new ArgumentNullException(nameof(display));

            this.Display = display;
            return this;
        }

        public ServerMenu WithLinkMatchAll()
        {

            this.Action = new ActionReference(string.Empty)
            {
                Match = NavLinkMatch.All
            };

            return this;

        }

        public ServerMenu NavigateTo<T>(Action<ActionReference<T>> action)
            where T : ComponentBase
        {
            var a = new ActionReference<T>()
            {
                Match = NavLinkMatch.Prefix,
            };
            action(a);
            this.Action = a;
            return this; ;

        }

        public ServerMenu NavigateTo<T>(NavLinkMatch nav = NavLinkMatch.Prefix)
            where T : ComponentBase
        {
            var attribute = typeof(T).GetCustomAttribute<RouteAttribute>(true);
            return NavigateTo(attribute?.Template, nav);
        }

        public ServerMenu NavigateTo(string path, NavLinkMatch nav = NavLinkMatch.Prefix)
        {

            this.Action = new ActionReference(path)
            {
                Match = nav,            
            };

            return this;

        }

        public ServerMenu WithViewPolicies(params string[] policies)
        {
            ViewGuard.Add(policies);
            return this;
        }

        public ServerMenu WithEnabledPolicies(string policies)
        {
            EnabledGuard.Add(policies);
            return this;
        }

        private readonly List<ServerMenu> _list;
        private Delegate _action;
        private bool _scoped;
        private bool _diviserAfter;
        private bool _iconIsDefined = false;
        private string _icon;

        public event DisposedEventHandler? Disposed;
        public event PropertyChangedEventHandler? PropertyChanged;

    }



}
