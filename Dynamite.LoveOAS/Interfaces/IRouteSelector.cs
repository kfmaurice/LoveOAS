using Dynamite.LoveOAS.Attributes;

namespace Dynamite.LoveOAS
{
  public interface IRouteSelector
  {
    /// <summary>
    /// Get unique route from all the possibilities
    /// </summary>
    /// <param name="routes">All possible routes</param>
    /// <returns></returns>
    string GetRoute(RouteAttributes routes);
  }
}
