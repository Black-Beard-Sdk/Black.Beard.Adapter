using Bb.ComponentModel;
using Bb.ComponentModel.Accessors;
using ICSharpCode.Decompiler.Metadata;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;

namespace Bb.Commands
{


    public static class RestoreExtension
    {

        public static void Resolve<TKey, TValue>(this Dictionary<TKey, TValue> target, Dictionary<TKey, TValue> source, Action<TValue> toRemove, Action<TValue> toAdd, Action<TValue, TValue> toUpdate)
        {

            foreach (var item in source)
                if (!target.ContainsKey(item.Key))
                    toRemove(item.Value);

            foreach (var item in target)
            {
                if (source.TryGetValue(item.Key, out TValue value))
                    toUpdate(value, item.Value);

                else
                    toAdd(item.Value);
            }

        }

        public static void Resolve<T, R>(this IEnumerable<T> targetList, Func<T, R> key, IEnumerable<T> sourceList, Action<T> toRemove, Action<T> toAdd, Action<T, T> toUpdate)
        {
            var source = sourceList.ToDictionary(key);
            var target = targetList.ToDictionary(key);
            Resolve(target, source, toRemove, toAdd, toUpdate);
        }
     
        public static List<AccessorItem> ResolvePropertiesToRestore<T>(this T self)
        {

            if (self == null)
                throw new NullReferenceException(nameof(self));

            var type = self.GetType();
            var accessorSource = type.GetAccessors(MemberStrategy.Instance | MemberStrategy.Properties | MemberStrategy.Direct);
            var items = accessorSource.Where(c => c.EvaluateAccessor(self)).OrderBy(c => c.GetRestoreOrder()).ToList();
            return items;
        }

        private static bool EvaluateAccessor(this AccessorItem self, object left)
        {
            var result = self.IsClonable
                        && !typeof(MulticastDelegate).IsAssignableFrom(self.Type)
                        && !self.ContainsAttribute<RestoreIgnoreAttribute>(left)
                        ;
            return result;
        }

        public static int GetRestoreOrder(this AccessorItem self)
        {
            var o = self.GetAttributes<RestoreOrderAttribute>().FirstOrDefault();
            if (o != null)
                return o.Order;
            return int.MaxValue;
        }

    }


}
