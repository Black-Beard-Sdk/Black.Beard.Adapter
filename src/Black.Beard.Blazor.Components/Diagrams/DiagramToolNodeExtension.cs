namespace Bb.Diagrams
{
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
