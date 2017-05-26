using System;
using Microsoft.Practices.Unity;

using Dynamite.LoveOAS;
using Dynamite.LoveOAS.Discovery;
using Dynamite.LoveOAS.Filters;

namespace TestWebApiUnity.App_Start
{
  /// <summary>
  /// Specifies the Unity configuration for the main container.
  /// </summary>
  public class UnityConfig
  {
    #region Unity Container
    private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
    {
      var container = new UnityContainer();
      RegisterTypes(container);
      return container;
    });

    /// <summary>
    /// Gets the configured Unity container.
    /// </summary>
    public static IUnityContainer GetConfiguredContainer()
    {
      return container.Value;
    }
    #endregion

    /// <summary>Registers the type mappings with the Unity container.</summary>
    /// <param name="container">The unity container to configure.</param>
    /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
    /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
    public static void RegisterTypes(IUnityContainer container)
    {
      // injecting instances for the default processor
      container.RegisterType<IExtractor, Extractor>();
      container.RegisterType<IDiscoverer, Discoverer>();
      container.RegisterInstance<ISettings>(new Settings {
        UseAbsoluteUrl = true,
        HandleOnlyMarkedApis = true,
        TreatCollectionAsPayload = true,
        CheckAuthorization = false
      });
      //container.RegisterType<IAuthorization, Authorization>(); // default processor instantiate this one on its own
      //container.RegisterType<IRouteSelector, RouteSelector>(); // default processor instantiate this one on its own
      //container.RegisterType<IParser, Parser>(); // default processor instantiate this one on its own
      container.RegisterType<ISerializer, JsonOutputSerializer>();
      container.RegisterType<IOrchestrator, Orchestrator>("default", new InjectionConstructor(typeof(IExtractor), typeof(IDiscoverer), typeof(ISerializer), typeof(ISettings)));
    }
  }
}
