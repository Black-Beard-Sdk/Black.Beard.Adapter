using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace Bb.Diagrams
{


    public class CustomizedLinkLabelModel : LinkLabelModel
    {

        public CustomizedLinkLabelModel(BaseLinkModel parent, LabelCreator creator)
            : this(parent, creator.Id, creator.Refresh(), creator.Distance, creator.Point) 
        {
         
            this._creator = creator;
            this.Content = creator.Refresh();
            this.Distance = creator.Distance;
            this.Offset = creator.Point;
        }

        public CustomizedLinkLabelModel(BaseLinkModel parent, Guid id, string content, double? distance = null, Blazor.Diagrams.Core.Geometry.Point? offset = null) 
            : base(parent, id.ToString(), content, distance, offset)
        {

        }

        public bool Update()
        {

            var c = _creator.Refresh();
            if (this.Content != c)
            {
                this.Content = c;
                return true;
            }

            return false;

        }

        private readonly LabelCreator _creator;

    }


}
