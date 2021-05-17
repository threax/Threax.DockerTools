using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Threax.DockerBuildConfig;
using Threax.ProcessHelper;

namespace Threax.DockerTools.Controller
{
    record CloneController
    (
        BuildConfig appConfig,
        ILogger<CloneController> logger,
        IProcessRunner processRunner
    ) : IController
    {
        public Task Run()
        {
            var clonePath = Path.GetFullPath(appConfig.ClonePath);
            var repo = appConfig.RepoUrl;

            if (Directory.Exists(appConfig.ClonePath))
            {
                logger.LogInformation($"Pulling changes to {clonePath}");
                var startInfo = new ProcessStartInfo("git") { ArgumentList = { "pull" }, WorkingDirectory = Path.GetFullPath(appConfig.ClonePath) };
                var exitCode = processRunner.Run(startInfo);
                if (exitCode != 0)
                {
                    throw new InvalidOperationException("Error during pull.");
                }
            }
            else
            {
                logger.LogInformation($"Cloning {repo} to {clonePath}");
                var startInfo = new ProcessStartInfo("git") { ArgumentList = { "clone", repo, clonePath } };
                var exitCode = processRunner.Run(startInfo);
                if (exitCode != 0)
                {
                    throw new InvalidOperationException("Error during clone.");
                }
            }

            return Task.CompletedTask;
        }
    }
}
