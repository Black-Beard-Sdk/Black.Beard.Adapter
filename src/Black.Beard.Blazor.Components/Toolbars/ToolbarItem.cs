using Bb.ComponentModel.Translations;

namespace Bb.Toolbars
{

    public partial class Tool
    {

        public Tool
        (
            TranslatedKeyLabel name,
            TranslatedKeyLabel description,
            string icon,
            object? tag, 
            bool withToggle,
            bool draggable,
            bool show
        )
        {
            this.Name = name;
            
            this.Icon = icon;
            this.ToolTip = description;
            Tag = tag;
            this.WithToggle = withToggle;
            this.Draggable = draggable ? "true" : "false";
            this.Show = show;
        }

        /// <summary>
        /// Name of the entity tool
        /// </summary>
        public TranslatedKeyLabel Name { get; }

        /// <summary>
        /// Description of the entity tool
        /// </summary>
        public TranslatedKeyLabel ToolTip { get; }

        /// <summary>
        /// Icon of the entity tool
        /// </summary>
        public string Icon { get; }

        public object? Tag { get; }

        public bool WithToggle { get; }

        public string Draggable { get; }

        public bool Show { get;  }

        public ToolbarGroup? Group { get; internal set; }

    }


}
