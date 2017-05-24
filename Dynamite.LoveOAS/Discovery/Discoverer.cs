using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Reflection;

using Dynamite.LoveOAS.Model;
using Dynamite.LoveOAS.Attributes;


namespace Dynamite.LoveOAS.Discovery
{
  public class Discoverer : IDiscoverer
  {
    public const string KeySeparator = "#";

    public IExtractor Extractor { get; set; }
    public ISettings Settings { get; set; }

    public Discoverer(IExtractor extractor, ISettings settings)
    {
      Extractor = extractor;
      Settings = settings;
    }

    #region IExplorer interface implementation
    /// <summary>
    /// Discover all endpoints in the given namespaces
    /// <param name="namespaces">Namespaces where the endpoints are defined</param>
    /// <returns></returns>
    /// </summary>
    public virtual IEnumerable<Node> Discover(params string[] namespaces)
    {
      var nodes = new List<Node>();
      var classes = namespaces.SelectMany(x => Assembly.Load(x).GetTypes()); // classes
      var methods = classes.SelectMany(x => x.GetMethods()).Where(x => x.IsPublic && x.GetCustomAttribute<NonActionAttribute>() == null); // methods
      var entries = methods.Where(y => y.GetCustomAttributes<EntryAttribute>().Count() > 0);

      Discover(entries, nodes, methods);

      // handle orphans
      var exits = methods.Where(y => y.GetCustomAttributes<ExitAttribute>().Count() > 0);
      var orphans = GetOrphans(exits, nodes);

      if (Settings.AllowOrphans)
      {
        Discover(orphans, nodes, methods);
      }

      // handle base
      var bases = methods.Where(y => y.GetCustomAttributes<BaseAttribute>().Count() > 0);

      Discover(bases, nodes, methods);

      return nodes;
    }

    /// <summary>
    /// Get unique identifier for method
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public string GetKey(MethodInfo info)
    {
      var parameters = info.GetParameters().Select(x => x.ParameterType.Name);
      var str = parameters.Count() > 0 ? String.Join(KeySeparator, parameters) : String.Empty;

      return info.DeclaringType.FullName + KeySeparator + info.Name + (String.IsNullOrWhiteSpace(str) ? String.Empty : KeySeparator + str);
    }

    /// <summary>
    /// Detect exit which are not bound to an entry or to another exit
    /// </summary>
    /// <param name="exits">All exits</param>
    /// <param name="nodes">Discovered nodes</param>
    public IEnumerable<MethodInfo> GetOrphans(IEnumerable<MethodInfo> exits, IEnumerable<Node> nodes)
    {
      var orphans = exits.Where(x => !nodes.Any(y => y.Key == GetKey(x)));

      if (orphans.Count() > 0 && !Settings.AllowOrphans)
      {
        foreach (var orphan in orphans)
        {
          var parameters = orphan.GetParameters().Select(x => x.ParameterType.Name);
          var str = parameters.Count() > 0 ? String.Join(KeySeparator, parameters) : String.Empty;
          var msg = $"{nameof(LoveOAS)}: warning: exit detected without entry at {orphan.DeclaringType.FullName}.{orphan.Name}({str}). Consider setting '{nameof(Settings.AllowOrphans)}' to 'true' to make such exits discoverable !";

          Console.WriteLine(msg);
          Debug.WriteLine(msg);
        }
      }

      return orphans;
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Discover exits of the given node
    /// </summary>
    /// <param name="node">Node</param>
    /// <param name="nodes">Nodes collection to be extended</param>
    /// <param name="methods">Global methods collection</param>
    private void DiscoverExits(Node node, List<Node> nodes, IEnumerable<MethodInfo> methods)
    {
      foreach (var e in node.Metadata.Exits)
      {
        var method = methods.Where(x => x.Name == e.Method && x.DeclaringType.FullName == e.Parent.FullName &&
          (e.Parameters == null || e.Parameters.Length == 0 || Discovery.Extractor.CheckMethodParameters(x.GetParameters(), e.Parameters))) // handle overload methods
          .First();
        var key = GetKey(method);
        var temp = new Node { Key = key, Metadata = Extractor.ExtractEndpoint(method), Relation = e.Relation };

        node.Exits.Add(temp);
        if (!nodes.Any(x => x.Key == key)) // recursion break
        {
          nodes.Add(temp);
          // recursion
          DiscoverExits(temp, nodes, methods);
        }

      }
    } 

    /// <summary>
    /// Discover nodes through recursion
    /// </summary>
    /// <param name="entries">Entries or exits</param>
    /// <param name="nodes">Node accumulator</param>
    /// <param name="methods">All endpoints</param>
    private void Discover(IEnumerable<MethodInfo> entries, List<Node> nodes, IEnumerable<MethodInfo> methods)
    {
      foreach (var e in entries)
      {
        var key = GetKey(e);
        var endpoint = Extractor.ExtractEndpoint(e);
        var node = new Node { Key = key, Metadata = endpoint, Relation = endpoint.IsEntry ? endpoint.Entry.Relation : String.Empty };

        if (!nodes.Any(x => x.Key == key)) // recursion break
        {
          nodes.Add(node);
          // recursion
          DiscoverExits(node, nodes, methods);
        }
      }
    }
    #endregion
  }
}
