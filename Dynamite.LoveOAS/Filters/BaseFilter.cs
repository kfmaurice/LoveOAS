using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Dynamite.LoveOAS.Discovery;
using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS.Filters
{
  /// <summary>
  /// Base class for the MVC and Web Api Filters
  /// </summary>
  public class BaseFilter
  {
    protected IOrchestrator processor;

    public BaseFilter(IOrchestrator processor)
    {
      this.processor = processor;
    }

    /// <summary>
    /// Compute absolute url
    /// </summary>
    /// <param name="uri">Uri object</param>
    /// <param name="root">Base url</param>
    /// <returns></returns>
    public virtual string GetAbsoluteUrl(Uri uri, string root)
    {
      return $"{uri.Scheme}://{uri.Authority}{root}";
    }

    /// <summary>
    /// Prepare response to be sent
    /// </summary>
    /// <param name="key">Endpoint key</param>
    /// <param name="data"></param>
    /// <param name="authorization">Authorization object</param>
    /// <returns>True if response should be modified</returns>
    public virtual IEnumerable<Link> Prepare(string key, object data)
    {
      if (!String.IsNullOrWhiteSpace(key))
      {
        var isBase = processor.Nodes.SingleOrDefault(x => x.Key == key)?.Metadata?.IsBase;

        if (isBase != true // no base
            && HandleMarkApis(key) // check 
            && !(data is Output) // no output
            && (processor.Settings.TreatCollectionAsPayload || !typeof(IEnumerable).IsAssignableFrom(data.GetType())) // collection is treated as payload
           )
        {

          return processor.Parser.GetLinks(key, processor.Nodes);
        }
      }

      return null;
    }

    /// <summary>
    /// Whether to handle only marked apis (entry, exit, base)
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual bool HandleMarkApis(string key)
    {
      if (processor.Settings.HandleOnlyMarkedApis)
      {
        var node = processor.Nodes.SingleOrDefault(x => x.Key == key);

        return node != null && (node.Metadata?.IsBase == true || node.Metadata?.IsEntry == true || node.Metadata?.IsExit == true); 
      }
      else
      {
        return true;
      }
    }

    /// <summary>
    /// Set default instances for authorization and route selection
    /// </summary>
    /// <param name="authorization">Default authorization</param>
    /// <param name="routeSelector">Default route selector</param>
    public virtual void SetDefaults(IAuthorization authorization, IRouteSelector routeSelector)
    {
      // authorization
      if (processor.Authorization == null || typeof(Authorization) == processor.Authorization.GetType())
      {
        processor.Authorization = authorization;
      }

      // route selector
      if (processor.RouteSelector == null || typeof(RouteSelector) == processor.RouteSelector.GetType())
      {
        processor.RouteSelector = routeSelector;
      }

      // parser
      if (processor.Parser == null || typeof(Parser) == processor.Parser.GetType())
      {
        processor.Parser = new Parser(processor.Authorization, processor.RouteSelector);
      }
    }
  }
}
