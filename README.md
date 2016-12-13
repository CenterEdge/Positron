# Positron
HTML 5 UI system for .Net, using Chromium and ASP.Net MVC 6

[![Build status](https://ci.appveyor.com/api/projects/status/akifm35uejxtawv1/branch/master?svg=true)](https://ci.appveyor.com/project/brantburnett/positron/branch/master)

## Overview

[Electron](http://electron.atom.io/) is a great tool for writing desktop applications with modern HTML5 user interfaces. But what if
you're a .Net developer? What if you already have a .Net application and want to layer a modern HTML5 user interface on top of it?

Enter Positron. It's acts as in-process middleware between ASP.Net MVC 6 and the Chromium Embedded Framework. It hosts a fully featured
MVC application in process, hosts a Chromium browser inside a plain WPF window, and wires the two together in memory (no network or HTTP
stack involved to slow down performance).

## The MVC Server

Did you know that the new ASP.Net Core isn't just for [.Net Core](https://www.microsoft.com/net/core)? We know, they both have "Core" in
their name. But ASP.Net Core actually targets the [.Net Standard](https://docs.microsoft.com/en-us/dotnet/articles/standard/library).
This means that it can also be used within good ole .Net Framework applications.

Positron works by operating a custom in-process server. You start it up with a WebHostBuilder just like you do a
[regular ASP.Net Core application](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/hosting). But instead of using Kestrel,
we use a Positron in-process server instead

```csharp
var builder = new WebHostBuilder()
    .UseEnvironment("Development")
    .ConfigureLogging(factory =>
    {
        factory.AddConsole();
    })
    .UsePositronServer()
    .UseStartup<Startup>();
```

Within your Startup class, instead of registering MVC you'll register Positron. It will also register MVC for you, but alters some of
the standard MVC behaviors to fit the Positron paradigm.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            // Register Positron and  MVC services
            .AddPositronServer()
            // Make sure all DLL files that reference MVC and start with Positron in the name are loaded as application parts
            // Name filtering should be adjusted to match your application
            .AddApplicationParts(p => Path.GetFileName(p).StartsWith("Positron"));
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStatusCodePages();
        
        // Register the Positron handlers.  This registers MVC using Positron's route model (this can be adjusted).
        // It also register middleware to handle static resources.
        app.UsePositron();
    }
}
```

To build your application, simply add Controllers and Razor views. In order to support larger applications that span multiple
assemblies, the default route is registered as `{assembly}/{controller=Home}/{action=Index}/{id?}`. This isolates controllers from
each assembly to prevent name collision. You can customize the `{assembly}` portion of the URL to be something other than the assembly
name using an assembly attribute.

```csharp
[assembly: PositronRouteIdentifier("CustomUrlSegment")]
```

Positron serves static content as resources embedded in the assembly, so you don't need to worry about packaging lots of files. This
also helps provide security, making tampering with your resources more difficult. By default, views, images, and fonts are included as
resources automatically (at `{assembly}/path/to/file.ext`). This is adjustable via MSBuild properties.

For CSS and JS, we assume that you may want to add these after preprocessing via Grunt or Gulp.  So we don't add these files
automatically. For simple applications, you can just specify the build action as `Resource` in the Properties window in Visual Studio
and the file will be included.

## The Positron Window

The next step is actually opening a Positron window containing a Chromium browser. First, you must configure the UI during application
startup.

```csharp
// Start initializing a WPF application
var app = new App();
app.InitializeComponent();

// Build the Positron UI
var uiBuilder = new PositronUiBuilder()
    .SetWebHost(webHost)
    // Want to use Chrome tools for dev?  Specify a port then browse to http://localhost:xxx after startup!
    .UseDebugPort(8080)
    .ConfigureServices(services =>
    {
        // This is how you register .Net objects to be available via Javascript
        services.AddSingleton<IGlobalScriptObject, TestScriptObject>();
    });

var windowHandler = uiBuilder.Build();

// Create the WPF window and start the app!
app.Run(windowHandler.CreateWindow((Window)null, // no parent window 
  "http://positron/Positron.Application"));
```

Note the URL format. Positron URLs always start with `http://positron/`. After that the default route segment is the assembly name.
The URL `http://positron/Positron.Application` will run the Index action on the Home controller within the Positron.Application
assembly.

Want to run HTML UIs as an extension for an existing WPF application? You can do that, too, just save the `IWindowHandler` returned by
`uiBuilder.Build()` and use it from anywhere. Add the Positron.UI.WinFormsInterop package and you can use WinForms as the parent
window as well.

**Note:** Because the Positron.UI package depends on CefSharp and Chromium, we recommend only installing it in your application
executable, or some library towards the end of your build order. Assemblies that are serving content with controllers only need a
reference to the Positron.Server package.

**Note:** CefSharp requires some [special considerations](https://github.com/cefsharp/CefSharp/pull/1753) to support the AnyCPU build
mode. We haven't implemented these yet in Positron, so just make sure you specify x86 as the Platform for your project. CefSharp
won't let you just set it in build settings, it must be the actual Platform name within your solution configuration.

## Security Considerations

Currently, the Chromium browser is restricted to only allow content from within Positron.  It can't make AJAX requests or load
resources from the internet or the LAN.  We plan on offering ways to reduce this restriction in the future.

All views are embedded as resources in the assembly.  They are specifically blocked from being downloaded raw by Chromium (just like
in regular MVC applications).

## Who We Are

[CenterEdge Software](http://centeredgesoftware.com/) is a leading software provider in the entertainment industry. We provide a full
suite of integrated products for managing and operation entertainment venues, from trampoline parks to waterparks to amusement parks.

We see HTML5 based user interfaces as the future of our product, allowing us to deliver a modern user experience with the least
investment in time and resources. We're creating the Positron system as a means to deliver this, while still maintaining our
client/server model for our Point of Sale applications. This will let us utilize our existing .Net business logic as well as necessary
low-level hardware interfaces, while still using modern technologies like [React](https://facebook.github.io/react/) to enhance our
user experience.

After due consideration, we have recognized that this need might be reflective of a need throughout the .Net community. While WPF is a
powerful user interface system, finding developers with a WPF skillset is both difficult and expensive. Our hope is that we can provide
this solution to the rest of the community, and garner feedback and improvements that will help both CenterEdge and the rest of the
community.

## Contributing

Positron is an open source project curated by CenterEdge Software. We welcome community input and support. If you'd like to contribute,
please fork the repo, submit a pull request, and we'd be happy to review it. If you just want to submit a suggestion or a bug, please
[file an issue](https://github.com/CenterEdge/Positron/issues).

## Legal Stuff

Positron is copyright &copy; 2016 Pathfinder Software, LLC. All Rights Reserved.

Positron is licensed for use under the [Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0.html).
