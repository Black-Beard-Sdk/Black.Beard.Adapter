
namespace Bb.PropertyGrid
{

    public partial class ComponentChar
    {

        public string? Item
        {
            get
            {
                if (base.Value != '\0')
                    return base.Value.ToString();
                return null;
            }
            set
            {
                base.Value = !string.IsNullOrEmpty(value) ? value[0] : '\0';
            }
        }



    }

}
