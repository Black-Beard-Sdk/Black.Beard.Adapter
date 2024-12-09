using Bb.TypeDescriptors;
using Bb.Commands;

namespace Bb.Diagrams
{
    public class Ports
        : List<Port>
        , IRestorableModel
    {

        public Ports()
        {

        }

        public Ports(int capacity)
            : base(capacity)
        {

        }

        public Ports(IEnumerable<Port> collection)
            : base(collection)
        {

        }

        public override int GetHashCode()
        {
            int result = 0;

            foreach (var item in this.OrderBy(c => c.Uuid))
                result ^= item.GetHashCode();

            return result;

        }

        public override bool Equals(object? obj)
        {

            if (obj is Ports ports)
            {

                if (ports.Count != this.Count)
                    return false;

                foreach (var item in ports)
                    if (!this.Any(c => c.Uuid == item.Uuid))
                        return false;

            }

            return true;

        }

        public bool Restore(object model, RefreshContext context, RefreshStrategy strategy = RefreshStrategy.All)
        {

            bool result = false;

            if (model is Ports ports)
            {

                if (strategy.HasFlag(RefreshStrategy.Remove))
                {
                    var l = this.ToList();
                    foreach (var item in l)
                    {
                        var p = Get(item.Uuid);
                        if (p != null)
                        {
                            Remove(p);
                            result = true;
                        }
                    }

                }

                if (strategy.HasFlag(RefreshStrategy.Update))
                    foreach (var item in ports)
                    {
                        var p = Get(item.Uuid);
                        if (p != null && p.Alignment != item.Alignment)
                        {
                            item.Alignment = p.Alignment;
                            result = true;
                        }
                    }

                if (strategy.HasFlag(RefreshStrategy.Add))
                    foreach (var item in ports)
                    {
                        var p = Get(item.Uuid);
                        if (p == null)
                        {
                            Add(p);
                            result = true;
                        }
                    }

            }

            return result;

        }

        public void Remove(Guid uuid)
        {
            var i = Get(uuid);
            if (i != null)
                Remove(i);
        }

        public Port? Get(Guid uuid)
        {
            return this.FirstOrDefault(c => c.Uuid == uuid);
        }

    }


}
