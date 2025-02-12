namespace Bb.ComponentDescriptors
{


    public partial class ListAccessor
    {

        private class MethodDictionary<TKey, TValue> : MethodListBase
        {

            public MethodDictionary()
            {
                IsDictionary = true;
                this.CanAdd = true;
                this.CanDel = true;
                this.CanGet = true;
                this.CanSet = true;
            }

            public override object GetKey(object instance, object value)
            {

                var dic = instance as IDictionary<TKey, TValue>;
                foreach (var item in dic)
                    if (item.Value.Equals(value))
                        return item.Key;

                return default;

            }

            public override void Add(object instance, object key, object value)
            {
                var dic = instance as IDictionary<TKey, TValue>;
                var k = (TKey)key;
                var v = (TValue)value;
                dic.Add(k, v);
            }

            public override void Del(object instance, object key)
            {
                var k = (TKey)key;
                var dic = instance as IDictionary<TKey, TValue>;
                dic.Remove(k);
            }

            public override object Get(object instance, object key)
            {
                var k = (TKey)key;
                var dic = instance as IDictionary<TKey, TValue>;
                if (dic.TryGetValue(k, out var v))
                    return v;
                return default(TValue);
            }

            public override void Set(object instance, object key, object value)
            {
                var dic = instance as IDictionary<TKey, TValue>;
                var k = (TKey)key;
                var v = (TValue)value;
                dic[k] = v;
            }


        }

    }


}