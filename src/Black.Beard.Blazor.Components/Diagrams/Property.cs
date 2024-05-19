using System.Diagnostics;

namespace Bb.Diagrams
{

    [DebuggerDisplay("{Name} = {Value}")]
    public class Property
    {

        public string Name { get; set; }

        public string Value { get; set; }

    }


}
