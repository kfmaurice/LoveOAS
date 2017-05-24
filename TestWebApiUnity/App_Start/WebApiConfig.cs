using System.Web.Http;
using TestWebApiUnity.App_Start;

using Dynamite.LoveOAS.Discovery;
using Dynamite.LoveOAS.Filters;
using Newtonsoft.Json.Serialization;

namespace TestWebApiUnity
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // Web API configuration and services

      // using unity to call default instance
      var orchestrator = UnityConfig.GetConfiguredContainer().Resolve(typeof(Orchestrator), "default") as Orchestrator;

      // add filter for web api controller i.e. ApiController
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
      orchestrator.Setup(namespaces: nameof(TestWebApiUnity));
    }
  }
}
