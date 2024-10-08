﻿using Bb.ComponentModel.Translations;
using Bb.Modules.Bpms.Models;
using Bb.Modules.Etl.Models;
using Bb.PropertyGrid;
using Bb.TypeDescriptors;
using Blazor.Diagrams.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bb.Modules.bpms.Components
{

    public partial class SwimLaneComponent : ComponentBase
    {

        public SwimLaneComponent()
        {

        }

        //[Inject]
        //private IJSRuntime Runtime { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }


        [Inject]
        public IRefreshService RefreshService { get; set; }


        [Parameter]
        public NodeModel? Node { get; set; }

        public BpmsSwimLane? Item => Node as BpmsSwimLane;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            //var result = base.OnAfterRenderAsync(firstRender);
            //var rect = await Runtime.GetBoundingClientRect(this._element);
            //if (rect != null)
            //{
            //    Item.Size = new Size(rect.Width, rect.Height);
            //    StateHasChanged();
            //}

        }

        public string? Name
        {
            get => Node?.Title;
            set
            {
                if (Node != null)
                {
                    Node.Title = value;
                    RefreshService.CallRequestRefresh(this, nameof(PropertyGridView));
                }
            }
        }

        public double Height
        {
            get => Item?.Size?.Height ?? 10d;
        }

        public int HalfHeight
        {
            get
            {
                return 90;
                //Item.Source.GetDiagram<Diagram>().Getii(Height);
                //return (int)(Height / 2);
            }
        }

        public int HalfWidth
        {
            get
            {
                return 100;
            }
        }

        public double Width
        {
            get => Item?.Size?.Width ?? 10d;
        }

        // private ElementReference _element;

    }
}
