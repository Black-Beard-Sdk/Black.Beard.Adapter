using Bb.ComponentDescriptors;
using Microsoft.AspNetCore.Components;

namespace Bb.Editors
{


    public class ContextEditor
    {

        public ComponentBase Component { get; internal set; }

        public object SelectedObject { get; internal set; }

        public object? ViewObject { get; internal set; }

        public IMapper? Mapper { get; internal set; }

        public bool Result { get; internal set; }

        public bool Canceled { get; internal set; }

    }

}
