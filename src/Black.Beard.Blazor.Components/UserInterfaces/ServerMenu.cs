using Bb.ComponentModel;
using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using Bb.UIComponents.Guards;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using static MudBlazor.CategoryTypes;


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
            this.Icon = string.Empty;
            this.Action = ActionReference.Empty;
            this.ViewGuard = new GuardBlock(serviceProvider);
            this.EnabledGuard = new GuardBlock(serviceProvider);
        }

        public Guid Uui { get; set; }

        public bool IsEmpty { get => _list.Count == 0; }

        public TranslatedKeyLabel Display { get; set; }

        public GuardBlock ViewGuard { get; }

        public GuardBlock EnabledGuard { get; }

        public bool HasImage { get => !string.IsNullOrEmpty(this.Icon); }

        public string Icon { get; set; }

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
                        s.Append(menu);
                    }
                }
            };

            _list.Add(m);

            return this;

        }


        private readonly List<ServerMenu> _list;


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

        public ServerMenu SetKeyboardArrowDown(string glyph)
        {
            this.KeyboardArrowDown = glyph;
            return this;
        }

        public ServerMenu WithDisplay(TranslatedKeyLabel display)
        {
            this.Display = display;
            return this;
        }

        public ServerMenu DoActionMatchAll()
        {

            this.Action = new ActionReference()
            {
                Match = NavLinkMatch.All,
                HRef = string.Empty,
            };

            return this;

        }

        public ServerMenu DoAction(string path, NavLinkMatch nav)
        {

            this.Action = new ActionReference()
            {
                Match = nav,
                HRef = path,
            };

            return this;

        }

        public ServerMenu WithIcon(Glyph glyph)
        {
            this.Icon = glyph.Value;
            return this;
        }

        public ServerMenu WithIcon(string glyph)
        {
            this.Icon = glyph;
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


        private Delegate _action;
        private bool _scoped;

        public event DisposedEventHandler? Disposed;
        public event PropertyChangedEventHandler? PropertyChanged;

    }



}
