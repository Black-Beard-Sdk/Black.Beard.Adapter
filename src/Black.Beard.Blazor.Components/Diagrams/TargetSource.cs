namespace Bb.Diagrams
{
    public class TargetSource
    {
        private object model;

        public TargetSource(object model)
        {
            this.model = model;
        }

        public Guid Feature { get; set; }

        public List<Guid>? Target { get; set; }

        public object Model => model;

    }


}
