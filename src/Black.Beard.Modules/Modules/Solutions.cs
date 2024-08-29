using Bb.Addons;
using Bb.ComponentModel.Attributes;
using Bb.Storage;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Bb.Modules
{


    [ExposeClass(UIConstants.Service, ExposedType = typeof(Solutions), LifeCycle = IocScopeEnum.Scoped)]
    public class Solutions
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ModuleReferential"></param>
        public Solutions(AddOnLibraries ModuleReferential,
            IStore<Guid, Solution> store)
        {
            this._referentiel = ModuleReferential;
            this._store = store;
        }

        /// <summary>
        /// Return a module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public Solution GetModule(Guid uuid)
        {
            Solution module = _store.Load(uuid);
            //var s = _referentiel.GetModule(module.Specification);
            //module.ModuleSpecification = s;
            return module;
        }


        /// <summary>
        /// Return a module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public ObservableCollection<Solution> GetModules()
        {

            Initialize();

            List<Solution> list = new List<Solution>();
            foreach (var item in _store.Index())
            {
                //var s = _referentiel.GetModule(item.Specification);
                //item.ModuleSpecification = s;
                list.Add(item);
            }

            var result = new ObservableCollection<Solution>(list);
            return result;
        }


        /// <summary>
        /// Create a new module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public Solution Create(string name, string description)
        {

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException(nameof(description));

            Initialize();

            var result = new Solution()
            {
                Uuid = Guid.NewGuid(),
                Label = name,
                Description = description,                
            };


            _store.Save(result);

            return result;

        }


        public void Save(Solution module)
        {
            _store.Save(module);
        }


        public void Remove(Solution module)
        {          
            module.Documents.RemoveAllFeatureOf(module);
            _store.RemoveKey(module.Uuid);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Initialize()
        {
            if (!_initialized)
                lock (_lock)
                    if (!_initialized)
                    {
                        _store.Initialize();
                        _initialized = true;
                    }
        }


        private AddOnLibraries _referentiel;
        private readonly IStore<Guid, Solution> _store;
        private volatile object _lock = new object();
        private bool _initialized;

    }



}
