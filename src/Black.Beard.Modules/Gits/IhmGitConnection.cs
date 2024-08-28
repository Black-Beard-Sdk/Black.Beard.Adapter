using Bb.ComponentDescriptors;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Security.Permissions;

namespace Bb.Configuration.Git
{


    [Mapper(typeof(IhmGitConnection))]
    public class IhmGitConnection
        : IMapper<IhmGitConnection, GitConnection>
        , IMapper<GitConnection, IhmGitConnection>
    {

        /// <summary>
        /// the url of the git repository
        /// </summary>
        [Required, Url]
        public string GitRemoteUrl { get; set; }

        /// <summary>
        /// the user name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// the user email
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// the user password
        /// </summary>
        [PasswordPropertyText]
        public string Password { get; set; }

        /// <summary>
        /// the branch to use. by default the main branch is used.
        /// </summary>
        public string Branch { get; set; }


        #region IMapper

        object? IMapper.MapTo(object source, Type targetType, object? target)
        {

            if (targetType == typeof(GitConnection) 
                && MapperAttribute.TryToMap<IhmGitConnection, GitConnection>(this, source, ref target))
                return target;

            else if (targetType == typeof(IhmGitConnection) 
                && MapperAttribute.TryToMap<GitConnection, IhmGitConnection>(this, source, ref target))
                return target;

            throw new System.Exception();

        }

        public GitConnection MapTo(IhmGitConnection source, GitConnection? target)
        {

            if (target == null)
                target = new GitConnection();

            if (target.GitRemoteUrl != source.GitRemoteUrl)
                target.GitRemoteUrl = source.GitRemoteUrl;

            if (target.GitUserName != source.Username)
                target.GitUserName = source.Username;

            if (target.GitPassword != source.Password)
                target.GitPassword = source.Password;

            if (target.GitBranch != source.Branch)
                target.GitBranch = source.Branch;

            if (target.GitEmail != source.Email)
                target.GitEmail = source.Email;

            return target;

        }

        public IhmGitConnection MapTo(GitConnection source, IhmGitConnection? target)
        {

            if (target == null)
                target = new IhmGitConnection();

            if (target.GitRemoteUrl != source.GitRemoteUrl)
                target.GitRemoteUrl = source.GitRemoteUrl;

            if (target.Username != source.GitUserName)
                target.Username = source.GitUserName;

            if (target.Password != source.GitPassword)
                target.Password = source.GitPassword;

            if (target.Branch != source.GitBranch)
                target.Branch = source.GitBranch;

            if (target.Email != source.GitEmail)
                target.Email = source.GitEmail;

            return target;

        }

        #endregion IMapper

    }

}




//var pwd = _configuration["gitPassword"];
//var securePwd = new SecureString();
//foreach (char c in pwd)
//    securePwd.AppendChar(c);
//securePwd.MakeReadOnly(); // Marque la chaîne comme étant en lecture seule
