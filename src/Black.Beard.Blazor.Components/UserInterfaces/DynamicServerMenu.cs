
namespace Bb.UserInterfaces
{


    public class DynamicServerMenu : ServerMenu
    {

        public DynamicServerMenu(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {

        }

        public Action<ServerMenu, List<ServerMenu>> Builder { get; internal set; }


        override public bool IsEmpty
        {
            get
            {
                return false;
            }
        }
    }



}
