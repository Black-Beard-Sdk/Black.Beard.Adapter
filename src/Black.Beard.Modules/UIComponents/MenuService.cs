using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Translations;
using Bb.UserInterfaces;

namespace Bb.UIComponents
{



    [ExposeClass("Service", ExposedType = typeof(MenuService), LifeCycle = IocScopeEnum.Singleton)]
    public class MenuService
    {

        public MenuService(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
            this._menus = new Dictionary<string, ServerMenu>();
        }


        public ServerMenu GetMenu(string name)
        {
            if (_menus.TryGetValue(name, out var block))
                return block;
            return null;
        }


        public void Initialize(string name, Guid? guid, Action<ServerMenu> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            ServerMenu? menu = GetOrCreateMenu(name, guid, null);

            lock (_lock)
                action(menu);

        }


        public ServerMenu GetOrCreateMenu(string name, Guid? guid, TranslatedKeyLabel? label = null)
        {

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (!guid.HasValue)
                guid = Guid.Empty;

            if (!_menus.TryGetValue(name, out var block))
                lock (_lock)
                    if (!_menus.TryGetValue(name, out block))
                        _menus.Add(name, block = new ServerMenu(_serviceProvider)
                        {
                            Display = label,
                        });

            return block;

        }

        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, ServerMenu> _menus;
        private volatile object _lock = new object();

    }



}
