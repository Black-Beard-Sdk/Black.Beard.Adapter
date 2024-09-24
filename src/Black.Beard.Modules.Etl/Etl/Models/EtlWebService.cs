using Bb.Diagrams;
using Blazor.Diagrams.Core.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bb.Modules.Etl.Models
{

    public class EtlWebService : UIGroupModel
    {

        public EtlWebService(DiagramGroupNode source)
            : base(source)
        {

        }


        [Required]
        public string RootPath { get; set; }


        #region EtlWebMethodService

        public void AddMethod(EtlWebMethodService method)
        {
            if (!_methods.Contains(method))
            {
                _methods.Add(method);
                method.Service = this;
                //AddPort(column, column.Primary ? PortAlignment.Right : PortAlignment.Left);
                method.PropertyChanged += method_PropertyChanged;
            }
        }

        public void RemoveMethod(EtlWebMethodService method)
        {
            if (_methods.Contains(method))
            {
                _methods.Remove(method);
                method.Service = null;
                //RemovePort(column);
                method.PropertyChanged -= method_PropertyChanged;
            }
        }

        private void method_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(EtlWebMethodService.Primary))
            //{
            //    var column = (Column)sender!;
            //    var port = GetPort(column);
            //    if (port != null)
            //    {
            //        RemovePort(column);
            //        AddPort(CreatePort(column));
            //    }
            //}
        }

        [Browsable(false)]
        public List<EtlWebMethodService> Methods
        {
            get => _methods ?? (_methods = new List<EtlWebMethodService>());
            set
            {

                if (_methods == null)
                    _methods = new List<EtlWebMethodService>();

                foreach (EtlWebMethodService column in value)
                    AddMethod(column);

            }
        }

        #endregion Columns


        private List<EtlWebMethodService> _methods;

    }

   
}
