using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Extensions;

namespace Bb.Diagrams
{
    /// <summary>
    /// Override NodeModel.
    /// NodeModel is graphical model
    /// </summary>
    public class UIGroupModel : UIModel
    {

        public UIGroupModel(SerializableDiagramGroupNode source)
            : base(source)
        {
            Padding = source.Padding;
            _children = new HashSet<NodeModel>();
        }

        #region Add/Delete

        #region add

        public void AddChildren(params UIModel[] children)
        {
            bool flag = false;

            foreach (UIModel child in children)
            {
                flag = child.SetParent(new Guid(this.Id));
                flag = flag | Attach(child);
            }

            if (flag && UpdateDimensions())
                Refresh();

        }


        internal bool Attach(NodeModel child)
        {
            if (child != null)
                if (_children.Add(child))
                {
                    Observe(child);
                    return true;
                }
            return false;
        }

        private void Observe(NodeModel child)
        {
            child.SizeChanged += OnNodeChanged;
            child.Moving += OnNodeChanged;
        }

        #endregion add

        #region delete

        public void RemoveChildren(params UIModel[] children)
        {
            bool flag = false;
            foreach (UIModel child in children)
            {
                flag = child.SetParent(null);
                flag = flag | Detach(child);
            }

            if (flag && UpdateDimensions())
            {
                Refresh();
                RefreshLinks();
            }

        }

        internal bool Detach(NodeModel child)
        {

            if (child != null)
                if (_children.Remove(child))
                {
                    UnObserve(child);
                    return true;
                }
            return false;
        }

        private void UnObserve(NodeModel child)
        {
            child.SizeChanged -= OnNodeChanged;
            child.Moving -= OnNodeChanged;
        }

        public void UnGroup()
        {

            foreach (NodeModel child in Children)
                UnObserve(child);

            _children.Clear();

            if (UpdateDimensions())
            {
                Refresh();
                RefreshLinks();
            }

        }

        #endregion delete

        #endregion Add/Delete


        public override void SetPosition(double x, double y)
        {
            double deltaX = x - base.Position.X;
            double deltaY = y - base.Position.Y;
            base.SetPosition(x, y);
            foreach (NodeModel child in Children)
            {
                child.UpdatePositionSilently(deltaX, deltaY);
                child.RefreshLinks();
            }

            Refresh();
            RefreshLinks();
        }

        public override void UpdatePositionSilently(double deltaX, double deltaY)
        {
            base.UpdatePositionSilently(deltaX, deltaY);
            foreach (NodeModel child in Children)
            {
                child.UpdatePositionSilently(deltaX, deltaY);
            }

            Refresh();
        }


        private void OnNodeChanged(NodeModel node)
        {
            if (UpdateDimensions())
                Refresh();
        }

        private bool UpdateDimensions()
        {

            if (Children.Count() == 0)
                return true;

            if (Children.Any((NodeModel n) => n.Size == null))
                return false;

            Rectangle bounds = Children.GetBounds();
            Point point = new Point(bounds.Left - (double)(int)Padding, bounds.Top - (double)(int)Padding);
            if (!base.Position.Equals(point))
            {
                base.Position = point;
                TriggerMoving();
            }

            base.Size = new Size(bounds.Width + (double)(Padding * 2), bounds.Height + (double)(Padding * 2));
            return true;
        }

        public byte Padding { get; }

        public IEnumerable<NodeModel> Children => _children;


        private readonly HashSet<NodeModel> _children;

    }

}

