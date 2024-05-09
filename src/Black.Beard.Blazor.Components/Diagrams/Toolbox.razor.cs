using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
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

        public DiagramSpecificationBase CurrentDragStarted { get; private set; }

        public async Task DragStart(DragEventArgs args, DiagramSpecificationBase item)
        {
            this.CurrentDragStarted = item;
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

            var links = items.OfType<DiagramSpecificationRelationshipBase>().ToList();
            if (links.Count ==1)
                CurrentLink = links[0];
            else
                CurrentLink = links.FirstOrDefault(c => c.IsDefaultLink);

        }

        public List<ToolboxCategory> Items { get; }

        public string Current { get; internal set; }

        public DiagramSpecificationRelationshipBase CurrentLink         { get; set; }


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

    }


    public class ClickAbleMudToggleIconButton : MudToggleIconButton
    {

        public ClickAbleMudToggleIconButton()
        {
            ToggledChanged = new EventCallback<bool>(this, LinkMudToggleIconButton_ToggledChanged);
        }

        [Parameter]
        public ToolboxList ToolboxList { get; set; }

        [Parameter]
        public DiagramSpecificationBase Instance { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (ToolboxList.CurrentLink == this.Instance)
                Toggled = true;
        }


        public Action<bool> LinkMudToggleIconButton_ToggledChanged1 { get; }

        public void LinkMudToggleIconButton_ToggledChanged(bool toggled)
        {
            ToolboxList.CurrentLink = this.Instance as DiagramSpecificationRelationshipBase;
        }
        
    }

}
