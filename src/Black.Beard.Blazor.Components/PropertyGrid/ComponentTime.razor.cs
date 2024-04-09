using Microsoft.AspNetCore.Components;

namespace Bb.PropertyGrid
{

    public partial class ComponentTime
    {


        public ComponentTime()
        {

        }              

        public TimeSpan? Time
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
