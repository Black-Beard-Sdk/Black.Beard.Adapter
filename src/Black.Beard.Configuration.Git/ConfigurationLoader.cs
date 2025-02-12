using LibGit2Sharp;
using Bb;
using System;
using LibGit2Sharp.Handlers;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;


namespace Bb.Configuration.Git
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


        public string GetLocalBranchName(string localFolder)
        {

            using (var repo = new Repository(localFolder))
            {

                var l = repo.Commits.Last();

                var b = repo.Branches
                    .Where(c => (!c.IsRemote && c.IsTracking))
                    .FirstOrDefault();
                if (b != null)
                {
                    var t1 = "origin/";
                    var t = b.TrackedBranch.FriendlyName;

                    if (t.StartsWith(t1))
                        t = t.Substring(t1.Length);

                    return t;

                }
            }

            return null;

        }

        public void GetStatus(string localFolder)
        {

            using (var repo = new Repository(localFolder))
            {

                foreach (Commit c in repo.Commits.Take(1))
                {

                    Console.WriteLine(string.Format("commit {0}", c.Id));

                    if (c.Parents.Count() > 1)
                    {
                        Console.WriteLine("Merge: {0}",
                            string.Join(" ", c.Parents.Select(p => p.Id.Sha.Substring(0, 7)).ToArray()));
                    }

                    Console.WriteLine(string.Format("Author: {0} <{1}>", c.Author.Name, c.Author.Email));
                    Console.WriteLine("Date:   {0}", c.Author.When.ToString(RFC2822Format, CultureInfo.InvariantCulture));
                    Console.WriteLine();
                    Console.WriteLine(c.Message);
                    Console.WriteLine();
                }
            }
        }


        public GitConfiguration GitConfiguration { get; set; }

        /// <summary>
        /// Refresh the git repository
        /// </summary>
        /// <param name="folder">target folder</param>
        /// <param name="branch">branch to clone</param>
        /// <returns></returns>
        public bool Refresh(string localFolder, string? branch = null)
        {
            var folder = localFolder.AsDirectory();
            return Refresh(folder, branch);
        }

        /// <summary>
        /// Clone the git repository
        /// </summary>
        /// <param name="folder">target folder</param>
        /// <param name="branch">branch to clone</param>
        /// <returns></returns>
        public bool Refresh(DirectoryInfo folder, string? branch = null)
        {

            if (!folder.Exists)
                folder.CreateFolderIfNotExists();

            var f = folder.FullName;

            var status = GitConfiguration.Initialized(folder.FullName);
            switch (status)
            {

                case GitStatus.NotInitialized:
                    return Clone(f, branch ?? GitConfiguration.GitBranch ?? "main");

                case GitStatus.Initialized:
                    return Pull(f);

                case GitStatus.FolderNotEmpty:
                case GitStatus.FolderNotCreated:
                default:
                    break;
            }

            return false;

        }

        #region private

        private bool Fetch(string localFolder)
        {
            try
            {

                string logMessage = string.Empty;
                using (var repo = new Repository(localFolder))
                {
                    FetchOptions options = GetFetchOptions();
                    foreach (Remote remote in repo.Network.Remotes)
                    {
                        IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                        Commands.Fetch(repo, remote.Name, refSpecs, options, logMessage);
                    }
                }

                if (string.IsNullOrEmpty(logMessage))
                    Console.WriteLine(logMessage);

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to pull : {ex.Message}");
            }

            return false;

        }

        private bool Pull(string localFolder)
        {
            try
            {
                using (var repo = new Repository(localFolder))
                {
                    var pullOptions = GetPullOptions();
                    var identity = new Identity(GitConfiguration.GitUserName, GitConfiguration.GitEmail);
                    var signature = new Signature(identity, DateTimeOffset.Now);
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
                Repository.Clone(GitConfiguration.GitRemoteUrl, localFolder, cloneOptions);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur est survenue : {ex.Message}");
            }

            return false;

        }

        private FetchOptions GetFetchOptions(string branch = "main")
        {
            FetchOptions options = new FetchOptions()
            {
                CredentialsProvider = GetCredential()
            };
            return options;
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
            options.FetchOptions.CredentialsProvider = GetCredential();
            return options;

        }

        private PullOptions GetPullOptions()
        {
            var options = new PullOptions()
            {
                FetchOptions = GetFetchOptions()
            };
            return options;
        }

        private CredentialsHandler GetCredential()
        {

            if (!GitConfiguration.HasPassword)
                return (_url, _user, _cred) => new LibGit2Sharp.DefaultCredentials();

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


        private const string RFC2822Format = "ddd dd MMM HH:mm:ss yyyy K";

    }
}
