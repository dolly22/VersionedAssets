# ASP.NET Core - CdnAssets

CndAssets is middleware and taghelper for easier integration with CDN configured for origin pull. TagHelper
creates asset urls with content version hash burned in url (for cache busting). Middleware handles requests
comming from CDN and sets caching headers acordingly.

Following example assumes your CDN is configured with origin pull to base url
<code>http://[your.site]/cdn</code>.

TagHelper generated url: <code>http://[your.cdn]/[version-hash]/static/app.js</code>
Origin pull url: <code>http://[your.side]/cdn/[version-hash]/static/app.js</code>


## Startup.cs configuration

```c#
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging();
        services.AddMvc();            
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddConsole(LogLevel.Debug);

        app.UseIISPlatformHandler();

        // respond to cdn origin pull asset requests in the form /cdn/[hash]/* 
        app.UseCdnAssets("/cdn");

        app.UseMvcWithDefaultRoute();
    }

    // Entry point for the application.
    public static void Main(string[] args) => WebApplication.Run<Startup>(args);
}
```

## View configuration


```razor
@addTagHelper "*, AspNetCore.CdnAssets"
@{
    var cdnUrl = "http://[your.cdn].azureedge.net";
}
<!DOCTYPE html>
<html lang="cs">
<head>
  <meta charset="UTF-8">
  <title>SPA</title>
</head>
<body>
  <script src="~/static/app.js" asp-cdn-asset="@cdnUrl"></script>
</body>
</html>

```
