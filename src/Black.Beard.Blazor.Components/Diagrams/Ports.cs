using Bb.TypeDescriptors;
using Bb.Commands;
using Blazor.Diagrams.Core.Models.Base;
using static MudBlazor.CategoryTypes;

namespace Bb.Diagrams
{
    public class Ports
        : List<Port>
        , IRestorable
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


        public bool Restore(object right, RefreshContext context)
        {

            bool result = false;

            if (right is Ports ports)
            {

                Action<Port> remove = (c) =>
                {
                    Remove(c);
                    result = true;
                    context.Apply(RefreshStrategy.Removed, c);
                };

                Action<Port> add = (c) => 
                {
                    Add(c);
                    result = true;
                    context.Apply(RefreshStrategy.Added, c);
                };

                Action<Port, Port> update = (c, d) => 
                {
                    if (c.Alignment != d.Alignment)
                    {
                        c.Alignment = d.Alignment;
                        result = true;
                        context.Apply(RefreshStrategy.Updated, c);
                    }
                };

                this.Resolve(c => c.Uuid, ports, remove, add, update);

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
