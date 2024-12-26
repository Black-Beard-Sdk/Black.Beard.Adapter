using System.Runtime.CompilerServices;

namespace Bb.ComponentDescriptors
{


    public interface ITransaction : IDisposable
    {

        public void Abort();

        public void Commit();

    }


    public sealed class MapperProvider
    {

        /// <summary>
        /// Get the mapper for the type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IMapper? GetMapper(Type typeSource, Type targetType)
        {

            var typeMapper = typeof(IMapper<,>).MakeGenericType(typeSource, targetType);

            var att = typeSource.GetCustomAttributes(typeof(MapperAttribute), false);
            if (att.Length > 0)
                foreach (MapperAttribute mapperAttribute in att)
                    if (!mapperAttribute.IsNotMapper && typeMapper.IsAssignableFrom(mapperAttribute.TypeMapper))
                        return (IMapper)Activator.CreateInstance(mapperAttribute.TypeMapper);

            att = targetType.GetCustomAttributes(typeof(MapperAttribute), false);
            if (att.Length > 0)
                foreach (MapperAttribute mapperAttribute in att)
                    if (!mapperAttribute.IsNotMapper && typeMapper.IsAssignableFrom(mapperAttribute.TypeMapper))
                        return (IMapper)Activator.CreateInstance(mapperAttribute.TypeMapper);

            return null;

        }

    }


    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class TypeDescriptorViewAttribute : Attribute
    {

        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string positionalString;

        // This is a positional argument
        public TypeDescriptorViewAttribute(Type typeView)
        {
            this.TypeView = typeView;
        }

        public static Type GetTypeView(Type type) => type.GetCustomAttributes(typeof(TypeDescriptorViewAttribute), false).Select(c => ((TypeDescriptorViewAttribute)c).TypeView).FirstOrDefault();

        public Type TypeView { get; }

    }


    /// <summary>
    /// Attribute to define a mapper
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    public sealed class MapperAttribute : Attribute
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MapperAttribute"/> class.
        /// </summary>
        /// <param name="typeMapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MapperAttribute(Type typeMapper)
        {

            if (typeMapper == null)
                throw new ArgumentNullException(nameof(typeMapper));

            this.TypeMapper = typeMapper;
            this.IsNotMapper = typeMapper.IsAssignableFrom(typeof(IMapper));
        }

        /// <summary>
        /// Method to map an object to another object
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="instanceSource"></param>
        /// <param name="instanceTarget"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryToMap<TSource, TTarget>(IMapper mapper, object instanceSource, ref object? instanceTarget)
            where TSource : class
            where TTarget : class
        {

            var source = instanceSource as TSource;

            if (source != null && mapper is IMapper<TSource, TTarget> m)
            {
                instanceTarget = m.MapTo(source, (TTarget)instanceTarget);
                return true;
            }

            return false;

        }

        /// <summary>
        /// Type of the mapper
        /// </summary>
        public Type? TypeMapper { get; }

        /// <summary>
        /// If true, the type is not a mapper
        /// </summary>
        public bool IsNotMapper { get; }

    }

    /// <summary>
    /// Global interface for mapping objects
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// Method to map an object to another object
        /// </summary>
        /// <param name="source">source object</param>
        /// <param name="targetType"></param>
        /// <param name="targetInstance">targetInstance for map property</param>
        /// <returns></returns>
        object MapTo(object source, Type targetType, object? targetInstance);

    }

    /// <summary>
    /// Specialized interface for mapping objects
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <typeparam name="TTarget">Target type</typeparam>
    public interface IMapper<TSource, TTarget> : IMapper
    {

        /// <summary>
        /// Method to map an object to another object
        /// </summary>
        /// <param name="source">source object</param>
        /// <param name="target">target object. if null it is used to map properties</param>
        /// <returns>target object. if target is not null the instance is used.</returns>
        TTarget MapTo(TSource source, TTarget? target);

    }

}
