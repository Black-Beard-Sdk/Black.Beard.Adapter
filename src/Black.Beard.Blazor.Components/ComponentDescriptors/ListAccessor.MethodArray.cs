using static MudBlazor.Colors;

namespace Bb.ComponentDescriptors
{


    public partial class ListAccessor
    {

        private class MethodArray<T> : MethodListBase
        {

            public MethodArray()
            {
                IsList = true;
                //this.CanAdd = true;
                //this.CanDel = true;
                this.CanGet = true;
                this.CanSet = true;
            }

            public override object GetKey(object instance, object key)
            {

                var i = instance as T[];
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
                throw new NotImplementedException();
            }

            public override void Del(object instance, object key)
            {
                throw new NotImplementedException();
            }

            public override object Get(object instance, object key)
            {
                var k = (int)key;
                var i = instance as T[];
                return (T)i[k];
            }

            public override void Set(object instance, object key, object value)
            {
                var k = (int)key;
                var i = instance as T[];
                i[k] = (T)value;
            }

        }

    }


}