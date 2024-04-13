
using Bb.ComponentModel.Translations;

namespace Bb.UIComponents
{


    public class ItemEnumerable<T> : ItemEnumerable
    {

        public ItemEnumerable(T item, TranslatedKeyLabel label)
        {
            base.Label = label;
            this.Tag = item;
        }

        public T? Tag { get; }

    }



}


