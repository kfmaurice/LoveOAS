using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

using System;
using System.Collections.Generic;
using System.Linq;

using Dynamite.LoveOAS.Discovery;
using Dynamite.LoveOAS.Filters;

namespace Dynamite.LoveOAS.NetCore
{
  public class LoveOASFilterAttributeImplementation : BaseFilter, IActionFilter
    {

      public LoveOASFilterAttributeImplementation(IOrchestrator processor) : base(processor)
      {

      }

      public void OnActionExecuted(ActionExecutedContext context)
      {
        var result = context.Result as ObjectResult;
        var info = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;
        var key = processor.Discoverer.GetKey(info);
        var node = processor.Nodes.SingleOrDefault(x => x.Key == key);

        if (String.IsNullOrWhiteSpace(processor.Settings.AbsoluteBaseUrl))
        {
          processor.Settings.AbsoluteBaseUrl = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}{context.HttpContext.Request.PathBase}/";
        }

        SetDefaults(context);

        if (node?.Metadata?.IsBase == true)
        {
          result.Value = processor.GetBase(result.Value);
          return;
        }

        if (result?.Value != null)
        {
          if (info != null)
          {
            var links = Prepare(key, result.Value);

            if (links != null)
            {
              result.Value = processor.Serializer.Merge(result.Value, links);
            }
          }
        }
      }

      public void OnActionExecuting(ActionExecutingContext context)
      {

      }

      /// <summary>
      /// Get templates from the route data
      /// </summary>
      /// <param name="routeData">Route data from the action context</param>
      /// <returns></returns>
      public virtual IEnumerable<string> GetTemplates(RouteData routeData)
      {
        var templates = new List<string>();
        var routers = routeData.Routers.Where(x => x.GetType().IsAssignableFrom(typeof(RouteCollection))).FirstOrDefault() as RouteCollection;

        if (routers != null)
        {
          for (int i = 0; i < routers.Count; i++)
          {
            Route route = null;
            if (routers[i].GetType().IsAssignableFrom(typeof(Route)))
            {
              route = routers[i] as Route;
              templates.Add(route?.RouteTemplate);
            }
          } 
        }
        templates.RemoveAll(x => String.IsNullOrWhiteSpace(x));

        return templates;
      }

      /// <summary>
      /// Set default instances for authorization and route selection.
      /// This will be called each the times the LoveOAS filters run to initialize IAuthorization and IRouteSelector since those might depend on objects available to the filter.
      /// </summary>
      /// <param name="authorization">Default authorization</param>
      /// <param name="routeSelector">Default route selector</param>
      public virtual void SetDefaults(ActionExecutedContext context)
      {
        // authorization
        if (processor.Authorization == null || typeof(Authorization) == processor.Authorization.GetType())
        {
          processor.Authorization = new Authorization(processor.Settings);
        }

        // route selector
        if (processor.RouteSelector == null || typeof(RouteSelector) == processor.RouteSelector.GetType())
        {
          processor.RouteSelector = new RouteSelector(GetTemplates(context.RouteData));
        }

        // parser
        if (processor.Parser == null || typeof(Parser) == processor.Parser.GetType())
        {
          processor.Parser = new Parser(processor.Authorization, processor.RouteSelector);
        }
      }
    }
}
