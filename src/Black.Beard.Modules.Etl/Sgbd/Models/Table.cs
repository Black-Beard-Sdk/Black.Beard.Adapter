using Bb.Diagrams;
using Blazor.Diagrams.Core.Models;
using ICSharpCode.Decompiler.CSharp.Transforms;
using System.ComponentModel;


namespace Bb.Modules.Sgbd.Models
{

    public class Table : CustomizedNodeModel, INotifyPropertyChanged
    {

        public Table(DiagramNode source)
            : base(source)
        {

        }

        public void AddColumn(Column column)
        {
            if (!_columns.Contains(column))
            {
                _columns.Add(column);
                AddPort(column, column.Primary ? PortAlignment.Right : PortAlignment.Left);
                column.PropertyChanged += Column_PropertyChanged;
            }
        }

        public void RemoveColumn(Column column)
        {
            if (_columns.Contains(column))
            {
                _columns.Remove(column);
                RemovePort(column);
                column.PropertyChanged -= Column_PropertyChanged;
            }
        }

        private void Column_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Column.Primary))
            {
                var column = (Column)sender!;
                var port = GetPort(column);
                if (port != null)
                {
                    RemovePort(column);
                    AddPort(CreatePort(column));
                }
            }
        }


        public List<Column> Columns
        {
            get => _columns ?? (_columns = new List<Column>());
            set
            {
                
                if (_columns == null)
                    _columns = new List<Column>();

                foreach (Column column in value)
                    AddColumn(column);

            }
        }

        public bool HasPrimaryColumn => Columns.Any(c => c.Primary);


        public ColumnPort GetPort(Column column) => Ports.Cast<ColumnPort>().FirstOrDefault(p => p.Column == column);

        public void AddPort(Column column, PortAlignment alignment)
        {
            var id = column.Id.ToString();
            var p = Ports.Cast<ColumnPort>().FirstOrDefault(c => c.Id == id);
            if (p == null)
            {
                AddPort(CreatePort(column));
            }
            else
                p.Column = column;
        }


        public override PortModel CreatePort(Port port)
        {
            return new ColumnPort(this, port.Uuid.ToString(), null, port.Alignment);;
        }

        public PortModel CreatePort(Column column)
        {
            return new ColumnPort(this, column.Id.ToString(), column, column.Primary ? PortAlignment.Left : PortAlignment.Right);
        }

        public void RemovePort(Column column)
        {
            var p = GetPort(column);
            if (p != null)
                base.RemovePort(p);
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler? PropertyChanged;


        private List<Column> _columns;

    }
}
