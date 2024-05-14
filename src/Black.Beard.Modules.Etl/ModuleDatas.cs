using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Translations;

namespace Bb.Modules
{

    [ExposeClass(ComponentModel.ConstantsCore.Plugin, ExposedType = typeof(ModuleSpecification), LifeCycle = IocScopeEnum.Singleton)]
    public class ModuleDatas : ModuleSpecification
    {

        public ModuleDatas() :
            base(
                new Guid(Filter),
                "Datas managements",
                "Datas module")
        {

        }

        public const string Filter = "C9119B69-5DD9-45D2-A28A-617D6CB9D7F9";

    }

    public static class DatasComponentConstants
    {

        public static TranslatedKeyLabel Column => new TranslatedKeyLabel("Column");
        public static TranslatedKeyLabel Columns => new TranslatedKeyLabel("Columns");
        public static TranslatedKeyLabel Types => new TranslatedKeyLabel("Types");
        public static TranslatedKeyLabel Nullable => new TranslatedKeyLabel("Nullable");
        public static TranslatedKeyLabel AddColumn => new TranslatedKeyLabel("Add Column");
        public static TranslatedKeyLabel DelColumn => new TranslatedKeyLabel("Remove Column");

    }


}
