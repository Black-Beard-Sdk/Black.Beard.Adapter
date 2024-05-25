namespace Bb.Modules.Sgbd.Models
{
    public class ColumnType
    {

        public ColumnType(string label, string code, ColumbTypeCategory category)
        {
            Label = label;
            Code = code;
            this.Category = category;
        }

        public string Label { get; }
        
        public string Code { get; }

        public ColumbTypeCategory Category { get; }

    }


    public enum ColumbTypeCategory
    {
        String,
        Integer,
        Number,
        Date,
        Boolean,
        Binary,
        Other,
        UUid
    }


}
