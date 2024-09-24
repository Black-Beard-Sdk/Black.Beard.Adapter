using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;


namespace Bb.Diagrams
{

    public partial class Toolbox : ComponentBase, ITranslateHost
    {


        [Parameter]
        public ToolboxList Tools { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }


        public DiagramToolBase CurrentDragStarted { get; private set; }

        public async Task DragStart(DragEventArgs args, DiagramToolBase item)
        {
            this.CurrentDragStarted = item;
        }
        
    }


}
