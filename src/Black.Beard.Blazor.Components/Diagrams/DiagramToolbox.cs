using Bb.ComponentModel.Translations;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Bb.Diagrams
{

    public class DiagramToolbox : IEnumerable<DiagramToolBase>
    {



        #region ctor

        /// <summary>
        /// Initialize a new toolbox
        /// </summary>
        /// <param name="dynamicToolbox"></param>
        /// <param name="count"></param>
        internal DiagramToolbox(int count = 10)
        {
            _tools = new Dictionary<Guid, DiagramToolBase>(count);
            _providers = new List<IDiagramToolBoxProvider>();
        }

        #endregion ctor


        #region Add tools

        /// <summary>
        /// Add a tool to the toolbox
        /// </summary>
        /// <param name="uuid">unique id of the tool</param>
        /// <param name="category">category of the tool </param>
        /// <param name="name">Name of the tool</param>
        /// <param name="description">Display tooltip description</param>
        /// <param name="icon">Icon svg in the toolbox</param>
        /// <param name="initializer">optional initializer</param>
        /// <returns></returns>
        public DiagramToolbox AddNode(Guid uuid, TranslatedKeyLabel category,
            TranslatedKeyLabel name,
            TranslatedKeyLabel description,
            string icon, Action<DiagramToolNode>? initializer = null)
        {
            return Add(new DiagramToolNode(uuid, category, name, description, icon), initializer);
        }

        /// <summary>
        /// Add a tool to the toolbox
        /// </summary>
        /// <param name="uuid">unique id of the tool</param>
        /// <param name="category">category of the tool </param>
        /// <param name="name">Name of the tool</param>
        /// <param name="description">Display tooltip description</param>
        /// <param name="icon">Icon svg in the toolbox</param>
        /// <param name="initializer">optional initializer</param>
        /// <returns></returns>
        public DiagramToolbox AddLink(Guid uuid, TranslatedKeyLabel category,
            TranslatedKeyLabel name,
            TranslatedKeyLabel description,
            string icon, Action<DiagramToolRelationshipBase>? initializer = null)
        {
            return Add(new DiagramToolRelationshipBase(uuid, category, name, description, icon), initializer);
        }

        /// <summary>
        /// Add a tool to the toolbox
        /// </summary>
        /// <typeparam name="T">type of the tool</typeparam>
        /// <param name="instance">instance to add</param>
        /// <param name="initializer">optional initializer</param>
        /// <returns></returns>
        public DiagramToolbox Add<T>(T instance, Action<T>? initializer = null)
            where T : DiagramToolBase
        {

            if (instance != null && !_tools.ContainsKey(instance.Uuid))
            {

                if (initializer != null)
                    initializer.Invoke(instance);

                _tools.Add(instance.Uuid, instance);

            }

            return this;

        }

        #endregion Add tools


        /// <summary>
        /// Return the tool enumerator for iteration
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DiagramToolBase> GetEnumerator()
        {
            Refresh();
            return _tools.Values.GetEnumerator();
        }

        /// <summary>
        /// Return the tool enumerator for iteration
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            Refresh();
            return _tools.Values.GetEnumerator();
        }


        public DiagramToolbox AppendInitializer(IDiagramToolBoxProvider initializer)
        {
            if (initializer != null)
            {
                if (initializer.DynamicToolbox)
                {
                    _dynamicToolbox = true;
                    _providers?.Add(initializer);
                }
                else
                    initializer.InitializeToolbox(this);
            }
            return this;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Refresh()
        {
            if (_dynamicToolbox)
                foreach (var item in _providers)
                    item.InitializeToolbox(this);
        }

        public bool TryGetNodeTool(Guid type, out DiagramToolNode? specModel)
        {

            Refresh();

            specModel = null;
            
            if (_tools.TryGetValue(type, out var result))
                if (result is DiagramToolNode node)
                {
                    specModel = node;
                    return true;

                }

            return false;
        }

        public bool TryGetLinkTool(Guid type, out DiagramToolRelationshipBase? specModel)
        {

            Refresh();

            specModel = null;

            if (_tools.TryGetValue(type, out var result))
                if (result is DiagramToolRelationshipBase link)
                {
                    specModel = link;
                    return true;

                }

            return false;
        }

        /// <summary>
        /// Return true if the toolbox is dynamic
        /// </summary>
        public bool DynamicToolbox => _dynamicToolbox;


        private readonly Dictionary<Guid, DiagramToolBase> _tools;
        private readonly List<IDiagramToolBoxProvider> _providers;
        private bool _dynamicToolbox;

    }

}
