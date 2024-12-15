

using MudBlazor;

namespace Bb.Editors
{

    public class Editor
    {

        public Editor()
        {

            Cancel = (c) => true;

            this.Options = new DialogOptions()
            {
                MaxWidth = MaxWidth.Medium,
                FullWidth = true,
                CloseOnEscapeKey = false,
                NoHeader = false,
                Position = DialogPosition.Center,
            };

        }

        public Func<ContextEditor, bool>? Cancel { get; set; }
        public DialogOptions Options { get; }

        public Func<ContextEditor, bool>? Validate { get; set; }


        public Action<Editor>? ToClose { get; set; }


        public ContextEditor? Result { get; internal set; }

        public bool WithList { get; set; }

        public DialogParameters Parameters(object target)
        {
            var b = new DialogParameters 
            { 
                { "SelectedObject", target }, 
                { "Actions", this }            
            };
            return b;
        }


        public async void ShowAsync(IDialogService dialogService, string title, object target)
        {
            var parameters = Parameters(target);
            var r = dialogService.ShowAsync<EditorComponent>(title, parameters, Options);
        }


    }

}
