using Bb.ComponentModel.Translations;

namespace Bb.ComponentDescriptors
{

    public interface IComponentFieldBase
    {

        ITranslateService TranslateService { get; set; }

        Descriptor Descriptor { get; set; }

        PropertyObjectDescriptor? Property { get; }

        string StrategyName { get; }

    }

    public interface IComponentFieldBase<T> : IComponentFieldBase
    {

    }

}