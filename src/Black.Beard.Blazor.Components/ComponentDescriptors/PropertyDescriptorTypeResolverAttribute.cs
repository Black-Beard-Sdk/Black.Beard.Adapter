using Bb.PropertyGrid;
using Microsoft.AspNetCore.SignalR;
using static MudBlazor.Colors;

namespace Bb.ComponentDescriptors
{


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PropertyDescriptorTypeResolverAttribute<T> : Attribute, IPropertyDescriptorTypeResolver
        where T : class, IPropertyDescriptorTypeResolver, new()
    {

        // This is a positional argument
        public PropertyDescriptorTypeResolverAttribute()
        {

        }             

        public bool ResolveType(PropertyObjectDescriptor current, Type type, out Type typeResult)
        {
            var resolver = new T();
            return resolver.ResolveType(current, type, out typeResult);
        }

    }

    internal class ResolveTypeFromValue : IPropertyDescriptorTypeResolver
    {

        public bool ResolveType(PropertyObjectDescriptor current, Type type, out Type typeResult)
        {

            var i = current.Parent.Value as ComponentFieldListItem;
            typeResult = i.Descriptor.Type;

            return typeResult != null;

        }
    }


}