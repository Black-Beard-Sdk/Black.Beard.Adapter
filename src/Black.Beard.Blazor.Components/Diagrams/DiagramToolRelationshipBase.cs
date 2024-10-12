using Bb.ComponentModel.Translations;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using System.Linq.Expressions;

namespace Bb.Diagrams
{
    public class DiagramToolRelationshipBase : DiagramToolBase
    {
        private LinkMarker _targetMarker;
        private LinkMarker _sourceMarker;

        public DiagramToolRelationshipBase(Guid uuid, TranslatedKeyLabel category, TranslatedKeyLabel name, TranslatedKeyLabel description, string icon)
            : base(uuid, category, name, description, icon)
        {
            Kind = ToolKind.Link;
        }

        public override string GetDefaultName()
        {
            return $"Link";
        }



        public void SetTargetMarker(LinkMarker marker)
        {
            _targetMarker = marker;
        }

        public void SetSourceMarker(LinkMarker marker)
        {
            _sourceMarker = marker;
        }

        public bool IsDefaultLink { get; protected set; }



        public virtual Anchor CreateAnchor(NodeModel model)
        {
            return new ShapeIntersectionAnchor(model);
        }

        public virtual Anchor CreateAnchor(PortModel model)
        {
            return new SinglePortAnchor(model);
        }



        public virtual LinkProperties CreateLink(Guid uuid, PortModel source, PortModel target)
        {
            return CreateLink(uuid, CreateAnchor(source), CreateAnchor(target));
        }

        public virtual LinkProperties CreateLink(Guid uuid, Anchor source, Anchor target)
        {

            Guid sourceId = source.Model.GetId();
            Guid targetId = target.Model.GetId();

            SerializableRelationship relationship = new SerializableRelationship()
            {
                Name = string.Empty,
                Uuid = uuid,
                Type = Uuid,
                Source = sourceId,
                Target = targetId,
            };

            return CreateLink(relationship, source, target);

        }

        internal protected virtual LinkProperties CreateLink(SerializableRelationship relationship, PortModel source, PortModel target)
        {
            var l = CreateLink(relationship, CreateAnchor(source), CreateAnchor(target));
            return l;
        }

        protected virtual LinkProperties CreateLink(SerializableRelationship relationship, Anchor source, Anchor target)
        {
            //var l = new CustomizedLinkModel(link, source, target);
            var link = new LinkModel(relationship.Uuid.ToString(), source, target);
            var m = new LinkProperties(relationship, link);
            link.PathGenerator = GetPathGenerator();
            link.Router = GetRouter();
            return m;
        }


        public void AddLabel(string value, double? distance = null, Blazor.Diagrams.Core.Geometry.Point? point = null)
        {
            AddLabel(Expression.Constant(value), distance, point);
        }


        public void AddLabel(Expression e, double? distance = null, Blazor.Diagrams.Core.Geometry.Point? point = null)
        {

            var model = new LabelCreator()
            {
                Content = e,
                Distance = distance,
                Point = point
            };

            _models.Add(model);

        }


        internal protected virtual void Customize(LinkProperties link)
        {

            if (_sourceMarker != null)
                link.UILink.SourceMarker = _sourceMarker;

            if (_targetMarker != null)
                link.UILink.TargetMarker = _targetMarker;

            link.SetLabels(_models);

        }

        public virtual PathGenerator GetPathGenerator()
        {
            //return new SmoothPathGenerator();
            return new StraightPathGenerator();
        }

        public virtual Router GetRouter()
        {
            //return new NormalRouter()
            return new OrthogonalRouter();
        }


        private List<LabelCreator> _models = new List<LabelCreator>();

    }


}
