using Bb.Diagrams;
using Bb.TypeDescriptors;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bb.Modules.Etl.Models
{

    public class EtlWebMethodService : UIModel
    {

        public EtlWebMethodService(SerializableDiagramNode source)
            : base(source)
        {
            //this._container = new DynamicDescriptorInstanceContainer(this);
        }

        [JsonIgnore]
        [Browsable(false)]
        public EtlWebService Service { get; set; }

        [Required]
        public WebMethodService Method { get; set; }

        [Required]
        public string Path { get; set; }


        #region INotifyPropertyChanged

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion INotifyPropertyChanged


        //#region IDynamicDescriptorInstance

        //public object GetProperty(string name) => this._container.GetProperty(name);

        //public void SetProperty(string name, object value) => this._container.SetProperty(name, value);

        //#endregion IDynamicDescriptorInstance


        //private readonly DynamicDescriptorInstanceContainer _container;

    }

    public enum WebMethodService
    {
        Get,
        Post,
        Put,
        Delete,
    }

}
