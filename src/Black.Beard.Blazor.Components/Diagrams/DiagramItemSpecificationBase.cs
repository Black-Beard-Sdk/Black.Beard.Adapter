using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.Design;

namespace Bb.Diagrams
{

    public class DiagramItemSpecificationBase
    {

        public DiagramItemSpecificationBase(Guid uuid, TranslatedKeyLabel name, TranslatedKeyLabel description, string icon)
        {
            this.Uuid = uuid;
            this.Name = name;
            this.ToolTip = description;
            Icon = icon;

        }

        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Uuid { get; set; }

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

    }


}
