using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Threax.Pipelines.Core;
using Threax.ProcessHelper;

namespace Threax.DockerTools.Tasks
{
    record StopContainerTask
    (
        IProcessRunner processRunner
    ) 
    : IStopContainerTask
    {
        public void StopContainer(String name)
        {
            //It is ok if this fails, probably means it wasn't running
            processRunner.Run(new ProcessStartInfo("docker") { ArgumentList = { "rm", name, "--force" } });
        }
    }
}
