using System;
using System.Collections.Generic;
using System.Linq;

using Dynamite.LoveOAS.Attributes;

namespace Dynamite.LoveOAS.NetCore
{
  public class RouteSelector : Discovery.RouteSelector, IRouteSelector
  {
    private IEnumerable<string> templates;

    /// <summary>
    /// Template placeholders
    /// </summary>
    public override List<string> placeholders { get; } = new List<string>() { "[controller]", "[action]" };

    public RouteSelector(IEnumerable<string> templates) : base(templates)
    {
      this.templates = templates;
    }

    #region IRouteSelector interface implementation
    public override string GetRoute(RouteAttributes routes)
    {
      return base.GetRoute(routes).Replace(placeholders[0], routes.ControllerName).ToLower();
    } 
    #endregion

    #region Helpers
    public override string GetDefaultRoute(RouteAttributes routes) // should this be a strategy to pass to the interface ?
    {
      return RemoveConstraints(routes.Attributes.FirstOrDefault()?.Template.Replace(placeholders[1], routes.ActionName ?? routes.MethodName) ?? String.Empty);
    }
    #endregion
  }
}
