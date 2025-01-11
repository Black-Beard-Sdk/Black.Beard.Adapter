using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;
using Bb.Configuration.Git;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;


namespace Bb.Logging.NLog
{

    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<Initializer>), LifeCycle = IocScopeEnum.Transiant)]
    [Priority(1)]
    public class ConfigLoader : IInjectBuilder<Initializer>
    {

        public string FriendlyName => typeof(ConfigLoader).Name;

        public Type Type => typeof(ConfigLoader);

        public bool CanExecute(Initializer context) => context.CanExecuteModule(FriendlyName);

        public bool CanExecute(object context) => CanExecute((Initializer)context);

        public object Execute(object context) => Execute((Initializer)context);

        public object Execute(Initializer context)
        {

            var cnx = Configuration.GetConnexionStringKeyValues(FriendlyName);
            var remoteUrl = cnx["url"];
            if (!string.IsNullOrEmpty(remoteUrl))
            {

                var userName = cnx.ContainsKey(_user) ? cnx[_user] : string.Empty;
                var email = cnx.ContainsKey(_email) ? cnx[_email] : string.Empty;
                var pwd = cnx.ContainsKey(_pwd) ? cnx[_pwd] : string.Empty;

                var git = new GitConfiguration(remoteUrl, userName, email, pwd);
                if (git.IsValid())
                {

                    var branch = cnx.ContainsKey(_branch) ? cnx[_branch] : string.Empty;
                    if (!string.IsNullOrEmpty(branch))
                        git.GitBranch = branch;

                    var folder = cnx.ContainsKey(_folder)
                        ? Environment.CurrentDirectory.Combine(cnx[_folder])
                        : Environment.CurrentDirectory.Combine("Config");

                    folder = folder.Combine("Current");

                    var loader = new ConfigurationLoader(git);

                    var dir = folder.AsDirectory();
                    dir.Refresh();
                    if (dir.Exists)
                    {
                        var branchName = loader.GetLocalBranchName(folder);
                        if (branch != branchName)
                        {
                            dir.DeleteFolderIfExists();
                            dir.Refresh();
                        }
                    }

                    loader.Refresh(folder);

                    ConfigurationFolder.AddDirectoryIfExists(loader.RepositoryLocal);

                }
            }

            return null;

        }

        public const string _user = "user";
        public const string _email = "email";
        public const string _pwd = "pwd";
        public const string _branch = "branch";
        public const string _folder = "folder";

        [Inject]
        public IConfiguration Configuration { get; set; }

    }

    public static class ConnexionReaderExtension
    {

        public static Dictionary<string, string> GetConnexionStringKeyValues(this IConfiguration self, string connexionStringName)
        {
            return self.GetConnectionString(connexionStringName).GetKeyValues();
        }


    }


}
