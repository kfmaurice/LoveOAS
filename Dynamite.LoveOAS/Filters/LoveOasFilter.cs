using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http.Formatting;
using System.Collections.Generic;

using Dynamite.LoveOAS.Discovery;
using Dynamite.LoveOAS.Model;

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
          SetDefaults(processor.Authorization = new Authorization(processor.Settings, actionContext), 
            new RouteSelector(actionContext.RequestContext.Configuration.Routes.Skip(1).ToList().Select(x => x.RouteTemplate)));

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
    #endregion
  }
}
