
using Bb.ComponentModel.Translations;


namespace Bb.UIComponents
{

    [System.Diagnostics.DebuggerDisplay("{Label}")]
    public class ItemEnumerable
    {

        public ItemEnumerable()
        {
            Label = TranslatedKeyLabel.EmptyKey;
            Subs = new List<ItemEnumerable>();
            Keys = new Dictionary<string, string>();
        }

        public TranslatedKeyLabel Label { get; protected set; }


        public List<ItemEnumerable> Subs { get; set; }

        public Dictionary<string, string> Keys { get; protected set; }


    }



}


