using Microsoft.AspNetCore.Components.Routing;


namespace Bb.UIComponents
{

    public class ActionReference
    {

        public NavLinkMatch Match { get; set; }

        public string? HRef { get; set; }

        public static ActionReference Default { get; } = new ActionReference() { HRef = "\\", Match = NavLinkMatch.Prefix };

    }


}


