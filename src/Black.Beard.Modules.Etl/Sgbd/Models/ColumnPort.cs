using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using System.Text.Json.Serialization;

namespace Bb.Modules.Sgbd.Models
{

    public class ColumnPort : PortModel
    {

        public ColumnPort(NodeModel parent, string id, Column column, PortAlignment alignment = PortAlignment.Bottom)
            : base(id, parent, alignment, null, null)
        {
            Column = column;
        }


        [JsonIgnore]
        public Column Column { get; internal set; }


        public override bool CanAttachTo(ILinkable other)
        {
            return base.CanAttachTo(other);
        }

        //public override bool CanAttachTo(PortModel port)
        //{
        //    // Avoid attaching to self port/node
        //    if (!base.CanAttachTo(port))
        //        return false;

        //    var targetPort = port as ColumnPort;
        //    var targetColumn = targetPort.Column;

        //    if (Column.Type != targetColumn.Type)
        //        return false;

        //    if (Column.Primary && targetColumn.Primary)
        //        return false;

        //    if (Column.Primary && targetPort.Links.Count > 0 ||
        //        targetColumn.Primary && Links.Count > 1) // Ongoing link
        //        return false;

        //    return true;
        //}



    }
}
