using Bb.Commands;
using Bb.ComponentModel.Accessors;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using static MudBlazor.CategoryTypes;

namespace Bb.Diagrams
{

    public static class DiagramExtension
    {

        public static void Apply(this object left, object right, string nameKey, RefreshContext context)
        {

            bool result = false;

            var accessorSource = left.GetType().GetAccessors(AccessorStrategyEnum.Direct);
            var accessorTarget = right.GetType().GetAccessors(AccessorStrategyEnum.Direct);

            foreach (var item in accessorSource)
            {

                var target = accessorTarget.Get(item.Name);
                if (target != null)
                {
                    if (target.CanWrite && target.Member.MemberType == System.Reflection.MemberTypes.Property)
                    {
                        if (target.Type == item.Type)
                        {

                            var oldValue = target.GetValue(left);
                            var newValue = item.GetValue(right);

                            if (!Compare(oldValue, newValue))
                            {

                                if (item.Type.IsValueType || item.Type == typeof(string))
                                {
                                    target.SetValue(left, newValue);
                                    result = true;
                                }

                                else if (newValue is IRestorableModel r)
                                {
                                    r.Restore(oldValue, context, RefreshStrategy.All);
                                }

                                else if (!TryApply(oldValue, newValue))
                                {
                                    target.SetValue(left, newValue);
                                    result = true;
                                }

                            }

                        }

                        else
                        {

                        }
                    }
                }
            }

            if (result)
            {
                var key = accessorTarget.Get(nameKey).ToString();
                context.Add(key, left, RefreshStrategy.Update);
            }

        }

        private static bool TryApply(object oldValue, object newValue)
        {
            return false;
        }

        private static bool Compare(object oldValue, object newValue)
        {

            if (object.Equals(oldValue, newValue))
                return true;

            return false;

        }

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
