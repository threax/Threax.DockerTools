using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Threax.ProcessHelper;

namespace Threax.DockerTools.Services
{
    record ImageManager
    (
        IProcessRunnerFactory processRunnerFactory
    )
    : IImageManager
    {
        public string FindLatestImage(string image, string baseTag, string currentTag)
        {
            var processRunner = new JsonOutputProcessRunner(processRunnerFactory.Create(), true);

            //Get the tags from docker
            var startInfo = new ProcessStartInfo("docker") { ArgumentList = { "inspect", "--format={{json .RepoTags}}", $"{image}:{currentTag}" } };
            processRunner.Run(startInfo);
            var tags = processRunner.GetResult<List<String>>($"Error inspecting image '{image}'");

            //Remove any tags that weren't set by this software
            tags.Remove($"{image}:{currentTag}");
            var tagFilter = $"{image}:{baseTag}";
            tags = tags.Where(i => i.StartsWith(tagFilter)).ToList();
            tags.Sort(); //Docker seems to store these in order, but sort them by their names, the tags are date based and the latest will always be last

            var latestDateTag = tags.LastOrDefault();

            if (latestDateTag == null)
            {
                throw new InvalidOperationException($"Cannot find a tag in the format '{tagFilter}' on image '{image}'.");
            }

            return latestDateTag;
        }
    }
}
