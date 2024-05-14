namespace Bb.Modules.Sgbd.Models
{
    public class ColumnType
    {

        public ColumnType(string label, string code)
        {
            Label = label;
            Code = code;
        }

        public string Label { get; }
        
        public string Code { get; }

        //Boolean,
        //Char,
        //String,
        //SByte,
        //Short,
        //Integer,
        //Long
    }

}
