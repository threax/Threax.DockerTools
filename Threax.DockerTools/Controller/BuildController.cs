using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Threax.DockerBuildConfig;
using Threax.ProcessHelper;

namespace Threax.DockerTools.Controller
{
    record BuildController
    (
        BuildConfig buildConfig, 
        ILogger<BuildController> logger, 
        IShellRunner shellRunner,
        IProcessRunnerFactory processRunnerFactory
    ) 
    : IController
    {
        public Task Run()
        {
            var context = buildConfig.GetContext();
            var dockerFile = Path.GetFullPath(Path.Combine(context, buildConfig.Dockerfile ?? throw new InvalidOperationException($"Please provide {nameof(buildConfig.Dockerfile)} when using build.")));
            var image = buildConfig.ImageName;
            var buildTag = buildConfig.GetBuildTag();
            var currentTag = buildConfig.GetCurrentTag();

            var processStartInfo = new ProcessStartInfo("docker")
            {
                ArgumentList =
                {
                    "build", context,
                    "-f", dockerFile,
                    "-t", $"{image}:{buildTag}",
                    "-t", $"{image}:{currentTag}",
                    "--progress=plain"
                }
            };

            if (buildConfig.PullAllImages)
            {
                processStartInfo.ArgumentList.Add("--pull");
            }

            if(buildConfig.Args != null)
            {
                foreach(var arg in buildConfig.Args)
                {
                    processStartInfo.ArgumentList.Add("--build-arg");
                    processStartInfo.ArgumentList.Add($"{arg.Key}={arg.Value}");
                }
            }

            var processRunner = processRunnerFactory.Create();
            processRunner.Run(processStartInfo);

            return Task.CompletedTask;
        }
    }
}
