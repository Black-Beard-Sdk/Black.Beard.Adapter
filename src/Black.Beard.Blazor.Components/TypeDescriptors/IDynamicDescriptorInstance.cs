namespace Bb.TypeDescriptors
{

    public interface IDynamicDescriptorInstance
    {

        object? GetProperty(string name);

        void SetProperty(string name, object? value);

    }

}
