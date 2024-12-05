using Bb.ComponentModel.Translations;
using Blazor.Diagrams.Core.Models;

namespace Bb.Diagrams
{

    public class DiagramToolNode : DiagramToolBase
    {

        public DiagramToolNode
        (
            Guid uuid,
            TranslatedKeyLabel category,
            TranslatedKeyLabel name, 
            TranslatedKeyLabel description, 
            string icon)
            : base(uuid, category, name, description, icon)
        {
            this.InitializingPorts = new HashSet<PortAlignment>();
            Kind = ToolKind.Node;
            this._parentTypes = new HashSet<Type>();
        }

        /// <summary>
        /// Append new parent type. Restrict to drop this node in the diagram only in the parent type.
        /// </summary>
        /// <typeparam name="T">Type of parent</typeparam>
        public void AddParentType<T>()
            where T : NodeModel, INodeModel
        {
            this._parentTypes.Add(typeof(T));
        }

        public DiagramToolBase AddPort(params PortAlignment[] ports)
        {
            foreach (var port in ports)
                InitializingPorts.Add(port);
            return this;
        }

        public override void SetTypeModel<T>()
        {

            var type = typeof(T);
            var ctors = type.GetConstructors();

            foreach (var item in ctors)
            {
                var parameters = item.GetParameters();
                if (parameters.Length == 1)
                {
                    var parameterType = parameters[0].ParameterType;
                    if (typeof(SerializableDiagramNode).IsAssignableFrom(parameterType))
                        this.SourceType = parameterType;
                }
            }

            if (this.SourceType == null)
                throw new System.Exception($"The type {type.Name} must have a constructor with a single parameter of type DiagramNode");

            base.SetTypeModel<T>();

        }

        public virtual UIModel? CreateUI<T>(T model, Diagram diagram)
            where T : SerializableDiagramNode
        {
                     
            UIModel? result = (UIModel)Activator.CreateInstance(TypeModel, new object[] 
            { 
                model 
            });

            if (result != null)
            {
                model.SetUI(result);
                result.SetAvailableParents(_parentTypes, false);
                result.Source.SetDiagram(diagram);
                diagram.AddNode(result);
                result.InitializeFirst(this);
            }

            return result;
        }
            
        public virtual SerializableDiagramNode CreateModel(Diagram diagram, double x, double y, string name, Guid? uuid = null)
        {

            SerializableDiagramNode model = Create();            
            model.Uuid = uuid.HasValue ? uuid.Value : Guid.NewGuid();
            model.Label = name;
            model.Position = new Position(x, y);
            model.Initialize(this, true);
        
            CustomizeNode(model, diagram);

            return model;
        }

        protected virtual SerializableDiagramNode Create()
        {
            var model = (SerializableDiagramNode)Activator.CreateInstance(this.SourceType, new object[] { Uuid });
            //model.Type = Uuid;
            return model;
        }

        internal protected virtual void CustomizeNode(SerializableDiagramNode node, Diagram diagram)
        {

        }

        public HashSet<PortAlignment> InitializingPorts { get; }

        public bool IsGroup => typeof(SerializableDiagramGroupNode).IsAssignableFrom(this.SourceType);
        
        public byte Padding { get; internal set; }
        
        public bool ControlledSize { get; internal set; }
        
        public bool Locked { get; internal set; }
        
        public Type SourceType { get; private set; }


        private HashSet<Type> _parentTypes;

    }

}
