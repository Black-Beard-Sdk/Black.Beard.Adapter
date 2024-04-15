using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel;
using System.Diagnostics;


namespace Bb.UserInterfaces

{

    [DebuggerDisplay("{Display}")]
    public class DynamicServerMenu : List<DynamicServerMenu> //, ILocalSite
    {

        public DynamicServerMenu(int capacity)
            : base(capacity)
        {

            //_parent = parent;
            //_parent.Register(this);
            //_parent.PropertyChanged += _parent_PropertyChanged;
            //_parent.Disposed += _parent_Disposed;

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




        //#region ILocalSite

        public IServiceProvider ServiceProvider { get; set; }

        //public ILocalComponent Component => _parent;


        //private void _parent_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        //{

        //}

        //private void _parent_Disposed(object? sender, EventArgs e)
        //{

        //}

        //#region IDisposable

        //private bool disposedValue;

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposedValue)
        //    {
        //        if (disposing)
        //        {
        //            // TODO: supprimer l'état managé (objets managés)
        //        }

        //        // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
        //        // TODO: affecter aux grands champs une valeur null
        //        disposedValue = true;
        //    }
        //}

        //// // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
        //// ~DynamicServerMenu()
        //// {
        ////     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        ////     Dispose(disposing: false);
        //// }

        //public void Dispose()
        //{
        //    // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        //    Dispose(disposing: true);
        //    GC.SuppressFinalize(this);
        //}

        //#endregion IDisposable

        //#endregion ILocalSite



        private ILocalComponent _parent;
        private Delegate _action;
    }



}
