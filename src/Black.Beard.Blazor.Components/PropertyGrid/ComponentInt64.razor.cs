using Microsoft.AspNetCore.Components;
using System.Security.Cryptography;

namespace Bb.PropertyGrid
{

    public partial class ComponentInt64
    {

        // @oninput="@Convert"
        public long Convert(string value)
        {
            if (long.TryParse(value, out long result))
                return result;
            return 0;
        }


    }

}
