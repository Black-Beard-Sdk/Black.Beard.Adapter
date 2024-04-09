using Microsoft.AspNetCore.Components;

namespace Bb.PropertyGrid
{

    public partial class ComponentDate
    {

        public DateTime? Date
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = value.Value;
            }
        }



    }

}
