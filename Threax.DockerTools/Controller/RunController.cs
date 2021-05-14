using System.Threading.Tasks;
using Threax.DockerTools.Tasks;

namespace Threax.DockerTools.Controller
{
    class RunController : IController
    {
        private readonly IRunTask runTask;

        public RunController(IRunTask runTask)
        {
            this.runTask = runTask;
        }

        public Task Run()
        {
            return runTask.Run();
        }
    }
}
