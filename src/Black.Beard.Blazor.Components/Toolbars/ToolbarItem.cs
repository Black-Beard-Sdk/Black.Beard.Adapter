﻿using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components.Web;

namespace Bb.Toolbars
{

    public partial class ExtendedTool : Tool
    {

        public ExtendedTool(TranslatedKeyLabel name, TranslatedKeyLabel description, string icon, object? tag, bool show, Action<ExtendedTool, object> command)
            : base(name, description, icon, tag, false, false, show)
        {
            this._command = command;
        }

        public void Execute(MouseEventArgs args, object target)
        {
            _command.Invoke(this, target);
        }

        private Action<ExtendedTool, object> _command;

    }

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

        public bool Show { get; }

        public ToolbarGroup? Group { get; internal set; }

    }


}
