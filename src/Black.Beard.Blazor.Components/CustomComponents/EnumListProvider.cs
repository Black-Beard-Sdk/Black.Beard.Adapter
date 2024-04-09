using Bb.ComponentModel.DataAnnotations;
using Bb.ComponentModel.Translations;
using System.ComponentModel;

namespace Bb.CustomComponents
{

    public class EnumListProvider : IListProvider
    {


        public EnumListProvider()
        {

        }


        public PropertyDescriptor Property { get; set; }

        public ITranslateService TranslateService { get; set; }

        public object Instance { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<ListItem> GetItems()
        {

            var values = Enum.GetValues(Property.PropertyType);
            var fields = Property.PropertyType.GetFields();

            foreach (var item in values)
            {

                var n = item.ToString();
                var o = fields.Where(f => f.Name == n).First();
                TranslatedKeyLabel label = o.GetFrom().FirstOrDefault() ?? n;

                yield return new ListItem()
                {
                    Name = n,
                    Value = item,
                    Display = TranslateService.Translate(label),
                };

            }


        }


    }

}
