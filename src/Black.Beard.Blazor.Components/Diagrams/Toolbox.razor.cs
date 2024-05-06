using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Bb.Diagrams
{

    public partial class Toolbox : ComponentBase, ITranslateHost
    {

        [Parameter]
        public ToolboxList Tools { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }

    }



    public class ToolboxList
    {


        public ToolboxList(IEnumerable<DiagramToolBase> items)
        {

            var i = items.ToLookup(i => i.Category).ToList();
            Items = new List<ToolboxCategory>(i.Count);
            foreach (var item in i)
            {
                var display = item.Key.DefaultDisplay;
                var category = new ToolboxCategory(this, display, item);
                Items.Add(category);
            }
        }

        public List<ToolboxCategory> Items { get; }

    }


    public class ToolboxCategory
    {

        public ToolboxCategory(ToolboxList parent, string name, IEnumerable<DiagramToolBase> items)
        {
            _parent = parent;
            Name = name;
            Items = new List<DiagramToolBase>(items);
        }

        public bool IsExpanded { get; set; }


        public string Name { get; set; }

        public List<DiagramToolBase> Items { get; }

        public void Toggle()
        {
            _parent.Items.ForEach(i => i.IsExpanded = i == this);
        }
        private readonly ToolboxList _parent;

    }

}
