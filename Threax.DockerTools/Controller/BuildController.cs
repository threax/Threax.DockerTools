using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Threax.DockerBuildConfig;
using Threax.Pipelines.Core;
using Threax.ProcessHelper;

namespace Threax.DockerTools.Controller
{
    record BuildController
    (
        BuildConfig buildConfig, 
        ILogger<BuildController> logger, 
        IShellRunner shellRunner
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

            List<FormattableString> command = new List<FormattableString>() { $"docker build {context} -f {dockerFile} -t {image}:{buildTag} -t {image}:{currentTag} --progress=plain" };

            if (buildConfig.PullAllImages)
            {
                command.Add($" --pull");
            }

            if(buildConfig.Args != null)
            {
                foreach(var arg in buildConfig.Args)
                {
                    command.Add($" --build-arg {arg.Key}={arg.Value}");
                }
            }

            shellRunner.RunProcessVoid(command, invalidExitCodeMessage: "An error occured during the docker build.");

            return Task.CompletedTask;
        }
    }
}
