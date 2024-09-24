using Bb.ComponentModel.Translations;
using Bb.Diagrams;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bb.Toolbars
{
    public partial class ToolItemButton : ComponentBase, ITranslateHost
    {

        [Parameter]
        public Tool Item { get; set; }

        [Parameter]
        public ToolBar Panel { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }

    }
}
