using System.Collections.Generic;

using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS
{
  public interface IParser
  {
    IAuthorization Authorization { get; set; }
    IRouteSelector RouteSelector { get; set; }

    /// <summary>
    /// Get entry endpoints
    /// </summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    IEnumerable<Link> GetBase(IEnumerable<Node> nodes);

    /// <summary>
    /// Get all possible exits from the endpoint with the given key
    /// </summary>
    /// <param name="key">Endpoint key</param>
    /// <param name="nodes">Nework</param>
    /// <returns></returns>
    IEnumerable<Link> GetLinks(string key, IEnumerable<Node> nodes);
  }
}
