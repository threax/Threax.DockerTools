using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Threax.ConsoleApp;
using Threax.DeployConfig;
using Threax.DockerTools.Tasks;
using Threax.ProcessHelper;

namespace Threax.DockerTools.Controller
{
    record BackupController
    (
        DeploymentConfig deploymentConfig,
        ILogger<RunController> logger,
        IProcessRunnerFactory processRunnerFactory,
        IRunTask runTask,
        IArgsProvider argsProvider,
        IStopContainerTask stopContainerTask
    )
    : IController
    {

        public Task Run()
        {
            int exitCode;
            var args = argsProvider.Args;
            var restart = !args.Contains("norestart");
            var processRunner = processRunnerFactory.Create();

            try
            {
                //Get source path
                var fullDataPath = Path.GetFullPath(deploymentConfig.AppDataBasePath);
                if (!Directory.Exists(fullDataPath))
                {
                    throw new InvalidOperationException($"Cannot find source directory '{fullDataPath}'. Skipping backup.");
                }

                var dataParentPath = Path.GetFullPath(Path.Combine(fullDataPath, ".."));
                var dataFolder = Path.GetFileName(fullDataPath);

                //Get backup destination path
                var backupPath = deploymentConfig.DeploymentBasePath;
                var userProvidedPath = deploymentConfig.BackupDataPath;
                if (!String.IsNullOrEmpty(userProvidedPath))
                {
                    backupPath = Path.GetFullPath(Path.Combine(backupPath, userProvidedPath));
                }
                else
                {
                    backupPath = Path.GetFullPath(Path.Combine(backupPath, "backup"));
                }
                backupPath = Path.Combine(backupPath, deploymentConfig.Name);
                if (!Directory.Exists(backupPath))
                {
                    Directory.CreateDirectory(backupPath);
                }
                backupPath = Path.Combine(backupPath, $"{deploymentConfig.Name}-{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.tar.gz");

                //Do backup
                logger.LogInformation($"Backing up data folder '{fullDataPath}' to '{backupPath}'");
                stopContainerTask.StopContainer(deploymentConfig.Name);

                var startInfo = new ProcessStartInfo("tar") { ArgumentList = { "cvpzf", backupPath, dataFolder }, WorkingDirectory = dataParentPath };
                exitCode = processRunner.Run(startInfo);

                if (exitCode != 0)
                {
                    throw new InvalidOperationException("Error running tar command. No backup performed.");
                }
            }
            catch (Exception ex)
            {
                if (restart)
                {
                    logger.LogError(ex, $"{ex.GetType().Name} while backing up data.{Environment.NewLine}Message: '{ex.Message}'.");
                }
                else
                {
                    throw;
                }
            }

            if (restart)
            {
                return runTask.Run();
            }
            else
            {
                logger.LogInformation($"Backup created with norestart option. Deployment '{deploymentConfig.Name}' needs to be restarted manually.");
                return Task.FromResult(0);
            }
        }
    }
}
