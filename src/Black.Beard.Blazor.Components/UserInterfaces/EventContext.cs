using Bb.ComponentModel;
using Microsoft.AspNetCore.Components.Web;

namespace Bb.UserInterfaces
{

    public class EventContext : IDisposable
    {

        #region ctors

        public EventContext(MouseEventArgs mouseEventArgs)
        {
            MouseEventArgs = mouseEventArgs;
        }

        public EventContext(MouseEventArgs mouseEventArgs, ServerMenu dynamicServerMenu, IServiceScope? scope, IServiceProvider serviceProvider) 
            : this(mouseEventArgs)
        {
            this._serviceProvider = serviceProvider;
            DynamicServerMenu = dynamicServerMenu;
            this._scope = scope;
        }

        #endregion ctors


        public MouseEventArgs MouseEventArgs { get; }


        public IServiceProvider ServiceProvider => _serviceProvider;


        public ServerMenu DynamicServerMenu { get; }


        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(Disposed != null)
                        Disposed(this, EventArgs.Empty);
                    _scope?.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
        // ~EventContext()
        // {
        //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion Dispose


        public event DisposedEventHandler? Disposed;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope? _scope;
        private bool disposedValue;


    }



}
