using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.DataAnnotations;
using Bb.Modules.Sgbd.Models;
using static MudBlazor.CategoryTypes;

namespace Bb.Modules
{

    [ExposeClass(UIConstants.Service, ExposedType = typeof(ListProviderSelectColumn), LifeCycle = IocScopeEnum.Transiant)]
    public class ListProviderSelectColumn : ProviderListBase<Column>
    {

        public ListProviderSelectColumn()
        {
        }

        public override IEnumerable<ListItem<Column>> GetItems()
        {

            List<ListItem<Column>> list = new List<ListItem<Column>>();
            IEnumerable<Column> items = null;

            if (Instance != null && Property != null)
            {

                if (Instance is ColumnIndex column)
                {
                    var table = column.Index.Table;
                    items = table.Columns;
                }
                else
                {
                }

                if (items != null)
                    foreach (var item in items)
                        list.Add(CreateItem(item, item.Name, item.Id, c =>
                        {

                        }));

            }

            return list;

        }

        protected override object ResolveOriginalValue(ListItem<Column> item)
        {
            return item.Value;
        }


    }



}
