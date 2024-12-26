namespace Bb.Commands
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Struct | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class RestoreAttribute : Attribute
    {

        public RestoreAttribute(Type type)
        {
            this.Type = type;
        }


        public bool TypeCanRestore => typeof(IRestorableMapper).IsAssignableFrom(Type);

        public IRestorableMapper Get()
        {
            return (IRestorableMapper)Activator.CreateInstance(Type);
        }

        public Type Type { get; }

    }

}
