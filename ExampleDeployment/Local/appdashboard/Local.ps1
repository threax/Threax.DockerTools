$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path

&"$scriptPath/../tools/bin/Threax.DockerTools" clone $scriptPath/appsettings.json
&"$scriptPath/../tools/bin/Threax.DockerTools" build $scriptPath/appsettings.json
&"$scriptPath/../tools/bin/Threax.DockerTools" run $scriptPath/appsettings.json
&"$scriptPath/../tools/bin/Threax.DockerTools" exec $scriptPath/../id/appsettings.json SetupAppDashboard -l secret appdashboard/JwtAuth__ClientSecret $scriptPath/appsettings.json JwtAuth__ClientSecret