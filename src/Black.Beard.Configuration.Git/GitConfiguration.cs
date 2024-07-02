using Bb;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;

namespace Bb.Configuration.Git
{

    /// <summary>
    /// Configuration for using git
    /// </summary>
    public class GitConfiguration
    {

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="GitConfiguration"/> class.
        /// </summary>
        public GitConfiguration()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GitConfiguration"/> class.
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public GitConfiguration(string remoteUrl, string userName, string email, string password)
        {
            GitRemoteUrl = remoteUrl;
            GitUserName = userName;
            GitEmail = email;
            GitPassword = password;
        }

        /// <summary>
        /// Return configuration mapped from environment
        /// </summary>
        /// <returns></returns>
        public static GitConfiguration GetFromEnvironment() => new GitConfiguration().MapFromEnvironment();

        public static GitConfiguration GetFromConfiguration(IConfigurationRoot configuration) => new GitConfiguration().MapFromConfiguration(configuration);


        #endregion ctors

        /// <summary>
        /// the url of the git repository
        /// </summary>
        [Required]
        public string GitRemoteUrl { get; set; }

        /// <summary>
        /// the user name
        /// </summary>
        public string GitUserName { get; set; }

        /// <summary>
        /// the user email
        /// </summary>
        public string GitEmail { get; set; }

        /// <summary>
        /// the user password
        /// </summary>
        public string GitPassword { get; set; }

        /// <summary>
        /// the branch to use. by default the main branch is used.
        /// </summary>
        public string GitBranch { get; set; } = "main";

        /// <summary>
        /// Return true if the password is set
        /// </summary>
        public bool HasPassword => !string.IsNullOrEmpty(GitPassword);

        /// <summary>
        /// Return true if the configuration is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {

            if (string.IsNullOrEmpty(GitRemoteUrl))
                return false;

            var uri = new Uri(GitRemoteUrl);

            if (!uri.IsWellFormedOriginalString())
                return false;

            return true;

        }

        /// <summary>
        /// return the status of the git repository
        /// </summary>
        /// <param name="localRoot"></param>
        /// <returns></returns>
        public GitStatus Initialized(string localRoot)
        {

            var t0 = localRoot.AsDirectory();
            t0.Refresh();
            if (!t0.Exists)
                return GitStatus.FolderNotCreated;

            var notEmpty = t0.GetFiles().Length > 0 || t0.GetDirectories().Length > 1;

            var t1 = localRoot.Combine(".git").AsDirectory();
            t1.Refresh();
            var e = t1.Exists;

            if (!e && notEmpty)
                return GitStatus.FolderNotEmpty;

            if (e)
                return GitStatus.Initialized;

            return GitStatus.NotInitialized;

        }

    }

    public enum GitStatus
    {
        FolderNotCreated,
        FolderNotEmpty,
        Initialized,
        NotInitialized
    }

}




//var pwd = _configuration["gitPassword"];
//var securePwd = new SecureString();
//foreach (char c in pwd)
//    securePwd.AppendChar(c);
//securePwd.MakeReadOnly(); // Marque la chaîne comme étant en lecture seule
