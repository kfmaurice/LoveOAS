using System.Web.Http;

using Dynamite.LoveOAS;
using Dynamite.LoveOAS.Discovery;

namespace TestWebApi
{
  public class WebApiApplication : System.Web.HttpApplication
  {
    public static ISettings Settings = new Settings
    {
      UseAbsoluteUrl = true,
      HandleOnlyMarkedApis = true,
      TreatCollectionAsPayload = true,
      CheckAuthorization = false
    };
    protected void Application_Start()
    {
      GlobalConfiguration.Configure(WebApiConfig.Register);
    }
  }
}
