using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Dynamite.LoveOAS.Attributes;

namespace Dynamite.LoveOAS.Discovery
{
  public class RouteSelector : IRouteSelector
  {
    private IEnumerable<string> templates;

    /// <summary>
    /// Template placeholders
    /// </summary>
    public virtual List<string> placeholders { get; } = new List<string>() { "{controller}", "{action}" };

    public RouteSelector(IEnumerable<string> templates)
    {
      this.templates = templates;
    }

    #region IRouteSelector implementation
    /// <summary>
    /// Get a default route to be used in links
    /// </summary>
    /// <param name="routes"></param>
    /// <returns></returns>
    public virtual string GetRoute(RouteAttributes routes)
    {
      string result = String.Empty;
      var template = GetDefaultTemplate(routes);

      if (routes.Attributes.Count() > 0) // routing by attributes
      {
        var route = GetDefaultRoute(routes);
        var byRoute = GetRouteWithQuery(route, routes.Parameters.Where(x => !route.Contains($"{x.Name}".ToLower())).ToArray());

        if (String.IsNullOrWhiteSpace(routes.Prefix))
        {
          result = byRoute;
        }
        else
        {
          result = byRoute.StartsWith("~") ? byRoute.Replace("~/", String.Empty) : $"{routes.Prefix}/{byRoute}";
        }
      }
      else if (!String.IsNullOrWhiteSpace(routes.ActionName) || routes.Attributes.Count() == 0) // routing by template
      {
        var byAction = GetRouteByParameters(routes.ActionName ?? routes.MethodName, routes.Parameters);

        if (GetTemplateParameters(template).All(x => byAction.Contains(x)))
        {
          result = GetRouteWithQuery(template, routes.Parameters.Where(x => !template.Contains($"{x.Name}".ToLower())).ToArray());
        }
      }

      return result.ToLower().Trim(new char[] { '/' });
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Get parameters from template
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public virtual IEnumerable<string> GetTemplateParameters(string template)
    {
      return String.IsNullOrWhiteSpace(template) ? new List<string>() :
        new Regex("\\{[^\\}]+\\}", RegexOptions.IgnoreCase)
          .Matches(template.ToLower())
          .Cast<Match>()
          .Where(x => !placeholders.Contains(x.Value))
          .Select(x => x.Value.Trim(new char[] { '{', '}' }));
    }

    /// <summary>
    /// Remove contraints from given template
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public virtual string RemoveConstraints(string template)
    {
      var result = String.Empty;

      // {parameter:constraint}, {parameter=value}
      result = String.IsNullOrWhiteSpace(template) ? String.Empty : Regex.Replace(template, "(\\:|=)[^}]+(})", "$2").ToLower();
      // {parameter?}
      result = String.IsNullOrWhiteSpace(result) ? String.Empty : Regex.Replace(result, "({[^}?]+)\\?(})", "$1$2").ToLower();

      return result;
    }

    /// <summary>
    /// Get a default template
    /// </summary>
    /// <param name="routes">Routes</param>
    /// <returns></returns>
    public virtual string GetDefaultTemplate(RouteAttributes routes) // should this be a strategy to pass to the interface ?
    {
      var template = templates?.FirstOrDefault();

      return String.IsNullOrWhiteSpace(template) ? String.Empty : 
        RemoveConstraints(
         template.Replace(placeholders[0], routes.ControllerName?.ToLower()).Replace(placeholders[1], (routes.ActionName ?? routes.MethodName)?.ToLower())
        );
    }

    /// <summary>
    /// Select a route from the multiple alternatives
    /// </summary>
    /// <param name="routes"></param>
    /// <returns></returns>
    public virtual string GetDefaultRoute(RouteAttributes routes) // should this be a strategy to pass to the interface ?
    {
      return RemoveConstraints(routes.Attributes.FirstOrDefault()?.Template ?? String.Empty);
    }

    /// <summary>
    /// Form a route by using the given first part and the parameters
    /// </summary>
    /// <param name="start">First part of the route</param>
    /// <param name="parameters">Parameters</param>
    /// <returns></returns>
    private string GetRouteWithQuery(string start, ParameterInfo[] parameters)
    {
      if (parameters != null && parameters.Count() > 0)
      {
        var query = GetRouteByParameters(String.Empty, parameters).Replace("/", "&");

        if (start.Contains("?"))
        {
          return $"{start}{GetRouteByParameters(String.Empty, parameters).Replace("/", "&")}";
        }
        else
        {
          return $"{start}?{GetRouteByParameters(String.Empty, parameters).Replace("/", "&").TrimStart(new char[] { '/' })}".Trim(new char[] { '?' });
        } 
      }
      else
      {
        return start;
      }
    }

    /// <summary>
    /// Get route formed by the given template and parameters
    /// </summary>
    /// <param name="template">template</param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private string GetRouteByParameters(string template, ParameterInfo[] parameters)
    {
      var route = String.Empty;

      if (template != null)
      {
        if (parameters == null || parameters.Length == 0)
        {
          route = template;
        }
        else
        {
          route = $"{template}/{String.Join("/", parameters.Where(x => CheckPrimitive(x.ParameterType)).Select(x => $"{{{x.Name}}}"))}";
        }
      }

      return route.ToLower().Trim(new char[] { '/' }); ;
    }

    /// <summary>
    /// Whether given type is a primitive type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private bool CheckPrimitive(Type type)
    {
      return type.IsPrimitive || type.IsValueType || type == typeof(string);
    } 
    #endregion
  }
}
