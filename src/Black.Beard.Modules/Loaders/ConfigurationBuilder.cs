using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;


namespace Bb.Loaders
{
    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<IConfigurationBuilder>))]
    public class ConfigurationBuilder : InjectBuilder<IConfigurationBuilder>
    {

        public override object Execute(IConfigurationBuilder service)
        {

            return 0;

        }

    }


}