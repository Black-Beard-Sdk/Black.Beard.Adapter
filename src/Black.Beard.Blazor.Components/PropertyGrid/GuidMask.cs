using MudBlazor;

namespace Bb.PropertyGrid
{
    public class GuidMask : RegexMask
    {

        protected GuidMask(string regex, string mask)
            : base(regex, mask)
        {
        }

        public static RegexMask Guid()
        {
            var delimiters = "-";
            var mask = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";
            const string IPv6Filter = "(?!.*?[-]{2}?-)";
            var pattern = "[0-9A-Fa-f]{0,8}((-[0-9A-Fa-f]{0,4}){0,3}(-[0-9A-Fa-f]{0,12})?)";
            var regex = $"^{IPv6Filter}{pattern}{WhiteSpaceFilter}$";
            var regexMask = new GuidMask(regex, mask) { Delimiters = delimiters, AllowOnlyDelimiters = true };
            return regexMask;
        }

        private const string WhiteSpaceFilter = "(?!\\s)";

    }  

}
