using Bb.ComponentModel.Translations;

namespace Bb
{
    
    public static class ModuleConstants
    {

        public static TranslatedKeyLabel Description => new TranslatedKeyLabel("Description");

        public static TranslatedKeyLabel AddNewModuleSetName => new TranslatedKeyLabel("Specify the name and the description for the new module");
        public static TranslatedKeyLabel AddNewFeatureSetName => new TranslatedKeyLabel("Specify the name and the description for the new feature");

        public static TranslatedKeyLabel AddNewModuleSetType => new TranslatedKeyLabel("Specify the type of the new module");
        public static TranslatedKeyLabel AddNewFeatureSetType => new TranslatedKeyLabel("Specify the type of the new feature");

        public static TranslatedKeyLabel Type => new TranslatedKeyLabel("Type");

        public static TranslatedKeyLabel Filter => new TranslatedKeyLabel("Filter");

        public static TranslatedKeyLabel Modules => new TranslatedKeyLabel("Modules");

        public static TranslatedKeyLabel Manage => new TranslatedKeyLabel("Manage");

        public static TranslatedKeyLabel AddANewModule => new TranslatedKeyLabel("Add a new module");
        public static TranslatedKeyLabel AddANewFeature => new TranslatedKeyLabel("Add a new feature");

        public static TranslatedKeyLabel Cancel => new TranslatedKeyLabel("Cancel");

        public static TranslatedKeyLabel Delete => new TranslatedKeyLabel("Delete");

        public static TranslatedKeyLabel DoYouWantToDelete => new TranslatedKeyLabel("Do you want to delete '{0}' ?");

        public static TranslatedKeyLabel DoYouWantToDeleteItem => new TranslatedKeyLabel("Do you want to delete the {0} '{1}' ?");

        public const string Extension = ".json";


    }

}
