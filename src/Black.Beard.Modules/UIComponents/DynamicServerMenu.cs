using Bb.ComponentModel.Factories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Bb.UIComponents
{

    [DebuggerDisplay("{Display}")]
    public class DynamicServerMenu : List<DynamicServerMenu>
    {

        public DynamicServerMenu(int capacity)
            : base(capacity)
        {
            this.Roles = new List<string>();
            this.Icon = string.Empty;
            this.Action = ActionReference.Default;
        }

        public bool IsEmpty { get => this.Count == 0; }

        public bool ViewGuard { get; set; }

        public bool EnabledGuard { get; set; }

        public string? Display { get; set; }

        public Guid Uui { get; set; }

        public string? Type { get; set; }

        public List<string> Roles { get; set; }

        public bool HasImage { get => !string.IsNullOrEmpty(this.Icon); }

        public string Icon { get; set; }

        public string? KeyboardArrowDown { get; set; }

        public ActionReference Action { get; set; }

        public void SetExecute(Delegate action)
        {
            this._action = action;
            OnClick = EventCallback.Factory.Create<MouseEventArgs>(this, OnClickImpl);
        }

        public EventCallback<MouseEventArgs> OnClick { get; private set; }

        public IServiceProvider ServiceProvider { get; internal set; }

        private void OnClickImpl(MouseEventArgs e)
        {
            List<object> args = new List<object>();
            var parameters = _action.Method.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                if (p.ParameterType == typeof(MouseEventArgs))
                    args.Add(e);
                else if (p.ParameterType == typeof(EventContext))
                    args.Add(new EventContext(e, this));
                else
                    args.Add(this.ServiceProvider.GetService(p.ParameterType));
            }


            this._action.DynamicInvoke(args.ToArray());
        }
     
        private Delegate _action;

    }

}
