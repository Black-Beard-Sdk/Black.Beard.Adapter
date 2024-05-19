using Microsoft.AspNetCore.Components;

namespace Bb.PropertyGrid
{

    public partial class ComponentTime
    {


        public ComponentTime()
        {
            InputType = MudBlazor.InputType.Time;
        }

        //public DateTime? Time
        //{
        //    get
        //    {
        //        return new DateTime(base.Value.Ticks);
        //    }
        //    set
        //    {
        //        base.Value = new TimeSpan(0, value.Value.Hour, value.Value.Minute, value.Value.Second);
        //    }
        //}


    }

}
