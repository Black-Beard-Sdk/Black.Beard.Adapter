﻿using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Models;

namespace Bb.Modules.Etl.Etl
{
    [ExposeClass(EtlDiagramFeature.Filter, ExposedType = typeof(DiagramSpecificationBase))]
    public class TemplateJsltTool : DiagramSpecificationNodeBase
    {

        public TemplateJsltTool()
            : base(new Guid(Key),
                  "Template jslt",
                  "transform json",
                  GlyphFilled.Transform)
        {

            AddPort(PortAlignment.Left, PortAlignment.Right);

        }

        public override string GetDefaultName()
        {
            return $"jslt";
        }


        public const string Key = "078884D0-0D27-421E-8332-EA08A8EBAEF5";

    }
}
