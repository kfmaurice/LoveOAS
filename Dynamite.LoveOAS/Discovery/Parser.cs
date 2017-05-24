using System.Linq;
using System.Collections.Generic;

using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS.Discovery
{
  public class Parser : IParser
  {
    public IAuthorization Authorization { get; set; }
    public IRouteSelector RouteSelector { get; set; }

    public Parser(IAuthorization auth, IRouteSelector selector)
    {
      Authorization = auth;
      RouteSelector = selector;
    }

    #region IParser interface implementation
    /// <summary>
    /// Return base nodes ready to be serialized
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<Link> GetBase(IEnumerable<Node> nodes)
    {
      var links = new List<Link>();
      var entries = nodes.Where(x => x.Metadata.IsEntry);

      foreach (var node in entries)
      {
        if (Authorization.IsAuthorized(node.Metadata.Core.Authorize))
        {
          foreach (var verb in node.Metadata.Core.Verbs)
          {
            links.Add(new Link { Href = RouteSelector.GetRoute(node.Metadata.Core.Routes), Method = verb.ToString().ToUpper(), Rel = node.Relation });
          }
        }
      }

      return links;
    }

    /// <summary>
    /// Return links ready to be serialized
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IEnumerable<Link> GetLinks(string key, IEnumerable<Node> nodes)
    {
      var links = new List<Link>();
      var node = nodes.SingleOrDefault(x => x.Key == key);

      if (node != null)
      {
        foreach (var exit in node.Exits)
        {
          if (Authorization.IsAuthorized(exit.Metadata.Core.Authorize))
          {
            foreach (var verb in exit.Metadata.Core.Verbs)
            {
              links.Add(new Link { Href = RouteSelector.GetRoute(exit.Metadata.Core.Routes).ToLower(), Method = verb.ToString().ToUpper(), Rel = exit.Relation });
            }
          }
        }
      }

      return links;
    }
    #endregion
  }
}
