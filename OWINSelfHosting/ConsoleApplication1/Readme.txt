http://www.asp.net/web-api

Attribute Routing in ASP.NET Web API 2
http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2

ASP.NET - Getting Started with the Katana Project
https://msdn.microsoft.com/en-us/magazine/dn451439.aspx

Creating a simple REST like service with OWIN – Open Web Server Interface
http://weblogs.asp.net/fredriknormen/creating-a-simple-rest-like-service-with-owin-open-web-server-interface

Strategies for Fault-Tolerant Computing
https://msdn.microsoft.com/en-us/library/bb742373.aspx

Introduction to REST and .net Web API
http://blogs.msdn.com/b/martinkearn/archive/2015/01/05/introduction-to-rest-and-net-web-api.aspx

http://www.asp.net/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api

<send>
They look at examples and w/o theory they learn nothing
http://blog.deming.org/2015/09/people-copy-examples-and-wonder-why-they-dont-succeed

Seven Agile Best Practices
http://www.estherderby.com/2015/10/seven-agile-best-practices.html

We are not fucking competent
http://arlobelshee.com/we-are-not-fucking-competent

Passing multiple POST parameters to Web API Controller Methods
https://weblog.west-wind.com/posts/2012/May/08/Passing-multiple-POST-parameters-to-Web-API-Controller-Methods

How should I pass multiple parameters to an ASP.Net Web API GET?
http://stackoverflow.com/questions/10937524/how-should-i-pass-multiple-parameters-to-an-asp-net-web-api-get

Web API unsupported media type when uploading - charset not passed
http://stackoverflow.com/questions/24998144/web-api-unsupported-media-type-when-uploading-charset-not-passed
</send>

--Nuget log:
Each package is licensed to you by its owner. Microsoft is not responsible for, nor does it grant any licenses to, third-party packages. Some packages may include dependencies which are governed by additional licenses. Follow the package source (feed) URL to determine any dependencies.

Package Manager Console Host Version 3.1.1.0

Type 'get-help NuGet' to see all available NuGet commands.

PM> Install-Package Microsoft.AspNet.WebApi.OwinSelfHost
Attempting to gather dependencies information for package 'Microsoft.AspNet.WebApi.OwinSelfHost.5.2.3' with respect to project 'ConsoleApplication1', targeting '.NETFramework,Version=v4.0'
Attempting to resolve dependencies for package 'Microsoft.AspNet.WebApi.OwinSelfHost.5.2.3' with DependencyBehavior 'Lowest'
Resolving actions to install package 'Microsoft.AspNet.WebApi.OwinSelfHost.5.2.3'
Resolved actions to install package 'Microsoft.AspNet.WebApi.OwinSelfHost.5.2.3'
Install failed. Rolling back...
Package 'Microsoft.AspNet.WebApi.Client 5.2.3' does not exist in project 'ConsoleApplication1'
Package 'Microsoft.AspNet.WebApi.Client 5.2.3' does not exist in folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Install-Package : Could not install package 'Microsoft.AspNet.WebApi.Client 5.2.3'. You are trying to install this package into a project that targets '.NETFramework,Version=v4.0', but
 the package does not contain any assembly references or content files that are compatible with that framework. For more information, contact the package author.
At line:1 char:16
+ Install-Package <<<<  Microsoft.AspNet.WebApi.OwinSelfHost
    + CategoryInfo          : NotSpecified: (:) [Install-Package], Exception
    + FullyQualifiedErrorId : NuGetCmdletUnhandledException,NuGet.PackageManagement.PowerShellCmdlets.InstallPackageCommand
 
PM> Install-Package Microsoft.AspNet.WebApi.OwinSelfHost
Attempting to gather dependencies information for package 'Microsoft.AspNet.WebApi.OwinSelfHost.5.2.3' with respect to project 'ConsoleApplication1', targeting '.NETFramework,Version=v4.5'
Attempting to resolve dependencies for package 'Microsoft.AspNet.WebApi.OwinSelfHost.5.2.3' with DependencyBehavior 'Lowest'
Resolving actions to install package 'Microsoft.AspNet.WebApi.OwinSelfHost.5.2.3'
Resolved actions to install package 'Microsoft.AspNet.WebApi.OwinSelfHost.5.2.3'
Adding package 'Microsoft.Owin.Host.HttpListener.2.0.2' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.Owin.Host.HttpListener.2.0.2' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.Owin.Host.HttpListener.2.0.2' to 'packages.config'
Successfully installed 'Microsoft.Owin.Host.HttpListener 2.0.2' to ConsoleApplication1
Adding package 'Newtonsoft.Json.6.0.4' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Newtonsoft.Json.6.0.4' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Newtonsoft.Json.6.0.4' to 'packages.config'
Executing script file 'C:\design\github_spike\spike\OWINSelfHosting\packages\Newtonsoft.Json.6.0.4\tools\install.ps1'
Successfully installed 'Newtonsoft.Json 6.0.4' to ConsoleApplication1
Adding package 'Microsoft.AspNet.WebApi.Client.5.2.3' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.AspNet.WebApi.Client.5.2.3' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.AspNet.WebApi.Client.5.2.3' to 'packages.config'
Successfully installed 'Microsoft.AspNet.WebApi.Client 5.2.3' to ConsoleApplication1
Adding package 'Microsoft.AspNet.WebApi.Core.5.2.3' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.AspNet.WebApi.Core.5.2.3' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.AspNet.WebApi.Core.5.2.3' to 'packages.config'
Successfully installed 'Microsoft.AspNet.WebApi.Core 5.2.3' to ConsoleApplication1
Adding package 'Owin.1.0.0' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Owin.1.0.0' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Owin.1.0.0' to 'packages.config'
Successfully installed 'Owin 1.0.0' to ConsoleApplication1
Adding package 'Microsoft.Owin.2.0.2' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.Owin.2.0.2' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.Owin.2.0.2' to 'packages.config'
Successfully installed 'Microsoft.Owin 2.0.2' to ConsoleApplication1
Adding package 'Microsoft.AspNet.WebApi.Owin.5.2.3' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.AspNet.WebApi.Owin.5.2.3' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.AspNet.WebApi.Owin.5.2.3' to 'packages.config'
Successfully installed 'Microsoft.AspNet.WebApi.Owin 5.2.3' to ConsoleApplication1
Adding package 'Microsoft.Owin.Hosting.2.0.2' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.Owin.Hosting.2.0.2' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.Owin.Hosting.2.0.2' to 'packages.config'
Successfully installed 'Microsoft.Owin.Hosting 2.0.2' to ConsoleApplication1
Adding package 'Microsoft.AspNet.WebApi.OwinSelfHost.5.2.3', which only has dependencies, to project 'ConsoleApplication1'.
Adding package 'Microsoft.AspNet.WebApi.OwinSelfHost.5.2.3' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.AspNet.WebApi.OwinSelfHost.5.2.3' to folder 'C:\design\github_spike\spike\OWINSelfHosting\packages'
Added package 'Microsoft.AspNet.WebApi.OwinSelfHost.5.2.3' to 'packages.config'
Successfully installed 'Microsoft.AspNet.WebApi.OwinSelfHost 5.2.3' to ConsoleApplication1
PM> 