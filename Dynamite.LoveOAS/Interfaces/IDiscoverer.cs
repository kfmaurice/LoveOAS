using System.Collections.Generic;
using System.Reflection;

using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS
{
  /// <summary>
  /// Discover nodes based on metadata extraction
  /// </summary>
  public interface IDiscoverer
  {
    /// <summary>
    /// Discover endpoints
    /// </summary>
    /// <param name="namespaces"></param>
    /// <returns></returns>
    IEnumerable<Node> Discover(params string[] namespaces);

    /// <summary>
    /// Get endpoint key
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    string GetKey(MethodInfo info);
  }
}
