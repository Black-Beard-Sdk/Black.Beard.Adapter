using Bb.ComponentModel.Translations;

namespace Bb
{
    
    public static class ModuleConstants
    {

        public static TranslatedKeyLabel Description => new TranslatedKeyLabel("Description");


        public static TranslatedKeyLabel AddNewSolutionSetName => new TranslatedKeyLabel("Specify the name and the description for the new solution");
        public static TranslatedKeyLabel AddNewDocumentSetName => new TranslatedKeyLabel("Specify the name and the description for the new document");


        public static TranslatedKeyLabel AddNewSolutionSetType => new TranslatedKeyLabel("Specify the type of the new module");
        public static TranslatedKeyLabel AddNewDocumentSetType => new TranslatedKeyLabel("Specify the type of the new feature");


        public static TranslatedKeyLabel Type => new TranslatedKeyLabel("Type");

        public static TranslatedKeyLabel Filter => new TranslatedKeyLabel("Filter");

        public static TranslatedKeyLabel Solutions => new TranslatedKeyLabel("Solutions");

        public static TranslatedKeyLabel Manage => new TranslatedKeyLabel("Manage");

        public static TranslatedKeyLabel AddANewSolution => new TranslatedKeyLabel("Add a new solution");
        public static TranslatedKeyLabel AddANewDocument => new TranslatedKeyLabel("Add a new Document");
        public static TranslatedKeyLabel ManageGit => new TranslatedKeyLabel("Manage git");

        public static TranslatedKeyLabel Cancel => new TranslatedKeyLabel("Cancel");

        public static TranslatedKeyLabel Delete => new TranslatedKeyLabel("Delete");

        public static TranslatedKeyLabel DoYouWantToDelete => new TranslatedKeyLabel("Do you want to delete '{0}' ?");

        public static TranslatedKeyLabel DoYouWantToDeleteItem => new TranslatedKeyLabel("Do you want to delete the {0} '{1}' ?");

        public const string Extension = ".json";


    }

}
