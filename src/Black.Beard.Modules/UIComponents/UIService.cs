using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Translations;
using Bb.UserInterfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Bb.UIComponents
{


    [ExposeClass("Service", ExposedType = typeof(UIService), LifeCycle = IocScopeEnum.Singleton)]
    public class UIService
    {

        public UIService()
        {
            _menus = new Dictionary<string, List<UIComponent>>();
        }


        public Task<List<UIComponent>> GetUI(string name)
        {

            if (_menus.TryGetValue(name, out var block))
                return Task.FromResult(block);

            return Task.FromResult(new List<UIComponent>());

        }

        public ActionReference GetAction(NavLinkMatch match, string route)
        {

            var action = GetRoutes().Where(c => c.Item1.Template == route).First();

            var resultAction = new ActionReference()
            {
                Match = match,
                HRef = route,
            };

            return resultAction;


        }
        
        public ActionReference GetAction(NavLinkMatch match, Type type)
        {

            var action = GetRoutes().Where(c => c.Item2 == type).First();

            var resultAction = new ActionReference()
            {
                Match = match,
                HRef = action.Item1.Template,
            };

            return resultAction;

        }

        public void GetAction()
        {

            var actions = GetRoutes();

            foreach (var item in actions)
            {

                var route = item.Item1;



            }

        }

        private List<(RouteAttribute, Type)> GetRoutes()
        {

            if (_types == null)
                lock (_lock)
                    if (_types == null)
                    {

                        var types = new List<(RouteAttribute, Type)>();

                        var ii = System.AppDomain.CurrentDomain.GetAssemblies().ToList();

                        var items = ii.Where(c =>
                        {
                            var p = c.GetReferencedAssemblies();
                            foreach (var item in p)
                                if (item.Name == "Microsoft.AspNetCore.Components")
                                    return true;
                            return false;
                        }).ToList();


                        foreach (var item in ii)
                            foreach (var type in item.ExportedTypes)
                            {
                                var i = (RouteAttribute)type.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault();
                                if (i != null)
                                    types.Add((i, type));
                            }

                        _types = types;

                    }

            return _types;

        }


        public UIComponent? Resolve(Guid id, UIComponent? value = null)
        {

            if (value != null)
            {

                if (value.Uuid == id)
                    return value;

                foreach (var item in value.Children)
                {
                    var block = Resolve(id, item);
                    if (block != null)
                        return block;
                }
            }

            else
            {

                foreach (var item in _menus)
                    foreach (var item2 in item.Value)
                    {
                        var block = Resolve(id, item2);
                        if (block != null)
                            return block;
                    }

            }

            return null;

        }


        public UIComponentMenu GetOrCreateMenu(string name, Guid? guid, TranslatedKeyLabel? label = null)
        {


            UIComponent? block;

            if (!guid.HasValue)
                guid = Guid.Empty;


            if (!_menus.TryGetValue(name, out var listBlock))
                lock (_lock)
                    if (!_menus.TryGetValue(name, out listBlock))
                        _menus.Add(name, listBlock = new List<UIComponent>());


            if ((block = listBlock.FirstOrDefault(c => c.Uuid == guid.Value)) == null)
                lock (_lock)
                    if ((block = listBlock.FirstOrDefault(c => c.Uuid == guid.Value)) == null)
                        listBlock.Add(block = new UIComponentMenu(guid)
                        {
                            Service = this,
                            Display = label,
                            Type = UITypes.Menu,
                        });


            return (UIComponentMenu)block;


        }


        public void Initialize(string name, Guid? guid, Action<UIComponentMenu> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            UIComponentMenu? menu = GetOrCreateMenu(name, guid, null);

            lock (_lock)
                action(menu);

        }


        public UIComponentMenu? GetMenu(string name, Guid? guid)
        {

            if (!guid.HasValue)
                guid = Guid.Empty;

            if (!_menus.TryGetValue(name, out var listBlock))
                _menus.Add(name, listBlock = new List<UIComponent>());

            UIComponent? block = listBlock.FirstOrDefault(c => c.Uuid == guid.Value);

            if (block is UIComponentMenu menu)
                return menu;

            return null;

        }

        private readonly Dictionary<string, List<UIComponent>> _menus;
        private volatile object _lock = new object();
        private List<(RouteAttribute, Type)> _types;

    }



}
