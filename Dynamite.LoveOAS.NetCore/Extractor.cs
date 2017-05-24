using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;

using Dynamite.LoveOAS.Attributes;
using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS.NetCore
{
  public class Extractor : Discovery.Extractor
  {
    public Extractor() : base()
    {

    }

    public override RouteAttributes ExtractRoutes(MethodInfo info)
    {
      var routes = base.ExtractRoutes(info);

      List<RouteAttributeProxy> temp = new List<RouteAttributeProxy>(info.GetCustomAttributes<RouteAttribute>().Select(x => new RouteAttributeProxy { Name = x.Name, Order = x.Order, Template = x.Template }));

      temp.AddRange(info.GetCustomAttributes<AcceptVerbsAttribute>().Select(x => new RouteAttributeProxy {  Name = x.Name, Order = x.Order, Template = x.Route }));
      temp.AddRange(info.GetCustomAttributes<HttpGetAttribute>().Select(x => new RouteAttributeProxy { Name = x.Name, Order = x.Order, Template = x.Template }));
      temp.AddRange(info.GetCustomAttributes<HttpPostAttribute>().Select(x => new RouteAttributeProxy { Name = x.Name, Order = x.Order, Template = x.Template }));
      temp.AddRange(info.GetCustomAttributes<HttpPutAttribute>().Select(x => new RouteAttributeProxy { Name = x.Name, Order = x.Order, Template = x.Template }));
      temp.AddRange(info.GetCustomAttributes<HttpDeleteAttribute>().Select(x => new RouteAttributeProxy { Name = x.Name, Order = x.Order, Template = x.Template }));
      temp.AddRange(info.GetCustomAttributes<HttpHeadAttribute>().Select(x => new RouteAttributeProxy { Name = x.Name, Order = x.Order, Template = x.Template }));

      routes.Attributes = temp;
      routes.Prefix = info.DeclaringType.GetCustomAttributes<RouteAttribute>().FirstOrDefault()?.Template;
      routes.ActionName = info.GetCustomAttribute<ActionNameAttribute>()?.Name;
      routes.HasExplicitVerbAttribute = info.GetCustomAttributes().Any(x => (x as IActionHttpMethodProvider) != null || (x as HttpMethodAttribute) != null);

      return routes;
    }

    /// <summary>
    /// Extract verbs supported by the given endpoint
    /// </summary>
    /// <param name="info">Endpoint</param>
    /// <returns></returns>
    public override IEnumerable<HttpVerbProxy> ExtractVerbs(MethodInfo info)
    {
      var verbs = new List<HttpVerbProxy>();

      // throuh AcceptVerbs
      var v1 = info.GetCustomAttribute<AcceptVerbsAttribute>();
      if (v1 != null)
      {
        verbs.AddRange(v1.HttpMethods.Select(x => GetVerbByName(x)));
      }

      // through verb attributes
      verbs.Add(GetVerbByName(info.GetCustomAttribute<HttpGetAttribute>()?.HttpMethods?.FirstOrDefault()));
      verbs.Add(GetVerbByName(info.GetCustomAttribute<HttpPostAttribute>()?.HttpMethods?.FirstOrDefault()));
      verbs.Add(GetVerbByName(info.GetCustomAttribute<HttpPutAttribute>()?.HttpMethods?.FirstOrDefault()));
      verbs.Add(GetVerbByName(info.GetCustomAttribute<HttpDeleteAttribute>()?.HttpMethods?.FirstOrDefault()));
      verbs.Add(GetVerbByName(info.GetCustomAttribute<HttpHeadAttribute>()?.HttpMethods?.FirstOrDefault()));

      // clear empty verbs
      verbs.RemoveAll(x => x == HttpVerbProxy.NONE);

      if (verbs.Count == 0)
      {
        var convention = GetVerbByName(info.Name);

        if (convention == HttpVerbProxy.NONE)
        {
          verbs.Add(HttpVerbProxy.GET);
        }
        else
        {
          verbs.Add(convention);
        }
      }

      return verbs.Distinct();
    }
  }
}
