namespace Bb.Diagrams
{
    public class ToolboxList
    {

        public ToolboxList(IEnumerable<DiagramToolBase> items)
        {

            if (items != null)
            {

                var i = items.ToLookup(i => i.Category).ToList();
                Items = new List<ToolboxCategory>(i.Count);
                foreach (var item in i)
                {
                    var display = item.Key.DefaultDisplay;
                    var category = new ToolboxCategory(this, display, item);
                    Items.Add(category);
                }

                var links = items.OfType<DiagramToolRelationshipBase>().ToList();
                if (links.Count == 1)
                    CurrentLink = links[0];
                else
                    CurrentLink = links.FirstOrDefault(c => c.IsDefaultLink);

            }

        }

        public List<ToolboxCategory> Items { get; }

        public string Current { get; internal set; }

        public DiagramToolRelationshipBase CurrentLink         { get; set; }


        public void EnsureCategoryIsShown(DiagramToolBase item)
        {

            foreach (var category in Items)
                if (category.Items.Contains(item))
                {
                    category.Toggle();
                    return;
                }

        }


    }

}
