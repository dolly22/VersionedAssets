# ASP.NET Core - VersionedAssets

VersionedAssets is middleware and taghelper for easier integration with CDN configured for origin pull. TagHelper
creates asset urls with content version hash burned in url (for cache busting). Middleware handles requests
comming from CDN and sets caching headers acordingly.

Following example assumes your CDN is configured with origin pull to base url
<code>http://[your.site]/static</code>.

TagHelper generated url: <code>http://[your.cdn]/[version-hash]/bundles/app.js</code>
Origin pull url: <code>http://[your.side]/static/[version-hash]/bundles/app.js</code>


## Startup.cs configuration

```c#
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging();
        services.AddMvc();     
        services.AddVersionedAssets();       
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddConsole(LogLevel.Debug);

        // respond to cdn origin pull asset requests in the form /static/[hash]/* 
        app.UseVersionedAssets()
            .WithUrlPrefix("//[your.cdn]")

        app.UseMvcWithDefaultRoute();
    }
}
```

## View configuration


```razor
@addTagHelper "*, AspNetCore.VersionedAssets"
<!DOCTYPE html>
<html lang="cs">
<head>
  <meta charset="UTF-8">
  <title>SPA</title>
</head>
<body>
  <script src="~/bundles/app.js" asset-version="FileVersion"></script>
</body>
</html>

```
