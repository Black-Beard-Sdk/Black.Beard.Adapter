using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace Bb.Diagrams
{

    public static class DiagramExtension
    {

        public static string GetLabel(this MovableModel self)
        {
            var u = self as UIModel;
            if (u != null)
                return u.Label;

            var n = self as NodeModel;
            if (n != null)
                return n.Title;

            return null;

        }


        public static string GetLabel(this Anchor self)
        {

            var m = self.Model;
            if (m is null)
                return null;

            var p = m as PortModel;
            if (p != null)
                return p.GetLabel();

            return null;

        }

        public static string GetLabel(this PortModel self)
        {

            var p = self.Parent;
            if (p is null)
                return null;

            var u = p as UIModel;
            if (u != null)
                return u.Label;

            var n = p as NodeModel;
            if (n != null)
                return n.Title;

            return null;

        }

    }

    public static class DiagramToolNodeExtension
    {

        public static T IsControlled<T>(this T self, bool value)
            where T : DiagramToolNode
        {
            self.ControlledSize = value;
            return self;
        }

        public static T SetPadding<T>(this T self, byte value)
            where T : DiagramToolNode
        {
            self.Padding = value;
            return self;
        }

        public static T IsLocked<T>(this T self, bool value)
            where T : DiagramToolNode
        {
            self.Locked = value;
            return self;
        }

    }

}
