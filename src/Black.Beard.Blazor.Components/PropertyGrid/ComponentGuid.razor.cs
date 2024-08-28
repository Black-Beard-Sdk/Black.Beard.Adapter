using Bb.ComponentDescriptors;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Bb.PropertyGrid
{

    public partial class ComponentGuid
    {


        protected override Task OnInitializedAsync()
        {

            return base.OnInitializedAsync();
        }


        //public override string? ValueString
        //{
        //    get
        //    {
        //        return base.ValueString;
        //    }
        //    set
        //    {
        //        base.ValueString = value;
        //    }
        //}


        //protected override bool ConvertFromString(string value, out Guid valueResult)
        //{

        //    if (Guid.TryParse(value, out valueResult))
        //    {
        //        return true;
        //    }
        //    else
        //    {

        //    }

        //    return false;

        //}


        //protected override IMask CreateMask()
        //{
        //    return GuidMask.Guid();
        //}


    }

}
