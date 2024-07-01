using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Site.Loaders
{
    public class VaultConfiguration
    {

        #region ctors

        public VaultConfiguration()
        {

        }


        public VaultConfiguration(string remoteUrl)
        {
            VaultRemoteUrl = remoteUrl;
        }

        public static VaultConfiguration GetFromConfiguration(IConfigurationRoot configuration) => new VaultConfiguration().MapFromConfiguration(configuration);

        #endregion ctors

        [Required]
        public string VaultRemoteUrl { get; set; }


        public bool IsValid()
        {

            if (string.IsNullOrEmpty(VaultRemoteUrl))
                return false;

            var uri = new Uri(VaultRemoteUrl);

            if (!uri.IsWellFormedOriginalString())
                return false;

            return true;

        }

    }


}
