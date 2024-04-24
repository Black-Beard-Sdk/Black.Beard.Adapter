using Microsoft.AspNetCore.Components.Routing;


namespace Bb.UserInterfaces
{

    public class ActionReference
    {

        public NavLinkMatch Match { get; set; }

        public string? HRef { get; set; }

        public static ActionReference Default { get; } = new ActionReference() { HRef = "\\", Match = NavLinkMatch.Prefix };
        
        public static ActionReference Empty { get; } = new ActionReference() { HRef = string.Empty, Match = NavLinkMatch.All };


    }


}


