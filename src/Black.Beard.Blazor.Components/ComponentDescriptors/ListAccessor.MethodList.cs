namespace Bb.ComponentDescriptors
{


    public partial class ListAccessor
    {
        private class MethodList<T> : MethodListBase
        {

            public MethodList()
            {
                IsList = true;
                this.CanAdd = true;
                this.CanDel = true;
                this.CanGet = true;
                this.CanSet = true;
            }

            public override object GetKey(object instance, object value)
            {
                var i = instance as IList<T>;
                return i.IndexOf((T)value);
            }

            public override void Add(object instance, object key, object value)
            {
                var i = instance as IList<T>;
                i.Add((T)value);
            }

            public override void Del(object instance, object key)
            {
                var i = instance as IList<T>;
                i.Remove((T)key);
            }

            public override object Get(object instance, object key)
            {
                var i = instance as IList<T>;
                var index = (int)key;
                return i[index];
            }

            public override void Set(object instance, object key, object value)
            {
                var i = instance as IList<T>;
                var index = (int)key;
                i[index] = (T)value;
            }


        }

    }


}