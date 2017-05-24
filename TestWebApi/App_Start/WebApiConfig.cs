using System.Web.Http;

using Newtonsoft.Json.Serialization;

using Dynamite.LoveOAS.Discovery;
using Dynamite.LoveOAS.Filters;

namespace TestWebApi
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // Web API configuration and services
      
      var orchestrator = new Orchestrator(settings: WebApiApplication.Settings);

      config.Filters.Add(new LoveOasFilter(orchestrator));

      // Web API routes
      config.MapHttpAttributeRoutes();

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );

      config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
      config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;

      // boot
      orchestrator.Setup(namespaces: nameof(TestWebApi));
    }
  }
}
