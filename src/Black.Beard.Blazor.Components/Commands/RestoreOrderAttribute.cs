namespace Bb.Commands
{

    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class RestoreOrderAttribute : Attribute
    {

        // This is a positional argument
        public RestoreOrderAttribute(int order)
        {
            this.Order = order;
        }

        public int Order { get; }
        
    }


}
