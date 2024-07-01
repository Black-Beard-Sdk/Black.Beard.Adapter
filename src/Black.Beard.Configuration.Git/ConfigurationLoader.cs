using LibGit2Sharp;
using Bb;
using System;
using LibGit2Sharp.Handlers;


namespace Black.Beard.Configuration.Git
{


    public class ConfigurationLoader
    {

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationLoader"/> class.
        /// </summary>
        public ConfigurationLoader()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationLoader"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        public ConfigurationLoader(GitConfiguration configuration)
        {
            GitConfiguration = configuration;
        }

        #endregion ctor

        public GitConfiguration GitConfiguration { get; set; }

        public bool Refresh(string localFolder, string branch = "main")
        {

            var folder = localFolder.AsDirectory();
            if (!folder.Exists)
                folder.CreateFolderIfNotExists();

            var status = GitConfiguration.Initialized(localFolder);
            switch (status)
            {

                case GitStatus.NotInitialized:
                    return Clone(localFolder, branch);

                case GitStatus.Initialized:
                    return Pull(localFolder);

                case GitStatus.FolderNotEmpty:
                case GitStatus.FolderNotCreated:
                default:
                    break;
            }

            return false;

        }

        #region private

        private bool Pull(string localFolder)
        {
            try
            {
                using (var repo = new Repository(localFolder))
                {
                    var pullOptions = GetPullOptions();
                    var signature = new Signature(new Identity(GitConfiguration.GitUserName, GitConfiguration.GitEmail), DateTimeOffset.Now);
                    Commands.Pull(repo, signature, pullOptions);
                }

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to pull : {ex.Message}");
            }

            return false;

        }

        private bool Clone(string localFolder, string branch)
        {
            try
            {
                var cloneOptions = GetCloneOptions(branch);
                var repLocal = Repository.Clone(GitConfiguration.GitRemoteUrl, localFolder, cloneOptions);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur est survenue : {ex.Message}");
            }

            return false;

        }

        private CloneOptions GetCloneOptions(string branch = "main")
        {

            var options = new CloneOptions()
            {
                BranchName = branch,
                //Checkout = true,
                RecurseSubmodules = true,
                IsBare = false,
            };

            if (GitConfiguration.HasPassword)
                options.FetchOptions.CredentialsProvider = GetCredential();
            else
                options.FetchOptions.CredentialsProvider = (_url, _user, _cred) => new LibGit2Sharp.DefaultCredentials();

            return options;

        }

        private PullOptions GetPullOptions()
        {

            var options = new PullOptions();
            options.FetchOptions = new FetchOptions();

            if (GitConfiguration.HasPassword)
                options.FetchOptions.CredentialsProvider = GetCredential();
            else
                options.FetchOptions.CredentialsProvider = (_url, _user, _cred) => new LibGit2Sharp.DefaultCredentials();

            return options;

        }

        private CredentialsHandler GetCredential()
        {
            return (_url, _user, _cred) =>
            {
                return new UsernamePasswordCredentials()
                {
                    Username = GitConfiguration.GitUserName,
                    Password = GitConfiguration.GitPassword
                };
            };
        }

        #endregion private

    }
}
