using System.Collections.Generic;

using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS
{
  public interface IOrchestrator
  {
    IExtractor Extractor { get; set; }
    IDiscoverer Discoverer { get; set; }
    IAuthorization Authorization { get; set; }
    IRouteSelector RouteSelector { get; set; }
    IParser Parser { get; set; }
    ISerializer Serializer { get; set; }
    ISettings Settings { get; set; }
    IEnumerable<Node> Nodes { get; }

    /// <summary>
    /// Get base links made of entry endpoints
    /// </summary>
    /// <param name="payload">Custom start object</param>
    /// <returns></returns>
    Output GetBase(object payload = null);

    /// <summary>
    /// Boot the plugin by building up the network and caching it
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="namespaces"></param>
    void Setup(ISettings settings = null, params string[] namespaces);
  }
}
