﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Threax.Pipelines.Core;
using Threax.ProcessHelper;

namespace Threax.DockerTools.Tasks
{
    record StopContainerTask
    (
        IProcessRunnerFactory processRunnerFactory
    ) 
    : IStopContainerTask
    {
        public void StopContainer(String name)
        {
            var processRunner = processRunnerFactory.Create();
            //It is ok if this fails, probably means it wasn't running
            processRunner.Run(new ProcessStartInfo("docker") { ArgumentList = { "rm", name, "--force" } });
        }
    }
}
