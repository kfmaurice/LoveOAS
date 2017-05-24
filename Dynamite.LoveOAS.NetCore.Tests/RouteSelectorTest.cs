using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;

using Dynamite.LoveOAS.NetCore;
using Dynamite.LoveOAS.Attributes;
using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS.NetCoreTests
{
  [TestClass]
  public class RouteSelectorTests
  {
    [Route("api/[controller]")]
    class MyClass
    {
      [HttpGet]
      public string Get(int id)
      {
        return "value";
      }

      [HttpGet]
      [Route("route")]
      public string GetRoute(int id)
      {
        return "value";
      }

      [HttpGet("route/{id}")]
      [Route("myroute")]
      public string Details(int id)
      {
        return "value";
      }

      [ActionName("myaction")]
      public string Random(int id)
      {
        return "value";
      }
    }

    public enum MyEnum
    {
      A,
      B
    }

    public void Stub_CheckMethodParametersTest(string p1, int p2)
    {

    }

    public void Stub_CheckMethodParametersObjectTest(MyEnum p1, object p2)
    {

    }

    #region Template routing
    [TestMethod]
    public void GetRouteActionNameFullQueryTest()
    {
      var defaultRoute = "default/[action]/[controller]";
      var templates = new List<string>() { defaultRoute, "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes { ActionName = "MyAction", ControllerName = "MyController", Parameters = parameters };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{defaultRoute.Replace("[action]", routes.ActionName).Replace("[controller]", routes.ControllerName).ToLower()}?{{p1}}&{{p2}}", route);
    }

    [TestMethod]
    public void GetRouteActionNamePartialTest()
    {
      var defaultRoute = "default/[action]/[controller]/{p1}";
      var templates = new List<string>() { defaultRoute, "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes { ActionName = "MyAction", ControllerName = "MyController", Parameters = parameters };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{defaultRoute.Replace("[action]", routes.ActionName).Replace("[controller]", routes.ControllerName).ToLower()}?{{p2}}", route);
    }

    [TestMethod]
    public void GetRouteActionNameFullTest()
    {
      var defaultRoute = "default/[action]/[controller]/{p1}/{p2}";
      var templates = new List<string>() { defaultRoute, "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes { ActionName = "MyAction", ControllerName = "MyController", Parameters = parameters };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{defaultRoute.Replace("[action]", routes.ActionName).Replace("[controller]", routes.ControllerName).ToLower()}", route);
    }

    [TestMethod]
    public void GetRouteMethodNameFullQueryTest()
    {
      var defaultRoute = "default/[action]/[controller]";
      var templates = new List<string>() { defaultRoute, "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes { MethodName = "MyAction", ControllerName = "MyController", Parameters = parameters };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{defaultRoute.Replace("[action]", routes.MethodName).Replace("[controller]", routes.ControllerName).ToLower()}?{{p1}}&{{p2}}", route);
    }

    [TestMethod]
    public void GetRouteMethodNamePartialTest()
    {
      var defaultRoute = "default/[action]/[controller]/{p1}";
      var templates = new List<string>() { defaultRoute, "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes { MethodName = "MyAction", ControllerName = "MyController", Parameters = parameters };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{defaultRoute.Replace("[action]", routes.MethodName).Replace("[controller]", routes.ControllerName).ToLower()}?{{p2}}", route);
    }

    [TestMethod]
    public void GetRouteMethodNameFullTest()
    {
      var defaultRoute = "default/[action]/[controller]/{p1}/{p2}";
      var templates = new List<string>() { defaultRoute, "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes { MethodName = "MyAction", ControllerName = "MyController", Parameters = parameters };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{defaultRoute.Replace("[action]", routes.MethodName).Replace("[controller]", routes.ControllerName).ToLower()}", route);
    }
    #endregion

    #region Attributes routing
    [TestMethod]
    public void GetRouteActionAttributesNoPrefixTest()
    {
      var first = "first/{p2}/{p1}";
      var templates = new List<string>() { "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes
      {
        Attributes = new List<RouteAttributeProxy>() {
          new RouteAttributeProxy { Template = first },
          new RouteAttributeProxy { Template = "second" }
        },
        Parameters = parameters
      };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{first}".ToLower(), route);
    }

    [TestMethod]
    public void GetRouteActionAttributesFullQueryNoPrefixTest()
    {
      var first = "first";
      var templates = new List<string>() { "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes
      {
        Attributes = new List<RouteAttributeProxy>() {
          new RouteAttributeProxy { Template = first },
          new RouteAttributeProxy { Template = "second" }
        },
        Parameters = parameters
      };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{first}?{{{parameters[0].Name}}}&{{{parameters[1].Name}}}".ToLower(), route);
    }

    [TestMethod]
    public void GetRouteActionAttributesPartialNoPrefixTest()
    {
      var first = "first/{p2}";
      var templates = new List<string>() { "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes
      {
        Attributes = new List<RouteAttributeProxy>() {
          new RouteAttributeProxy { Template = first },
          new RouteAttributeProxy { Template = "second" }
        },
        Parameters = parameters
      };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{first}?{{{parameters[0].Name}}}".ToLower(), route);
    }

    [TestMethod]
    public void GetRouteActionAttributesPrefixTest()
    {
      var first = "first/{p2}/{p1}";
      var templates = new List<string>() { "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes
      {
        Attributes = new List<RouteAttributeProxy>() {
          new RouteAttributeProxy { Template = first },
          new RouteAttributeProxy { Template = "second" }
        },
        Parameters = parameters,
        Prefix = "myPrefix"
      };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{routes.Prefix}/{first}".ToLower(), route);
    }

    [TestMethod]
    public void GetRouteActionAttributesFullQueryPrefixTest()
    {
      var first = "first";
      var templates = new List<string>() { "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes
      {
        Attributes = new List<RouteAttributeProxy>() {
          new RouteAttributeProxy { Template = first },
          new RouteAttributeProxy { Template = "second" }
        },
        Parameters = parameters,
        Prefix = "myPrefix"
      };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{routes.Prefix}/{first}?{{{parameters[0].Name}}}&{{{parameters[1].Name}}}".ToLower(), route);
    }

    [TestMethod]
    public void GetRouteActionAttributesActionFullQueryPrefixTest()
    {
      var first = "first";
      var templates = new List<string>() { "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes
      {
        Attributes = new List<RouteAttributeProxy>() {
          new RouteAttributeProxy { Template = "[action]/" + first },
          new RouteAttributeProxy { Template = "second" }
        },
        Parameters = parameters,
        Prefix = "myPrefix",
        ActionName = "MyAction"
      };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{routes.Prefix}/{routes.ActionName}/{first}?{{{parameters[0].Name}}}&{{{parameters[1].Name}}}".ToLower(), route);
    }

    [TestMethod]
    public void GetRouteActionAttributesPartialPrefixTest()
    {
      var first = "first/{p2}";
      var templates = new List<string>() { "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes
      {
        Attributes = new List<RouteAttributeProxy>() {
          new RouteAttributeProxy { Template = first },
          new RouteAttributeProxy { Template = "second" }
        },
        Parameters = parameters,
        Prefix = "myPrefix"
      };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{routes.Prefix}/{first}?{{{parameters[0].Name}}}".ToLower(), route);
    }

    [TestMethod]
    public void GetRouteActionAttributesPrefixOverwrittenTest()
    {
      var first = "first/{p2}/{p1}";
      var templates = new List<string>() { "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes
      {
        Attributes = new List<RouteAttributeProxy>() {
          new RouteAttributeProxy { Template = "~/" + first },
          new RouteAttributeProxy { Template = "second" }
        },
        Parameters = parameters,
        Prefix = "myPrefix"
      };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{first}".ToLower(), route);
    }

    [TestMethod]
    public void GetRouteActionAttributesFullQueryPrefixOverwrittenTest()
    {
      var first = "first";
      var templates = new List<string>() { "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes
      {
        Attributes = new List<RouteAttributeProxy>() {
          new RouteAttributeProxy { Template = "~/" + first },
          new RouteAttributeProxy { Template = "second" }
        },
        Parameters = parameters,
        Prefix = "myPrefix"
      };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{first}?{{{parameters[0].Name}}}&{{{parameters[1].Name}}}".ToLower(), route);
    }

    [TestMethod]
    public void GetRouteActionAttributesPartialPrefixOverwrittenTest()
    {
      var first = "first/{p2}";
      var templates = new List<string>() { "none" };
      var selector = new RouteSelector(templates);
      var type = GetType();
      var parameters = type.GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();
      var routes = new RouteAttributes
      {
        Attributes = new List<RouteAttributeProxy>() {
          new RouteAttributeProxy { Template = "~/" + first },
          new RouteAttributeProxy { Template = "second" }
        },
        Parameters = parameters,
        Prefix = "myPrefix"
      };

      var route = selector.GetRoute(routes);

      Assert.AreEqual($"{first}?{{{parameters[0].Name}}}".ToLower(), route);
    }
    #endregion

    [TestMethod]
    public void GetDefaultTemplateTest()
    {
      var defaultRoute = "default/[action]/[controller]";
      var templates = new List<string>() { defaultRoute };
      var selector = new RouteSelector(templates);
      var routes = new RouteAttributes { ActionName = "MyAction", ControllerName = "MyController" };

      var route = selector.GetDefaultTemplate(routes);

      Assert.AreEqual($"{defaultRoute}".Replace("[action]", routes.ActionName).Replace("[controller]", routes.ControllerName).ToLower(), route);
    }

    [TestMethod]
    public void GetDefaultTemplateNoTemplateTest()
    {
      var templates = new List<string>();
      var selector = new RouteSelector(templates);
      var routes = new RouteAttributes { ActionName = "MyAction", ControllerName = "MyController" };

      var route = selector.GetDefaultTemplate(routes);

      Assert.AreEqual(string.Empty, route);
    }

    [TestMethod]
    public void GetDefaultTemplateMethodNameTest()
    {
      var defaultRoute = "default/[action]/[controller]";
      var templates = new List<string>() { defaultRoute };
      var selector = new RouteSelector(templates);
      var routes = new RouteAttributes { MethodName = "MyMethod", ControllerName = "MyController" };

      var route = selector.GetDefaultTemplate(routes);

      Assert.AreEqual($"{defaultRoute}".Replace("[action]", routes.MethodName).Replace("[controller]", routes.ControllerName).ToLower(), route);
    }

    [TestMethod]
    public void GetDefaultRouteTest()
    {
      var first = "first";
      var selector = new RouteSelector(null);
      var routes = new RouteAttributes
      {
        Attributes = new List<RouteAttributeProxy>() {
          new RouteAttributeProxy { Template = first },
          new RouteAttributeProxy { Template = "second" }
        },
      };

      var route = selector.GetDefaultRoute(routes);

      Assert.AreEqual(first, route);
    }

    [TestMethod]
    public void GetDefaultRoutePrefixTest()
    {
      var first = "~/first";
      var selector = new RouteSelector(null);
      var routes = new RouteAttributes
      {
        Attributes = new List<RouteAttributeProxy>() {
          new RouteAttributeProxy { Template = first },
          new RouteAttributeProxy { Template = "second" }
        },
      };

      var route = selector.GetDefaultRoute(routes);

      Assert.AreEqual(first, route);
    }

    [TestMethod]
    public void GetDefaultRouteEmptyTest()
    {
      var selector = new RouteSelector(null);
      var routes = new RouteAttributes { Attributes = new List<RouteAttributeProxy>() { } };

      var route = selector.GetDefaultRoute(routes);

      Assert.AreEqual(String.Empty, route);
    }

    [TestMethod]
    public void GetTemplateParametersEmptyTest()
    {
      var selector = new RouteSelector(null);
      var template = "";

      var parameters = selector.GetTemplateParameters(template);

      Assert.AreEqual(0, parameters.Count());
    }

    [TestMethod]
    public void GetTemplateParametersExcludeTest()
    {
      var selector = new RouteSelector(null);
      var template = "default/[action]/[controller]";

      var parameters = selector.GetTemplateParameters(template);

      Assert.AreEqual(0, parameters.Count());
    }

    [TestMethod]
    public void GetTemplateParametersTest()
    {
      var selector = new RouteSelector(null);
      var template = "default/[action]/{Id}/[controller]";

      var parameters = selector.GetTemplateParameters(template);

      Assert.AreEqual(1, parameters.Count());
      Assert.AreEqual("id", parameters.First());
    }

    [TestMethod()]
    public void RemoveConstraintsTest()
    {
      var selector = new RouteSelector(null);
      var template = "default/[action]/{Id:int}/[controller]/{name:alpha}";

      var route = selector.RemoveConstraints(template);

      Assert.AreEqual("default/[action]/{id}/[controller]/{name}", route);
    }
  }
}
