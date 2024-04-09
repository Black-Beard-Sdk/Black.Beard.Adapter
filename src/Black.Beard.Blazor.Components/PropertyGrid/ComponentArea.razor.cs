using Microsoft.AspNetCore.Components;

namespace Bb.PropertyGrid
{

    public partial class ComponentArea
    {


        [Parameter]
        public EventCallback<string> ChildDataChanged { get; set; }


        private async Task HandleOnChange(ChangeEventArgs args)
        {
            var data = args.Value.ToString();
            Validate(data);
            await ChildDataChanged.InvokeAsync(data);
        }


    }

}
