using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Controllers;

using Dynamite.LoveOAS.Model;
using Dynamite.LoveOAS.Attributes;

namespace Dynamite.LoveOAS.Discovery
{
  public class Extractor : IExtractor
  {
    public Extractor()
    {
    }

    #region IExtractor
    /// <summary>
    /// Extract endpoint attributes
    /// </summary>
    /// <param name="type">Type of the class with the endpoint</param>
    /// <param name="method">Method which represents the endpoint</param>
    /// <param name="parameters">Parameters to distinguish method overloads</param>
    /// <returns></returns>
    public virtual Endpoint ExtractEndpoint(Type type, string method, Type[] parameters = null)
    {
      return new Endpoint
      {
        Core = ExtractCore(type, method, parameters),
        Base = ExtractBase(type, method, parameters),
        Entry = ExtractEntry(type, method, parameters),
        Exits = ExtractExit(type, method, parameters)
      };
    }

    /// <summary>
    /// Extract endpoint attributes
    /// </summary>
    /// <param name="info">Endpoint method</param>
    /// <returns></returns>
    public virtual Endpoint ExtractEndpoint(MethodInfo info)
    {
      return new Endpoint
      {
        Core = ExtractCore(info),
        Base = ExtractBase(info),
        Entry = ExtractEntry(info),
        Exits = ExtractExit(info)
      };
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Extract the core attributes of the endpoint
    /// </summary>
    /// <param name="info">Endpoint</param>
    /// <returns></returns>
    public virtual CoreAttributes ExtractCore(MethodInfo info)
    {
      return new CoreAttributes
      {
        Routes = ExtractRoutes(info),
        Verbs = ExtractVerbs(info),
        Authorize = ExtractAuthorize(info.DeclaringType, info)
      };
    }

    /// <summary>
    /// Extract the core attributes of the endpoint
    /// </summary>
    /// <param name="type">Type of the class with the endpoint</param>
    /// <param name="method">Method which represents the endpoint</param>
    /// <param name="parameters">Parameters to distinguish method overloads</param>
    /// <returns></returns>
    public virtual CoreAttributes ExtractCore(Type type, string method, Type[] parameters = null)
    {
      if (!typeof(ApiController).IsAssignableFrom(type))
      {
        return null;
      }

      var info = type.GetMethods().Where(x => x.Name == method && (parameters == null || parameters.Length == 0 || CheckMethodParameters(x.GetParameters(), parameters))).FirstOrDefault();

      return new CoreAttributes
      {
        Routes = ExtractRoutes(info),
        Verbs = ExtractVerbs(info),
        Authorize = ExtractAuthorize(type, info),
      };
    }

    /// <summary>
    /// Extract the base attribute of the endpoint
    /// </summary>
    /// <param name="info">Endpoint</param>
    /// <returns></returns>
    public virtual BaseAttribute ExtractBase(MethodInfo info)
    {
      return info.GetCustomAttribute<BaseAttribute>();
    }

    /// <summary>
    /// Extract the base attribute of the endpoint
    /// </summary>
    /// <param name="type">Type of the class with the endpoint</param>
    /// <param name="method">Method which represents the endpoint</param>
    /// <param name="parameters">Parameters to distinguish method overloads</param>
    /// <returns></returns>
    public virtual BaseAttribute ExtractBase(Type type, string method, Type[] parameters = null)
    {
      if (!typeof(ApiController).IsAssignableFrom(type))
      {
        return null;
      }

      var info = type.GetMethods().Where(x => x.Name == method && (parameters == null || parameters.Length == 0 || CheckMethodParameters(x.GetParameters(), parameters))).FirstOrDefault();

      return info.GetCustomAttribute<BaseAttribute>();
    }

    /// <summary>
    /// Extract the entry attribute of the endpoint
    /// </summary>
    /// <param name="info">Endpoint</param>
    /// <returns></returns>
    public virtual EntryAttribute ExtractEntry(MethodInfo info)
    {
      return info.GetCustomAttribute<EntryAttribute>();
    }

    /// <summary>
    /// Extract the entry attribute of the endpoint
    /// </summary>
    /// <param name="type">Type of the class with the endpoint</param>
    /// <param name="method">Method which represents the endpoint</param>
    /// <param name="parameters">Parameters to distinguish method overloads</param>
    /// <returns></returns>
    public virtual EntryAttribute ExtractEntry(Type type, string method, Type[] parameters = null)
    {
      if (!typeof(ApiController).IsAssignableFrom(type))
      {
        return null;
      }

      var info = type.GetMethods().Where(x => x.Name == method && (parameters == null || parameters.Length == 0 || CheckMethodParameters(x.GetParameters(), parameters))).FirstOrDefault();

      return info.GetCustomAttribute<EntryAttribute>();
    }

    /// <summary>
    /// Extract the exit attributes of the endpoint
    /// </summary>
    /// <param name="info">Endpoint</param>
    /// <returns></returns>
    public virtual IEnumerable<ExitAttribute> ExtractExit(MethodInfo info)
    {
      return info.GetCustomAttributes<ExitAttribute>();
    }

    /// <summary>
    /// Extract the exit attributes of the endpoint
    /// </summary>
    /// <param name="type">Type of the class with the endpoint</param>
    /// <param name="method">Method which represents the endpoint</param>
    /// <param name="parameters">Parameters to distinguish method overloads</param>
    /// <returns></returns>
    public virtual IEnumerable<ExitAttribute> ExtractExit(Type type, string method, Type[] parameters = null)
    {
      if (!typeof(ApiController).IsAssignableFrom(type))
      {
        return null;
      }

      var info = type.GetMethods().Where(x => x.Name == method && (parameters == null || parameters.Length == 0 || CheckMethodParameters(x.GetParameters(), parameters))).FirstOrDefault();

      return info.GetCustomAttributes<ExitAttribute>();
    }

    /// <summary>
    /// Eytract route directing to the given endpoint
    /// </summary>
    /// <param name="info">Endpoint</param>
    /// <returns></returns>
    public virtual RouteAttributes ExtractRoutes(MethodInfo info)
    {
      var routes = new RouteAttributes();

      routes.ControllerName = info.DeclaringType.Name.Replace("Controller", String.Empty);
      routes.Prefix = info.DeclaringType.GetCustomAttributes<RoutePrefixAttribute>().FirstOrDefault()?.Prefix;
      routes.Attributes = info.GetCustomAttributes<RouteAttribute>().Select(x => new RouteAttributeProxy { Name = x.Name, Order = x.Order, Template = x.Template });
      routes.ActionName = info.GetCustomAttribute<ActionNameAttribute>()?.Name;
      routes.MethodName = info.Name;
      routes.Parameters = info.GetParameters();
      routes.HasExplicitVerbAttribute = info.GetCustomAttributes().Any(x => (x as IActionHttpMethodProvider) != null);
      routes.HasImplicitVerbAttribute = GetVerbByName(info.Name) != HttpVerbProxy.NONE;

      return routes;
    }

    /// <summary>
    /// Extract verbs supported by the given endpoint
    /// </summary>
    /// <param name="info">Endpoint</param>
    /// <returns></returns>
    public virtual IEnumerable<HttpVerbProxy> ExtractVerbs(MethodInfo info)
    {
      var verbs = new List<HttpVerbProxy>();

      // throuh AcceptVerbs
      var v1 = info.GetCustomAttribute<AcceptVerbsAttribute>();
      if (v1 != null)
      {
        verbs.AddRange(v1.HttpMethods.Select(x => Convert(x)));
      }

      // through verb attributes
      verbs.Add(Convert(info.GetCustomAttribute<HttpGetAttribute>()?.HttpMethods?.FirstOrDefault()));
      verbs.Add(Convert(info.GetCustomAttribute<HttpPostAttribute>()?.HttpMethods?.FirstOrDefault()));
      verbs.Add(Convert(info.GetCustomAttribute<HttpPutAttribute>()?.HttpMethods?.FirstOrDefault()));
      verbs.Add(Convert(info.GetCustomAttribute<HttpDeleteAttribute>()?.HttpMethods?.FirstOrDefault()));
      verbs.Add(Convert(info.GetCustomAttribute<HttpHeadAttribute>()?.HttpMethods?.FirstOrDefault()));

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

    /// <summary>
    /// Extract authorize attributes
    /// </summary>
    /// <param name="type">Type of the class with the endpoint</param>
    /// <param name="info">Endpoint</param>
    /// <returns></returns>
    public virtual AuthorizeAttributes ExtractAuthorize(Type type, MethodInfo info)
    {
      return new AuthorizeAttributes
      {
        AllowAnonymous = info.GetCustomAttribute<AllowAnonymousAttribute>(),
        MethodAuthorize = info.GetCustomAttributes<ExtendedAuthorizeAttribute>(),
        ClassAllowAnonymous = type.GetCustomAttribute<AllowAnonymousAttribute>(),
        ClassAuthorize = type.GetCustomAttributes<ExtendedAuthorizeAttribute>(),
      };
    }

    /// <summary>
    /// Get veer by following naming convention
    /// </summary>
    /// <param name="name">Method name</param>
    /// <returns></returns>
    public virtual HttpVerbProxy GetVerbByName(string name)
    {
      if (String.IsNullOrWhiteSpace(name))
      {
        return HttpVerbProxy.NONE;
      }
      else if (name.StartsWith(HttpVerbProxy.GET.ToString(), StringComparison.InvariantCultureIgnoreCase))
      {
        return HttpVerbProxy.GET;
      }
      else if (name.StartsWith(HttpVerbProxy.POST.ToString(), StringComparison.InvariantCultureIgnoreCase))
      {
        return HttpVerbProxy.POST;
      }
      else if (name.StartsWith(HttpVerbProxy.PUT.ToString(), StringComparison.InvariantCultureIgnoreCase))
      {
        return HttpVerbProxy.PUT;
      }
      else if (name.StartsWith(HttpVerbProxy.DELETE.ToString(), StringComparison.InvariantCultureIgnoreCase))
      {
        return HttpVerbProxy.DELETE;
      }
      else if (name.StartsWith(HttpVerbProxy.HEAD.ToString(), StringComparison.InvariantCultureIgnoreCase))
      {
        return HttpVerbProxy.HEAD;
      }

      return HttpVerbProxy.NONE;
    }

    /// <summary>
    /// Check method parameters to type requirements
    /// </summary>
    /// <param name="infos">Parameter infos</param>
    /// <param name="types">Required Types</param>
    /// <returns></returns>
    public static bool CheckMethodParameters(ParameterInfo[] infos, Type[] types)
    {
      if (types == null)
      {
        return infos == null || infos.Length == 0;
      }
      else
      {
        return Enumerable.SequenceEqual(infos.Select(x => x.ParameterType.Name), types.Select(x => x.Name));
      }
    }

    /// <summary>
    /// Convert HttpMethod object to HttpVerbs enum
    /// </summary>
    /// <param name="method">HttpMethod</param>
    /// <returns>HttpVerbs Enum or zero</returns>
    public static HttpVerbProxy Convert(HttpMethod method)
    {
      if (method == HttpMethod.Get)
      {
        return HttpVerbProxy.GET;
      }
      else if (method == HttpMethod.Post)
      {
        return HttpVerbProxy.POST;
      }
      else if (method == HttpMethod.Put)
      {
        return HttpVerbProxy.PUT;
      }
      else if (method == HttpMethod.Delete)
      {
        return HttpVerbProxy.DELETE;
      }
      else if (method == HttpMethod.Head)
      {
        return HttpVerbProxy.HEAD;
      }

      return HttpVerbProxy.NONE;
    }
    #endregion
  }
}
