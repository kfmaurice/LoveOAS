using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Dynamite.LoveOAS.Model;
using Dynamite.LoveOAS.Discovery;

namespace Dynamite.LoveOAS.Filters
{
  public class LoveOasFilter : BaseFilter, IActionFilter
  {
    public LoveOasFilter(IOrchestrator processor) : base(processor)
    {
    }

    #region IActionFilter Interface implementation
    public bool AllowMultiple
    {
      get
      {
        return false;
      }
    }

    public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
    {
      HttpActionExecutedContext executedContext;

      var response = await continuation();

      try
      {
        executedContext = new HttpActionExecutedContext(actionContext, null)
        {
          Response = response
        };

        var data = response.Content;
        HttpResponseMessage result = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

        if (response.IsSuccessStatusCode && data != null)
        {
          var info = (actionContext.ActionDescriptor as ReflectedHttpActionDescriptor)?.MethodInfo;

          if (String.IsNullOrWhiteSpace(processor.Settings.AbsoluteBaseUrl))
          {
            processor.Settings.AbsoluteBaseUrl = GetAbsoluteUrl(actionContext.Request.RequestUri, actionContext.RequestContext.VirtualPathRoot); ;
          }
          SetDefaults(actionContext);

          if (info != null)
          {
            IEnumerable<Link> links;
            var key = processor.Discoverer.GetKey(info);
            var node = processor.Nodes.SingleOrDefault(x => x.Key == key);
            var content = await data.ReadAsAsync<object>();

            if (node?.Metadata?.IsBase == true)
            {
              result.Content = new ObjectContent<Output>(processor.GetBase(content), actionContext.RequestContext.Configuration.Formatters.JsonFormatter, JsonMediaTypeFormatter.DefaultMediaType.MediaType);
              executedContext.Response = result;
            }
            else
            {
              links = Prepare(key, content);
              if (links != null)
              {
                result.Content = new ObjectContent<Output>(processor.Serializer.Merge(content, links), actionContext.RequestContext.Configuration.Formatters.JsonFormatter, JsonMediaTypeFormatter.DefaultMediaType.MediaType);
                executedContext.Response = result;
              }  
            }
          }
        }
      }
      catch (Exception exception)
      {
        executedContext = new HttpActionExecutedContext(actionContext, exception);
      }

      return executedContext.Response;
    }

    /// <summary>
    /// Set default instances for authorization and route selection.
    /// This will be called each the times the LoveOAS filters runs.
    /// It is used to initialize objects that might depend on properties of the filter.
    /// </summary>
    /// <param name="actionContext">Filter action context</param>
    public virtual void SetDefaults(HttpActionContext actionContext)
    {
      // authorization
      if (processor.Authorization == null || typeof(Authorization) == processor.Authorization.GetType())
      {
        processor.Authorization = new Authorization(processor.Settings, actionContext);
      }

      // route selector
      if (processor.RouteSelector == null || typeof(RouteSelector) == processor.RouteSelector.GetType())
      {
        processor.RouteSelector = new RouteSelector(actionContext.RequestContext.Configuration.Routes.Skip(1).ToList().Select(x => x.RouteTemplate));
      }

      // parser
      if (processor.Parser == null || typeof(Parser) == processor.Parser.GetType())
      {
        processor.Parser = new Parser(processor.Authorization, processor.RouteSelector);
      }
    }
    #endregion
  }
}
