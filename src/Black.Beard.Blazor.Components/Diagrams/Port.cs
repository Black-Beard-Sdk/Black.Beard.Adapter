using Blazor.Diagrams.Core.Models;

namespace Bb.Diagrams
{

    public class Port
    {

        public Port()
        {

        }

        public Guid Uuid { get; set; }

        public PortAlignment Alignment { get; set; }

        public override int GetHashCode()
        {
            int result = Uuid.GetHashCode();
            result ^= Alignment.GetHashCode();
            return result;
        }

    }


}
