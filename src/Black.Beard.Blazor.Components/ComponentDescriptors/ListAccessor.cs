
namespace Bb.ComponentDescriptors
{


    public partial class ListAccessor
    {


        static ListAccessor()
        {
            _orders = new[]
            {
                typeof(IDictionary<,>),
                typeof(IList<>),
                typeof(ICollection<>),
                typeof(IEnumerable<>)
            };

        }

        public ListAccessor(Type type)
        {


            if (type.IsArray)
            {
                var tt = type.GetElementType();
                _instance = (MethodListBase)Activator.CreateInstance(typeof(MethodArray<>).MakeGenericType(tt));
            }

            if (_instance == null)
            {

                var types = type.GetInterfaces().ToList();
                var p = new List<Type>(types.Count);

                foreach (var itemType in _orders)
                {
                    var type1 = TryToMatch(itemType, types);
                    if (type1 != null)
                    {
                        this._instance = Evaluate(itemType, type1);
                        if (_instance != null)
                            break;
                    }
                }

            }


            if (_instance != null)
            {
                this.CanAdd = _instance.CanAdd;
                this.CanDel = _instance.CanDel;
                this.CanGet = _instance.CanGet;
                this.CanSet = _instance.CanSet;
            }
            else
            {

            }

        }

        private static Type TryToMatch(Type type, List<Type> types)
        {
            if (type.IsGenericType)
            {
                foreach (var t in types)
                {
                    if (t.IsConstructedGenericType)
                    {
                        if (type.IsAssignableFrom(t.GetGenericTypeDefinition()))
                            return t;

                    }
                }
            }
            else
            {
                foreach (var t in types)
                {

                }
            }

            return null;

        }

        private MethodListBase Evaluate(Type type, Type item)
        {

            if (item.IsConstructedGenericType)
            {

                var args = item.GetGenericArguments();
                if (args.Length == 1)
                {

                    if (typeof(IList<>) == type)
                        return (MethodListBase)Activator.CreateInstance(typeof(MethodList<>).MakeGenericType(args));

                    else if (typeof(ICollection<>) == type)
                        return (MethodListBase)Activator.CreateInstance(typeof(MethodCollection<>).MakeGenericType(args));

                }
                else if (args.Length == 2)
                {

                    if (typeof(IDictionary<,>) == type)
                        return (MethodListBase)Activator.CreateInstance(typeof(MethodDictionary<,>).MakeGenericType(args));

                }

            }

            return null;

        }

        public object GetKey(object instance, object key)
        {
            return _instance?.GetKey(instance, key);
        }


        public void Add(object instance, object key, object value)
        {
            _instance?.Add(instance, key, value);
        }

        public void Del(object instance, object key)
        {
            _instance?.Del(instance, key);
        }

        public object Get(object instance, object key)
        {
            return _instance?.Get(instance, key);
        }

        public void Set(object instance, object key, object value)
        {
            _instance?.Set(instance, key, value);
        }




        public bool CanAdd { get; private set; }

        public bool CanDel { get; private set; }

        public bool CanGet { get; private set; }

        public bool CanSet { get; private set; }

        public bool IsDictionary { get; }

        public bool IsList { get; }


        private MethodListBase? _instance;
        private static readonly Type[] _orders;
    }


}