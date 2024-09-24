using System.Collections;

namespace Bb.Diagrams
{

    public class DiagramToolbox : IEnumerable<DiagramToolBase>
    {

        public DiagramToolbox()
        {
            _tools = new List<DiagramToolBase>();
        }


        public DiagramToolbox Add<T>(T instance, Action<T>? initializer = null)
            where T : DiagramToolBase
        {

            if (instance != null)
            {
                if (initializer != null)
                    initializer.Invoke(instance);

                _tools.Add(instance);

            }

            return this;

        }

        public IEnumerator<DiagramToolBase> GetEnumerator()
        {
            return _tools.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tools.GetEnumerator();
        }

        private readonly List<DiagramToolBase> _tools;

    }

}
