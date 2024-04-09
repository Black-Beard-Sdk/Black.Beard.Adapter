using Bb.ComponentModel.DataAnnotations;
using System.Collections;
using System.ComponentModel;

namespace Bb.Modules
{
    public class ListProviderModule : IListProvider
    {

        public object Instance { get; set; }

        public PropertyDescriptor Property { get; set; }

        public IEnumerable<ListItem> GetItems()
        {
            List<ListItem> list = new List<ListItem>();

            if (Instance != null && Property != null)
            {
                var items = (IEnumerable)Property.GetValue(Instance);
                foreach (var item in items)
                {
                    var value = item.GetType().GetProperty("Value").GetValue(item);
                    var text = item.GetType().GetProperty("Text").GetValue(item);
                    list.Add(new ListItem() { Name = "", Display = "", Value = "" });
                }
            }

            return list;
        }

    }


}
