namespace Bb.CustomComponents
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class StringMaskAttribute : Attribute
    {

        public StringMaskAttribute(StringType mask)
        {
            this.Mask = mask;
        }

        public StringType Mask { get; }
    }


}
