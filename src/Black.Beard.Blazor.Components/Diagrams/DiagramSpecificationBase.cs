using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using Blazor.Diagrams.Core.Models.Base;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.Design;

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
