namespace Bb.Diagrams
{
    public class SelectionChangedEventArgs : EventArgs
    {

        public SelectionChangedEventArgs(object model)
        {
            this.CurrentSelection = model;
        }

        public object CurrentSelection { get; }
    }


}
