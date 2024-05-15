using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.DataAnnotations;
using Bb.Modules.Sgbd.Models;

namespace Bb.Modules
{
    [ExposeClass(UIConstants.Service, ExposedType = typeof(ListProviderColumnTypeTechnologies), LifeCycle = IocScopeEnum.Transiant)]
    public class ListProviderColumnTypeTechnologies : ProviderListBase<ColumnType>
    {

        public ListProviderColumnTypeTechnologies()
        {


        }

        public override IEnumerable<ListItem<ColumnType>> GetItems()
        {

            List<ListItem<ColumnType>> list = new List<ListItem<ColumnType>>();

            if (Instance != null && Property != null)
            {

                if (Instance is Column c && c.Table.Source.Diagram is SgbdDiagram d)
                {

                    SgbdTechnology techno = d.GetTechnology();
                    if (techno != null)
                    {
                        var items = techno.ColumnTypes;
                        foreach (var item in items)
                            list.Add(CreateItem(item, item.Label, item.Code, c =>
                            {

                            }));
                    }
                }

            }

            return list;

        }

        protected override object ResolveOriginalValue(ListItem<ColumnType> item)
        {
            return item.Value;
        }



    }



}
