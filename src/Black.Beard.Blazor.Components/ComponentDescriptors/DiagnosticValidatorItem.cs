using System.ComponentModel;

namespace Bb.ComponentDescriptors
{
    public class DiagnosticValidatorItem
    {

        public DiagnosticValidatorItem()
        {

            this._diagnostics = new List<string>();
        }

        public void Add(string message)
        {
            _diagnostics.Add(message);
        }

        public IEnumerable<string> Messages => _diagnostics;

        public string Message => String.Concat(_diagnostics.Select(c => ", " + c)).Trim(',', ' ');

        public bool IsValid => _diagnostics.Count == 0;

        public List<string> MessageService => _diagnostics;

        public object Value { get; internal set; }

        private readonly List<string> _diagnostics;

    }


}
