using Bb.ComponentModel.Translations;

namespace Bb.Modules
{
    public static class DatasComponentConstants
    {

        public static TranslatedKeyLabel Column => new TranslatedKeyLabel("Column");
        public static TranslatedKeyLabel Columns => new TranslatedKeyLabel("Columns");
        public static TranslatedKeyLabel Types => new TranslatedKeyLabel("Types");
        public static TranslatedKeyLabel Nullable => new TranslatedKeyLabel("Nullable");
        public static TranslatedKeyLabel AddColumn => new TranslatedKeyLabel("Add Column");
        public static TranslatedKeyLabel DelColumn => new TranslatedKeyLabel("Remove Column");



        public static TranslatedKeyLabel Index => new TranslatedKeyLabel("Index");
        public static TranslatedKeyLabel Indexes => new TranslatedKeyLabel("Indexes");
        public static TranslatedKeyLabel AddIndex => new TranslatedKeyLabel("Add index");
        public static TranslatedKeyLabel DelIndex => new TranslatedKeyLabel("Remove index");


        public const string TableValidationMessage = "The {0} of the table is not valid.";
        public const string ColumnValidationMessage = "The {0} of the column is not valid.";
        public const string IndexValidationMessage = "The {0} of the index is not valid.";
        public const string ValueCantBeExceed = "{0} Can't be exceeds 128 characters.";


    }


}
