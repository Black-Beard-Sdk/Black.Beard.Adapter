namespace Bb.ComponentDescriptors
{


    public partial class ListAccessor
    {
        private abstract class MethodListBase
        {

            public bool CanAdd { get; protected set; }

            public bool CanDel { get; protected set; }

            public bool CanGet { get; protected set; }

            public bool CanSet { get; protected set; }

            public bool IsDictionary { get; protected set; }

            public bool IsList { get; protected set; }


            public abstract object GetKey(object instance, object key);

            public abstract void Add(object instance, object key, object value);

            public abstract void Del(object instance, object key);

            public abstract object Get(object instance, object key);

            public abstract void Set(object instance, object key, object value);


        }

    }


}