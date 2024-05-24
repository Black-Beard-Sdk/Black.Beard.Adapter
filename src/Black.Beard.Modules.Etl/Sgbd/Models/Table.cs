using Bb.Diagrams;
using Bb.TypeDescriptors;
using Blazor.Diagrams.Core.Models;
using ICSharpCode.Decompiler.CSharp.Transforms;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace Bb.Modules.Sgbd.Models
{

    public class Table : CustomizedNodeModel, INotifyPropertyChanged
    {

        public Table(DiagramNode source)
            : base(source)
        {

        }



        private void Index_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(Column.Primary))
            //{
            //    var column = (Index)sender!;
            //    var port = GetPort(column);
            //    if (port != null)
            //    {
            //        RemovePort(column);
            //        AddPort(CreatePort(column));
            //    }
            //}
        }

        public void AddIndex(Index index)
        {
            if (!_indexes.Contains(index))
            {
                _indexes.Add(index);
                index.Table = this;
                index.PropertyChanged += Index_PropertyChanged;
            }
        }

        public void RemoveIndex(Index index)
        {
            if (_indexes.Contains(index))
            {
                _indexes.Remove(index);
                index.Table = null;
                index.PropertyChanged -= Index_PropertyChanged;
            }
        }

        [Browsable(false)]
        public List<Index> Indexes
        {
            get => _indexes ?? (_indexes = new List<Index>());
            set
            {

                if (_indexes == null)
                    _indexes = new List<Index>();

                foreach (Index index in value)
                    AddIndex(index);

            }
        }



        [Browsable(false)]
        public bool HasPrimaryColumn => Columns.Any(c => c.Primary);



        [Required]
        [MaxLength(128, ErrorMessage = DatasComponentConstants.ValueCantBeExceed)]
        [RegularExpression(SqlserverConstants.NameConstraint, ErrorMessage = DatasComponentConstants.TableValidationMessage) ]
        public string Name
        {
            get => this.Title;
            set
            {
                if (this.Title != value)
                {
                    this.Title = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }


        #region Columns

        public void AddColumn(Column column)
        {
            if (!_columns.Contains(column))
            {
                _columns.Add(column);
                column.Table = this;
                AddPort(column, column.Primary ? PortAlignment.Right : PortAlignment.Left);
                column.PropertyChanged += Column_PropertyChanged;
            }
        }

        public void RemoveColumn(Column column)
        {
            if (_columns.Contains(column))
            {
                _columns.Remove(column);
                column.Table = null;
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

        [Browsable(false)]
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

        #endregion Columns


        #region Links

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

        #endregion Links


        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler? PropertyChanged;


        private List<Column> _columns;
        private List<Index> _indexes;
        private Index _primaryKey;

    }
}
