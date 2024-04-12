
namespace Bb.PropertyGrid
{

    public partial class ComponentDateOffset
    {


        public DateTime? DateTimeOffset
        {
            get
            {
                return base.Value.DateTime;
            }
            set
            {
                var utcTime1 = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                DateTimeOffset utcTime2 = utcTime1;
                base.Value = utcTime1;
            }
        }


    }

}
