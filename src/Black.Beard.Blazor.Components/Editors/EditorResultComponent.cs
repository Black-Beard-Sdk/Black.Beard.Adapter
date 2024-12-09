namespace Bb.Editors
{

    public class EditorResultComponent
    {

        public EditorResultComponent()
        {

            Cancel = (c) => true;

        }

        public Func<ContextEditor, bool>? Cancel { get; set; }


        public Func<ContextEditor, bool>? Validate { get; set; }


        public Action<EditorResultComponent>? ToClose { get; set; }


        public ContextEditor? Result { get; internal set; }


    }

}
