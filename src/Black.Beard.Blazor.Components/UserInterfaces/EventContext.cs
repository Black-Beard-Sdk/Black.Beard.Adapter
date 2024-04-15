using Microsoft.AspNetCore.Components.Web;

namespace Bb.UserInterfaces
{
    public class EventContext
    {
    
        public EventContext(MouseEventArgs mouseEventArgs)
        {
            MouseEventArgs = mouseEventArgs;
        }

        public EventContext(MouseEventArgs mouseEventArgs, DynamicServerMenu dynamicServerMenu) 
            : this(mouseEventArgs)
        {
            DynamicServerMenu = dynamicServerMenu;
        }

        public void Append(Dictionary<Type, object> _parameters)
        {
            
            foreach (var item in _parameters)
            {
            }
        }

        public MouseEventArgs MouseEventArgs { get; }
        public DynamicServerMenu DynamicServerMenu { get; }
    }



}
