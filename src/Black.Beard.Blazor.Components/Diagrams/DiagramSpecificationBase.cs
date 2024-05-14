using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel;

namespace Bb.Diagrams
{


    public class DiagramSpecificationBase
    {

        public DiagramSpecificationBase(Guid uuid, TranslatedKeyLabel name, TranslatedKeyLabel description, string icon)
        {
            this.Uuid = uuid;
            this.Name = name;
            this.ToolTip = description;
            Icon = icon;
            Category = Bb.ComponentConstants.Tools;
        }

        public virtual string GetDefaultName()
        {
            return $"item";
        }

        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Uuid { get; set; }


        public virtual void SetTypeModel<T>()
            where T : CustomizedNodeModel
        {
            var type = typeof(T);
            TypeModel = type;
        }

        public Type TypeModel { get; private set; } = typeof(CustomizedNodeModel);

        public virtual void SetTypeUI<T>()
            where T : Microsoft.AspNetCore.Components.IComponent
        {
            TypeUI = typeof(T);
        }

        public Type TypeUI { get; private set; } 

        public TranslatedKeyLabel Category { get; protected set; }

        /// <summary>
        /// Name of the entity tool
        /// </summary>
        public TranslatedKeyLabel Name { get; }

        /// <summary>
        /// Description of the entity tool
        /// </summary>
        public TranslatedKeyLabel ToolTip { get; }

        /// <summary>
        /// Icon of the entity tool in the toolbox
        /// </summary>
        public string Icon { get; }

        public ToolKind Kind { get; protected set; }


    }

    public enum ToolKind
    {
        Node,
        Link,
        Group,

    }

}
