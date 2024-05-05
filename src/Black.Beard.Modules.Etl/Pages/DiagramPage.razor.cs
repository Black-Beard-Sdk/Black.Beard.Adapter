using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bb.Modules.Etl.Pages
{
    public partial class DiagramPage : ComponentBase
    {


        [Parameter]
        public Guid Uuid { get; set; }

    }
}
