namespace Bb.ComponentDescriptors
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class ValueLabelAttribute : Attribute
    {

        public ValueLabelAttribute(string label)
        {
            this.Label = label;
        }

        public string Label { get; }
    }


}