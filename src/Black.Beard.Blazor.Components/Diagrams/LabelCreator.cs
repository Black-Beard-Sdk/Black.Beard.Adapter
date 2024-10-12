using Blazor.Diagrams.Core.Models;
using System.Linq.Expressions;
using Blazor.Diagrams.Core.Geometry;

namespace Bb.Diagrams
{
    public class LabelCreator
    {

        /// <summary>
        /// initialize a new instance of <see cref="LabelCreator"/>
        /// </summary>
        public LabelCreator()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set;}

        public Expression Content { get; set; }

        public double? Distance { get; set; }

        public Point? Point { get; set; }

        internal string Refresh()
        {

            if (_function == null)
                _function = Expression.Lambda<Func<string>>(Content)
                    .Compile();

            return _function();

        }

        internal LinkLabelModel Create(LinkModel link)
        {
            return new CustomizedLinkLabelModel(link, this);
        }

        private Func<string> _function;

    }


}
