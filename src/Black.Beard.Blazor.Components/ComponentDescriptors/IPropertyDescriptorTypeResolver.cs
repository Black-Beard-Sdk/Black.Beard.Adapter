namespace Bb.ComponentDescriptors
{


    /// <summary>
    /// Represents a type resolver for property descriptors.
    /// </summary>
    public interface IPropertyDescriptorTypeResolver
    {


        /// <summary>
        /// Resolves the type of the descriptor.
        /// </summary>
        /// <remarks>
        /// This method is used to resolve the type of the descriptor based on the provided type.
        /// </remarks>
        /// <param name="descriptor">The descriptor to resolve.</param>
        /// <param name="type">The type to resolve.</param>
        /// <returns><c>true</c> if the type is resolved successfully; otherwise, <c>false</c>.</returns>
        bool ResolveType(PropertyObjectDescriptor current, Type type, out Type typeResult);



    }


}