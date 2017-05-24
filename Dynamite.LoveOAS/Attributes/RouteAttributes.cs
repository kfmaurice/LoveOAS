using System.Collections.Generic;
using System.Reflection;

using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS.Attributes
{
  public class RouteAttributes
  {
    public RouteAttributes()
    {
      Attributes = new List<RouteAttributeProxy>();
    }

    /// <summary>
    /// Value from controller route prefix
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// Route attributes
    /// </summary>
    public IEnumerable<RouteAttributeProxy> Attributes { get; set; }

    /// <summary>
    /// Value of ActionName attribute
    /// </summary>
    public string ActionName { get; set; }

    /// <summary>
    /// Method name
    /// </summary>
    public string MethodName { get; set; }

    /// <summary>
    /// Controller name
    /// </summary>
    public string ControllerName { get; set; }

    /// <summary>
    /// Method parameters
    /// </summary>
    public ParameterInfo[] Parameters { get; set; }

    /// <summary>
    /// Whether action has an explicit http method/verb given trough an attribute
    /// </summary>
    public bool HasExplicitVerbAttribute { get; set; }

    /// <summary>
    /// Whether action has an implicit http method/verb one given by the method name
    /// </summary>
    public bool HasImplicitVerbAttribute { get; set; }
  }
}
