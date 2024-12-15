using Bb.ComponentDescriptors;
using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components;
using System.Collections;

namespace Bb.PropertyGrid
{

    public partial class PropertyListView
    {

        [Inject]
        public ITranslateService TranslateService { get; set; }

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }




        [Parameter]
        public IEnumerable SelectedObject
        {
            get => _selectedObject;
            set
            {

                if (_selectedObject != value)
                {
                    //if (_selectedObject is INotifyPropertyChanged old1)
                    //    old1.PropertyChanged -= PropertyChanged;

                    //if (_selectedObject is INotifyCollectionChanged old2)
                    //    old2.CollectionChanged -= CollectionChanged;
                }

                _selectedObject = value;
                //Update();

                //if (_selectedObject is INotifyPropertyChanged old3)
                //    old3.PropertyChanged += PropertyChanged;

                //if (_selectedObject is INotifyCollectionChanged old4)
                //    old4.CollectionChanged += CollectionChanged;

            }

        }

        public ObjectDescriptor Descriptor { get; private set; }

        private void Update()
        {

            if (!_mapperInitialized)
                lock (_lock)
                    if (!_mapperInitialized)
                    {
                        Initialize(StrategyMapper.Get(StrategyName));
                        _mapperInitialized = true;
                    }

            Descriptor = new ObjectDescriptor
            (
                _selectedObject,
                _selectedObject?.GetType(),
                TranslateService,
                ServiceProvider,
                StrategyName,
                null,
                PropertyFilter
            )
            {
                PropertyHasChanged = PropertyHasChanged_Impl,
            };

            this.Descriptor.PropertyHasChanged = this.SubPropertyHasChanged;

            try
            {
                StateHasChanged();
            }
            catch (Exception ex)
            {

            }

        }

        private static bool _mapperInitialized = false;
        private IEnumerable _selectedObject;
        private static object _lock = _mapperInitialized = false;


    }

}
