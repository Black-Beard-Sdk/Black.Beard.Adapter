using Bb.ComponentDescriptors;
using Bb.ComponentModel.Translations;
using Bb.PropertyGrid;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bb.Editors
{

    public partial class EditorComponent : ComponentBase, ITranslateHost, IDisposable
    {


        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }


        [Parameter]
        public EditorResultComponent Actions { get; set;}

        public string Title { get => MudDialog.Title; }


        [Inject]
        public ITranslateService TranslationService { get; set; }


        [Parameter]
        public object SelectedObject 
        { 
            get
            {
                return _selectedObject;
            }
            set
            {

                _selectedObject = value;
                _viewObject = value;

                if (_selectedObject != null)
                {
                    var type = _viewObject.GetType();
                    var typeView = TypeDescriptorViewAttribute.GetTypeView(type);
                    if (typeView != null)
                    {
                        var mapper = MapperProvider.GetMapper(type, typeView);
                        _viewObject = mapper?.MapTo(_selectedObject, typeView, null);
                        _mapperReturn = MapperProvider.GetMapper(typeView, type);
                    }

                }
            }
        }

        public object? ViewObject => _viewObject ?? _selectedObject;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {

                if (disposing)
                {

                }

                disposedValue = true;

            }
        }


        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        private void Cancel()
        {

            Actions.Result = new ContextEditor()
            {
                Component = this,
                SelectedObject = SelectedObject,
                ViewObject = ViewObject,
                Mapper = _mapperReturn,
                Canceled = true,
            };

            if (Actions.Result.Result = Actions.Cancel(Actions.Result))
            {
                MudDialog?.Cancel();
                Actions.ToClose(Actions);
            }

        }


        private void Validate()
        {
            Actions.Result = new ContextEditor()
            {
                Component = this,
                SelectedObject = SelectedObject,
                ViewObject = ViewObject,
                Mapper = _mapperReturn,
                Canceled = false,
            };

            if (Actions.Result.Result = Actions.Validate(Actions.Result))
            {
                MudDialog?.Close(DialogResult.Ok(true));
                Actions.ToClose(Actions);
            }

        }


        private bool disposedValue;
        private PropertyGridView CurrentPropertyGridView;
        private object _selectedObject;
        private object? _viewObject;
        private IMapper? _mapperReturn;
    }
}
