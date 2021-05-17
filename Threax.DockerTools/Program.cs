using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Threax.ConsoleApp;
using Threax.DeployConfig;
using Threax.DockerBuildConfig;
using Threax.DockerTools.Controller;
using Threax.DockerTools.Services;
using Threax.DockerTools.Tasks;
using Threax.Extensions.Configuration.SchemaBinder;
using Threax.Pipelines.Core;
using Threax.ProcessHelper;

namespace Threax.DockerTools
{
    class Program
    {
        public static Task<int> Main(string[] args)
        {
            var jsonConfigPath = args.Length > 1 ? args[1] : "unknown.json";

            string command = null;
            if (args.Length > 0)
            {
                command = args[0];
            }

            return AppHost
            .Setup<IController, HelpController>(command, services =>
            {
                services.AddSingleton<IArgsProvider>(s => new ArgsProvider(args));

                services.AddScoped<SchemaConfigurationBinder>(s =>
                {
                    var configBuilder = new ConfigurationBuilder();
                    configBuilder.AddJsonFile(jsonConfigPath);
                    return new SchemaConfigurationBinder(configBuilder.Build());
                });

                services.AddScoped<IConfigFileProvider>(s => new ConfigFileProvider(jsonConfigPath));

                services.AddScoped<IOSHandler>(s =>
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        return new OSHandlerWindows();
                    }
                    return new OSHandlerUnix(s.GetRequiredService<IProcessRunner>());
                });

                services.AddScoped<BuildConfig>(s =>
                {
                    var config = s.GetRequiredService<SchemaConfigurationBinder>();
                    var appConfig = new BuildConfig(jsonConfigPath);
                    config.Bind("Build", appConfig);
                    appConfig.Validate();
                    return appConfig;
                });

                services.AddScoped<DeploymentConfig>(s =>
                {
                    var config = s.GetRequiredService<SchemaConfigurationBinder>();
                    var deployConfig = new DeploymentConfig(jsonConfigPath);
                    config.Bind("Deploy", deployConfig);
                    deployConfig.Validate();
                    return deployConfig;
                });

                services.AddLogging(o =>
                {
                    o.AddConsole().AddSimpleConsole(co =>
                    {
                        co.IncludeScopes = false;
                        co.SingleLine = true;
                    });
                });

                services.AddThreaxProcessHelper(o =>
                {
                    //o.IncludeLogOutput = false;
                    //o.DecorateProcessRunner = r => new SpyProcessRunner(r)
                    //{
                    //    Events = new ProcessEvents()
                    //    {
                    //        ErrorDataReceived = (o, e) => { if (e.DataReceivedEventArgs.Data != null) Console.WriteLine(e.DataReceivedEventArgs.Data); },
                    //        OutputDataReceived = (o, e) => { if (e.DataReceivedEventArgs.Data != null) Console.WriteLine(e.DataReceivedEventArgs.Data); },
                    //    }
                    //};
                });

                services.AddScoped<ICreateBase64SecretTask, CreateBase64SecretTask>();
                services.AddScoped<ICreateCertificateTask, CreateCertificateTask>();
                services.AddScoped<ILoadTask, LoadTask>();
                services.AddScoped<IRunTask, RunTask>();
                services.AddScoped<IStopContainerTask, StopContainerTask>();
                services.AddScoped<IImageManager, ImageManager>();
            })
            .Run(c => c.Run());
        }
    }
}
