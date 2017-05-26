using System.Runtime.Caching;
using System.Collections.Generic;
using System.Linq;

using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS.Discovery
{
  public class Orchestrator : IOrchestrator
  {
    private const string _cacheKey = "LoveOASNodes";

    #region IProcessor interface implementation
    public IExtractor Extractor { get; set; }
    public IDiscoverer Discoverer { get; set; }
    public IAuthorization Authorization { get; set; }
    public IRouteSelector RouteSelector { get; set; }
    public IParser Parser { get; set; }

    public ISerializer Serializer { get; set; }

    public ISettings Settings { get; set; }


    public IEnumerable<Node> _nodes;
    public IEnumerable<Node> Nodes
    {
      get
      {
        var temp = MemoryCache.Default[_cacheKey] as IEnumerable<Node>;
        return temp ?? new List<Node>();
      }
      private set
      {
        _nodes = value ?? new List<Node>();
        MemoryCache.Default[_cacheKey] = _nodes;
      }
    }

    /// <summary>
    /// Get entry endpoint. This is a helper for mvc api since there is no support on dependency injection in filter attributes
    /// </summary>
    /// <param name="url">Application url</param>
    /// <param name="payload">Payload</param>
    /// <returns></returns>
    public Output GetBase(object payload = null)
    {
      var links = new List<Link>();
      var entries = Nodes.Where(x => x.Metadata.IsEntry);

      foreach (var node in entries)
      {
        if (Authorization.IsAuthorized(node.Metadata.Core.Authorize))
        {
          foreach (var verb in node.Metadata.Core.Verbs)
          {
            links.Add(new Link { Href = Parser.RouteSelector.GetRoute(node.Metadata.Core.Routes), Method = verb.ToString().ToUpper(), Rel = node.Metadata.Entry.Relation });
          }
        }
      }

      return Serializer.Merge(payload, links);
    }

    /// <summary>
    /// Initialization with default implementations
    /// </summary>
    /// <param name="settings">Settings</param>
    /// <param name="namespaces">Assembly where to find the endpoints</param>
    /// <returns></returns>
    public void Setup(ISettings settings = null, params string[] namespaces)
    {
      // merge settings
      if (settings != null)
      {
        Settings = settings;
      }

      // build network and eventually cache
      if (Settings.Mode == ModeEnum.Boot)
      {
        Nodes = Discoverer.Discover(namespaces);
      }
      else
      {
        // ??
      }
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extractor">Extractor instance</param>
    /// <param name="discoverer">Discoverer instance</param>
    /// <param name="serializer">Serializer instance</param>
    /// <param name="settings">Settings instance</param>
    public Orchestrator(IExtractor extractor, IDiscoverer discoverer, ISerializer serializer, ISettings settings) 
      : this(extractor, discoverer, null, null, null, serializer, settings)
    {
      
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extractor">Extractor instance</param>
    /// <param name="discoverer">Discoverer instance</param>
    /// <param name="authorization">Authorization instance</param>
    /// <param name="route">Route selector instance</param>
    /// <param name="parser">Parser instance</param>
    /// <param name="serializer">Serializer instance</param>
    /// <param name="settings">Settings instance</param>
    public Orchestrator(IExtractor extractor = null, IDiscoverer discoverer = null, IAuthorization authorization = null, IRouteSelector route = null, IParser parser = null, ISerializer serializer = null, ISettings settings = null)
    {
      Settings = settings ?? new Settings();
      Extractor = extractor ?? new Extractor();
      Discoverer = discoverer ?? new Discoverer(Extractor, Settings);
      Authorization = authorization;
      RouteSelector = route;
      Parser = parser ?? new Parser(Authorization, RouteSelector);
      Serializer = serializer ?? new JsonOutputSerializer(Settings);
    }
  }
}
