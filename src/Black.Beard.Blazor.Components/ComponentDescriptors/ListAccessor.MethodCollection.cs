namespace Bb.ComponentDescriptors
{


    public partial class ListAccessor
    {
        private class MethodCollection<T> : MethodListBase
        {

            public MethodCollection()
            {
                IsList = true;
                this.CanAdd = true;
                this.CanDel = true;
                this.CanGet = true;
                this.CanSet = true;
            }

            public override object GetKey(object instance, object key)
            {

                var i = instance as ICollection<T>;
                int c = 0;
                foreach (var item in i)
                {
                    if (object.Equals(item, key))
                        return c;
                    c++;
                }

                return key;
            }

            public override void Add(object instance, object key, object value)
            {
                var i = instance as ICollection<T>;
                i.Add((T)value);
            }

            public override void Del(object instance, object key)
            {

            }

            public override object Get(object instance, object key)
            {
                return null;
            }

            public override void Set(object instance, object key, object value)
            {

            }

        }

    }


}