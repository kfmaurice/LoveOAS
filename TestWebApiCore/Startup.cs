using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Dynamite.LoveOAS;
using Dynamite.LoveOAS.NetCore.Filters;
using Dynamite.LoveOAS.Discovery;

namespace TestWebApiCore
{
  public class Startup
  {
    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
          .AddEnvironmentVariables();
      Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddTransient<IExtractor, Dynamite.LoveOAS.NetCore.Extractor>();
      services.AddTransient<IDiscoverer, Discoverer>();
      services.AddSingleton(typeof(ISettings), new Settings {
        UseAbsoluteUrl = true,
        HandleOnlyMarkedApis = true,
        TreatCollectionAsPayload = true,
        CheckAuthorization = true // value doesn't matter since this is not handled by the plugin yet
      });
      //services.AddTransient<IAuthorization, Authorization>(); // default processor instantiate this one on its own
      //services.AddTransient<IRouteSelector, RouteSelector>(); // default processor instantiate this one on its own
      //services.AddTransient<IParser, Parser>(); // default processor instantiate this one on its own
      services.AddTransient<ISerializer, JsonOutputSerializer>();
      services.AddTransient<IOrchestrator>(provider => 
        new Orchestrator(extractor: provider.GetService<IExtractor>(), discoverer: provider.GetService<IDiscoverer>(), serializer: provider.GetService<ISerializer>(), settings: provider.GetService<ISettings>()));

      // Add framework services.
      services.AddMvc(options =>
      {
        options.Filters.Add(typeof(LoveOASFilterAttribute));
      });

      services.BuildServiceProvider().GetService<IOrchestrator>().Setup(namespaces: nameof(TestWebApiCore));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      app.UseMvc(routes => {
        routes.MapRoute(
          name: "default",
          template: "{controller=People}/{action=Get}/{id?}");
      });
    }
  }
}
