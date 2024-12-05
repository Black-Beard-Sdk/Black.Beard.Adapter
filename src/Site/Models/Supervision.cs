using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;

namespace Site.Models
{

    [ExposeClass(ConstantsCore.Configuration, "Supervision")]
    public class Supervision
    {

        public bool WithTelemetry { get; set; }

    }

}
