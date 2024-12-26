namespace Bb.Commands
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class RestoreIgnoreAttribute : Attribute
    {

        public RestoreIgnoreAttribute()
        {

        }

    }

}
