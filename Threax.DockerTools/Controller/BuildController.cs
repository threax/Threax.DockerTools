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
        IProcessRunner processRunner
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

            processStartInfo.EnvironmentVariables.Add("DOCKER_BUILDKIT", "1");

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

            processRunner.Run(processStartInfo);

            return Task.CompletedTask;
        }
    }
}
