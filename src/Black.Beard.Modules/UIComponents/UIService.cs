using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Translations;
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


        public UIComponentMenu GetMenuOrCreate(string name, Guid? guid, TranslatedKeyLabel? label = null)
        {

            if (!guid.HasValue)
                guid = Guid.Empty;

            if (!_menus.TryGetValue(name, out var listBlock))
                _menus.Add(name, listBlock = new List<UIComponent>());

            UIComponent? block = listBlock.FirstOrDefault(c => c.Uuid == guid.Value);

            if (block == null)
            {
                listBlock.Add(block = new UIComponentMenu(guid)
                {
                    Service = this,
                    Display = label,
                    Type = UIComponent.Types.Menu,
                });
            }

            return (UIComponentMenu)block;

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

        public static string LeftMenu { get; } = "LeftMenu";
        public static string TopLeftMenu { get; } = "TopLeftMenu";
        public static string TopRightMenu { get; } = "TopRightMenu";


        private readonly Dictionary<string, List<UIComponent>> _menus;
        private volatile object _lock = new object();
        private List<(RouteAttribute, Type)> _types;

        public static class Guids
        {

            public static Guid Configurations { get; } = new Guid("{0F368852-6097-4FD7-B060-26367A4852B1}");
            public static Guid Translations { get; } = new Guid("{46839164-66C6-4C04-BC63-387132DD961C}");

            public static Guid Home { get; } = new Guid("{2E1273B0-35F1-43AD-9E24-572B73BAAB90}");
            public static Guid Languages { get; } = new Guid("{F1349EB1-BCF7-40AD-B2DF-59025F3D0493}");

            public static Guid MenuUser { get; } = new Guid("{B87BF935-BC5A-4349-B5EA-54750E88735B}");
            public static Guid Login { get; } = new Guid("{5132F0F5-CD09-4050-BFE8-473939C1B7ED}");
            public static Guid Register { get; } = new Guid("{60DA5000-12E6-40F7-8471-6F8126BC782E}");


        }

    }


}
