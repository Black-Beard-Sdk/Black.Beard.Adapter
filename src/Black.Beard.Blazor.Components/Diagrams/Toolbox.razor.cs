using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

        public DiagramSpecificationBase LastDrag { get; private set; }

        public object DragStart(DiagramSpecificationBase item)
        {

            //e.DataTransfer?.SetData("text", item.Uuid.ToString());

            var e = new DragEventArgs();
            e.DataTransfer = new DataTransfer();
            e.DataTransfer.Types = new string[] { item.Category.Key, item.Kind.ToString() };
            e.DataTransfer.Items = new DataTransferItem[] { new DataTransferItem() { Kind = item.Kind.ToString(), Type = item.Uuid.ToString() } };

            e.DataTransfer.DropEffect = "copy";
            e.DataTransfer.EffectAllowed = "copy";

            this.LastDrag = item;

            return e;

        }

    }



    public class ToolboxList
    {


        public ToolboxList(IEnumerable<DiagramSpecificationBase> items)
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
        public string Current { get; internal set; }

        public void EnsureCategoryIsShown(DiagramSpecificationBase item)
        {

            foreach (var category in Items)
                if (category.Items.Contains(item))
                {
                    category.Toggle();
                    return;
                }

        }
    }


    public class ToolboxCategory
    {

        public ToolboxCategory(ToolboxList parent, string name, IEnumerable<DiagramSpecificationBase> items)
        {
            _parent = parent;
            Name = name;
            Items = new List<DiagramSpecificationBase>(items);
        }


        public bool IsExpanded
        {
            get => _parent.Current == this.Name;
        }


        public string Name { get; set; }

        public List<DiagramSpecificationBase> Items { get; }

        public void Toggle()
        {
            _parent.Current = this.Name;
        }

        private readonly ToolboxList _parent;
        private ToolboxCategory _current;

    }

}
