using Bb.UIComponents;

namespace Bb.Modules.Sgbd.Models
{

    public class SgbdTechnology
    {

        protected SgbdTechnology(string name, string description)
        {
            this.Name = name;
            this.Description = description;
            _columnTypes = new List<ColumnType>();
        }

        protected ColumnType AddColumnType(string label, string code)
        {
            var type = new ColumnType(label, code);
            _columnTypes.Add(type);
            return type;
        }


        public ColumnType DefaultColumnType { get; private set; }

        protected ColumnType IsDefaultColumnType(ColumnType columnType)
        {
            DefaultColumnType = columnType;
            return columnType;
        }

        public IEnumerable<ColumnType> ColumnTypes => _columnTypes.ToArray();

        public string Name { get; }

        public string Description { get; }

        public SgbdTechnologies Parent { get; internal set; }



        private List<ColumnType> _columnTypes;

    }

}
