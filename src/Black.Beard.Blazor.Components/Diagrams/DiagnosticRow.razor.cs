using Bb.PropertyGrid;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bb.Diagrams
{
    public partial class DiagnosticRow : ComponentBase
    {

        [Parameter]
        public Diagnostic Row { get; set; }

        [Parameter]
        public PropertyGridView PropertyGridView { get; set; }

        public void OnClick(MouseEventArgs eventArgs)
        {
            if (Row.Target.Model != null)
                PropertyGridView.SelectedObject = Row.Target.Model;
        }

    }

    public static class DiagnosticRowExtensions
    {

        public static string ConvertToIcon(this DiagnosticLevel self)
        {
            switch (self)
            {
         
                case DiagnosticLevel.Warning:
                    return @Icons.Material.Filled.Warning;
                
                case DiagnosticLevel.Error:
                    return @Icons.Material.Filled.Error;
                
                case DiagnosticLevel.Info:
                default:
                    return @Icons.Material.Filled.Info;

            }
        }

        public static Color ConvertToColor(this DiagnosticLevel self)
        {
            
            switch (self)
            {

                case DiagnosticLevel.Warning:
                    return Color.Warning;
                
                case DiagnosticLevel.Error:
                    return Color.Error;
                
                case DiagnosticLevel.Info:
                default:
                    return Color.Info;
            
            }

        }

    }


}
