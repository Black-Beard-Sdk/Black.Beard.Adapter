using Bb.ComponentModel.Loaders;
using Bb.ComponentModel.Translations;
using Blazor.Diagrams.Core.Models;
using System.Net.Sockets;

namespace Bb.Diagrams
{

    public class DiagramToolNode : DiagramToolBase
    {

        public DiagramToolNode(
            Guid uuid,
            TranslatedKeyLabel category,
            TranslatedKeyLabel name, 
            TranslatedKeyLabel description, 
            string icon)
            : base(uuid, category, name, description, icon)
        {
            this.Ports = new HashSet<PortAlignment>();
            Kind = ToolKind.Node;
            this._parentTypes = new HashSet<Type>();
        }

        public override string GetDefaultName()
        {
            return $"Node";
        }

        /// <summary>
        /// Append new parent type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddParentType<T>()
            where T : NodeModel, INodeModel
        {

            this._parentTypes.Add(typeof(T));

        }

        public DiagramToolBase AddPort(params PortAlignment[] ports)
        {
            foreach (var port in ports)
                Ports.Add(port);
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
                    if (typeof(IDiagramNode).IsAssignableFrom(parameterType))
                        this.SourceType = parameterType;
                }
            }

            if (this.SourceType == null)
                throw new System.Exception($"The type {type.Name} must have a constructor with a single parameter of type DiagramNode");

            base.SetTypeModel<T>();

        }



        internal protected virtual void CustomizeNode(IDiagramNode node, Diagram diagram)
        {

        }

        public virtual UIModel? CreateUI<T>(T model, Diagram diagram)
            where T : IDiagramNode
        {

            CustomizeNode(model, diagram);

            UIModel? result = (UIModel)Activator.CreateInstance(TypeModel, new object[] { model });

            if (result != null)
            {
                result.SetAvailableParents(_parentTypes, false);
                result.Source.SetDiagram(diagram);
                diagram.AddNode(result);
                result.InitializeFirst(this);
            }

            return result;
        }

        public virtual SerializableDiagramNode CreateModel(double x, double y, string name, Guid? uuid = null)
        {
            SerializableDiagramNode model = Create();
            model.Name = name;
            model.Uuid = uuid.HasValue ? uuid.Value : Guid.NewGuid();
            model.Type = Uuid;
            model.Position = new Position(x, y);
            model.InitializeFirst(this);
            return model;
        }

        protected virtual SerializableDiagramNode Create()
        {
            return (SerializableDiagramNode)Activator.CreateInstance(this.SourceType, new object[] { });
        }


        public HashSet<PortAlignment> Ports { get; }
        public bool IsGroup => this.SourceType == typeof(DiagramGroupNode);
        public byte Padding { get; internal set; }
        public bool ControlledSize { get; internal set; }
        public bool Locked { get; internal set; }
        public Type SourceType { get; private set; }


        private HashSet<Type> _parentTypes;

    }

    public static class DiagramToolNodeExtension
    {

        public static T IsControlled<T>(this T self, bool value)
            where T : DiagramToolNode
        {
            self.ControlledSize = value;
            return self;
        }

        public static T SetPadding<T>(this T self, byte value)
            where T : DiagramToolNode
        {
            self.Padding = value;
            return self;
        }

        public static T IsLocked<T>(this T self, bool value)
            where T : DiagramToolNode
        {
            self.Locked = value;
            return self;
        }

    }

}
