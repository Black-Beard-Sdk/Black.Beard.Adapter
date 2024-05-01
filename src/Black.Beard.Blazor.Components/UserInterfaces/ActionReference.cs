using Microsoft.AspNetCore.Components.Routing;


namespace Bb.UserInterfaces
{

    public class ActionReference
    {

        public NavLinkMatch Match { get; set; }

        public string? HRef 
        { get
            {
                return _hRef;
            }
            set
            {
                _hRef = value;
            }
        }


        private string? _hRef;

        public static ActionReference Empty { get; } = new ActionReference() { HRef = null, Match = NavLinkMatch.All };


    }


}


