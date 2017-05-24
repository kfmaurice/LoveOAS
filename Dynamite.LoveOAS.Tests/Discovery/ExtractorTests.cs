using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

using Dynamite.LoveOAS.Attributes;
using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS.Discovery.Tests
{
  [TestClass()]
  [AllowAnonymous]
  public class ExtractorTests : ApiController
  {
    [ActionName("")]
    public void Stub_EmptyActionNameTest()
    {

    }

    [ActionName("notempty")]
    public void Stub_ActionNameTest()
    {

    }

    [ActionName("notempty")]
    public void Stub_ActionNameParamsTest(int a, string bc)
    {

    }

    [Base]
    [HttpPost]
    [HttpGet]
    [AcceptVerbs("DELETE", "HEAD")]
    [Route("abc/test/{p1:alpha}", Order = 2)]
    [Route("abc/route", Order = 1)]
    [Route("abc/noorder")]
    public void Stub_Overloaded_CheckMethodParametersTest(string p1, int p2)
    {

    }

    [HttpPost]
    [HttpGet]
    [AcceptVerbs("DELETE", "HEAD", "POST")]
    [Route("abc/test/{p1:alpha}", Order = 2)]
    [Route("abc/route", Order = 1)]
    [Route("abc/noorder")]
    public void Stub_Overloaded_CheckMethodParametersTest()
    {

    }

    [HttpPost]
    [HttpGet]
    [ExtendedAuthorize]
    [AcceptVerbs("DELETE", "HEAD", "POST")]
    [Route("abc/test/{p1:alpha}", Order = 2)]
    [Route("abc/route", Order = 1)]
    [Route("abc/noorder")]
    public void Stub_CheckMethodParametersTest(string p1, int p2)
    {

    }

    [Entry("Entry")]
    [Exit(nameof(Stub_Overloaded_CheckMethodParametersTest), typeof(ExtractorTests), "Test", new Type[] { })]
    public void Stub_AttributesTest(int p1, string p2)
    {

    }

    [Entry("Entry")]
    [Exit(nameof(Stub_Overloaded_CheckMethodParametersTest), typeof(ExtractorTests), "Test", new Type[] { })]
    public void PostAttributesTest(int p1, string p2)
    {

    }

    public void Stub_NoRouteTest(int p1, string p2)
    {

    }

    [RoutePrefix("prefix")]
    class RoutePrefixStub
    {
      [Route("route")]
      public void Stub_RouteTest()
      {

      }

      [Route("~/route")]
      public void Stub_OverwrittenRouteTest()
      {

      }
    }

    [TestMethod]
    public void ExtractEndpointInfoTest()
    {
      var info = GetType().GetMethods().First(x => x.Name == nameof(Stub_AttributesTest));
      var extractor = new Extractor();

      var node = extractor.ExtractEndpoint(info);

      Assert.IsNotNull(node);
    }

    [TestMethod]
    public void ExtractEndpointTest()
    {
      var type = GetType();
      var method = nameof(Stub_AttributesTest);
      var extractor = new Extractor();

      var node = extractor.ExtractEndpoint(type, method);

      Assert.IsNotNull(node);
    }


    [TestMethod]
    public void ExtractCoreInfoTest()
    {
      var info = GetType().GetMethods().First(x => x.Name == nameof(Stub_AttributesTest));
      var extractor = new Extractor();

      var core = extractor.ExtractCore(info);

      Assert.IsNotNull(core);
      Assert.AreEqual(0, core.Routes.Attributes.Count());
      Assert.IsNotNull(core.Authorize);
      Assert.IsNull(core.Authorize.AllowAnonymous);
      Assert.IsNotNull(core.Authorize.ClassAllowAnonymous);
      Assert.AreEqual(0, core.Authorize.MethodAuthorize.Count());
      Assert.AreEqual(0, core.Authorize.ClassAuthorize.Count());
      Assert.AreEqual(0, core.Authorize.MethodAuthorize.Count());
      Assert.AreEqual(0, core.Authorize.ClassAuthorize.Count());
    }

    [TestMethod]
    public void ExtractCoreTest()
    {
      var type = GetType();
      var method = nameof(Stub_AttributesTest);
      var extractor = new Extractor();

      var core = extractor.ExtractCore(type, method);

      Assert.IsNotNull(core);
      Assert.AreEqual(0, core.Routes.Attributes.Count());
    }

    [TestMethod]
    public void ExtractEntryInfoTest()
    {
      var type = GetType();
      var info = GetType().GetMethods().First(x => x.Name == nameof(Stub_AttributesTest));
      var extractor = new Extractor();

      var attribute = extractor.ExtractEntry(info);

      Assert.AreEqual("Entry", attribute.Relation);
    }

    [TestMethod]
    public void ExtractEntryTest()
    {
      var type = GetType();
      var method = nameof(Stub_AttributesTest);
      var extractor = new Extractor();

      var attribute = extractor.ExtractEntry(type, method, new Type[] { typeof(int), typeof(string) });

      Assert.AreEqual("Entry", attribute.Relation);
    }

    [TestMethod]
    public void ExtractExitInfoTest()
    {
      var type = GetType();
      var info = GetType().GetMethods().First(x => x.Name == nameof(Stub_AttributesTest));
      var extractor = new Extractor();

      var attribute = extractor.ExtractExit(info).First();

      Assert.AreEqual(nameof(Stub_Overloaded_CheckMethodParametersTest), attribute.Method);
      CollectionAssert.AreEqual(new Type[] { }, attribute.Parameters);
      Assert.AreEqual(type, attribute.Parent);
      Assert.AreEqual("Test", attribute.Relation);
    }

    [TestMethod]
    public void ExtractExitTest()
    {
      var type = GetType();
      var method = nameof(Stub_AttributesTest);
      var extractor = new Extractor();

      var attribute = extractor.ExtractExit(type, method, new Type[] { typeof(int), typeof(string) }).First();

      Assert.AreEqual(nameof(Stub_Overloaded_CheckMethodParametersTest), attribute.Method);
      CollectionAssert.AreEqual(new Type[] { }, attribute.Parameters);
      Assert.AreEqual(type, attribute.Parent);
      Assert.AreEqual("Test", attribute.Relation);
    }

    [TestMethod]
    public void ExtractRouteEmptyActionNameTest()
    {
      var extractor = new Extractor();
      var routes = extractor.ExtractRoutes(GetType().GetMethods().First(x => x.Name == nameof(Stub_EmptyActionNameTest)));

      Assert.AreEqual(0, routes.Attributes.Count());
      Assert.AreEqual(String.Empty, routes.ActionName);

    }

    [TestMethod]
    public void ExtractRouteActionNameTest()
    {
      var extractor = new Extractor();
      var routes = extractor.ExtractRoutes(GetType().GetMethods().First(x => x.Name == nameof(Stub_ActionNameTest)));

      Assert.AreEqual("notempty", routes.ActionName);
    }

    [TestMethod]
    public void ExtractRouteActionNameParamsTest()
    {
      var extractor = new Extractor();
      var info = GetType().GetMethods().First(x => x.Name == nameof(Stub_ActionNameParamsTest));
      var routes = extractor.ExtractRoutes(info);

      Assert.AreEqual(0, routes.Attributes.Count());
      Assert.AreEqual("notempty", routes.ActionName);

      CollectionAssert.AreEqual(info.GetParameters(), routes.Parameters);
    }

    [TestMethod]
    public void ExtractRouteAttributesTest()
    {
      var extractor = new Extractor();
      var routes = extractor.ExtractRoutes(GetType().GetMethods().First(x => x.Name == nameof(Stub_Overloaded_CheckMethodParametersTest) && x.GetParameters().Length == 2));
      var expected = new List<string>()
      {
        "abc/noorder",
        "abc/route",
        "abc/test/{p1:alpha}"
      };

      CollectionAssert.AreEqual(expected.OrderBy(x => x).ToList(), routes.Attributes.Select(x => x.Template).OrderBy(x => x).ToList());
    }

    [TestMethod]
    public void ExtractRouteTest()
    {
      var extractor = new Extractor();
      var routes = extractor.ExtractRoutes(GetType().GetMethods().First(x => x.Name == nameof(Stub_Overloaded_CheckMethodParametersTest) && x.GetParameters().Length == 2));
      
      Assert.AreEqual(nameof(ExtractorTests), routes.ControllerName);
      Assert.AreEqual(nameof(Stub_Overloaded_CheckMethodParametersTest), routes.MethodName);
      Assert.AreEqual(null, routes.ActionName);
    }

    [TestMethod]
    public void ExtractRoutePrefixTest()
    {
      var extractor = new Extractor();
      var routes = extractor.ExtractRoutes(typeof(RoutePrefixStub).GetMethods().First(x => x.Name == nameof(RoutePrefixStub.Stub_RouteTest)));

      Assert.AreEqual("prefix", routes.Prefix);
    }

    [TestMethod()]
    public void ExtractVerbsTest()
    {
      var extractor = new Extractor();
      var verbs = extractor.ExtractVerbs(GetType().GetMethods().First(x => x.Name == nameof(Stub_Overloaded_CheckMethodParametersTest) && x.GetParameters().Length == 2))
        .ToList();
      var expected = new List<HttpVerbProxy>()
      {
        HttpVerbProxy.DELETE,
        HttpVerbProxy.HEAD,
        HttpVerbProxy.GET,
        HttpVerbProxy.POST
      };

      CollectionAssert.AreEqual(expected, verbs);
    }

    [TestMethod()]
    public void ExtractVerbsGetByDefaultTest()
    {
      var extractor = new Extractor();
      var verbs = extractor.ExtractVerbs(GetType().GetMethods().First(x => x.Name == nameof(Stub_AttributesTest)))
        .ToList();
      var expected = new List<HttpVerbProxy>()
      {
        HttpVerbProxy.GET
      };

      CollectionAssert.AreEqual(expected, verbs);
    }

    [TestMethod()]
    public void ExtractVerbsPostByNameTest()
    {
      var extractor = new Extractor();
      var verbs = extractor.ExtractVerbs(GetType().GetMethods().First(x => x.Name == nameof(PostAttributesTest)))
        .ToList();
      var expected = new List<HttpVerbProxy>()
      {
        HttpVerbProxy.POST
      };

      CollectionAssert.AreEqual(expected, verbs);
    }

    [TestMethod()]
    public void ExtractAuthorizeTest()
    {
      var extractor = new Extractor();
      var type = GetType();
      var info = type.GetMethod(nameof(Stub_CheckMethodParametersTest));

      Assert.AreEqual(1, extractor.ExtractAuthorize(type, info).MethodAuthorize.Count());
      Assert.AreEqual(1, extractor.ExtractAuthorize(type, info).MethodAuthorize.Count());
      Assert.IsNotNull(extractor.ExtractAuthorize(type, info).ClassAllowAnonymous);
    }

    [TestMethod()]
    public void CheckMethodParametersTest()
    {
      var parameters = GetType().GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();

      Assert.IsTrue(Extractor.CheckMethodParameters(parameters, new Type[] { typeof(string), typeof(int) }));
    }
    [TestMethod()]
    public void CheckMethodParametersOrderTest()
    {
      var parameters = GetType().GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();

      Assert.IsFalse(Extractor.CheckMethodParameters(parameters, new Type[] { typeof(int), typeof(string) }));
    }
    [TestMethod()]
    public void CheckMethodParametersFailedTest()
    {
      var parameters = GetType().GetMethod(nameof(Stub_CheckMethodParametersTest)).GetParameters();

      Assert.IsFalse(Extractor.CheckMethodParameters(parameters, new Type[] { typeof(string), typeof(string) }));
    }
    [TestMethod()]
    public void CheckMethodParametersOverloadedTest()
    {
      var parameters = GetType().GetMethods().First(x => x.Name == nameof(Stub_Overloaded_CheckMethodParametersTest) && x.GetParameters().Length == 0).GetParameters();

      Assert.IsTrue(Extractor.CheckMethodParameters(parameters, null));
    }
    [TestMethod()]
    public void CheckMethodParametersOverloadedEmptyTest()
    {
      var parameters = GetType().GetMethods().First(x => x.Name == nameof(Stub_Overloaded_CheckMethodParametersTest) && x.GetParameters().Length == 0).GetParameters();

      Assert.IsTrue(Extractor.CheckMethodParameters(parameters, new Type[] { }));
    }
    [TestMethod()]
    public void CheckMethodParametersOverloadedFailedTest()
    {
      var parameters = GetType().GetMethods().First(x => x.Name == nameof(Stub_Overloaded_CheckMethodParametersTest) && x.GetParameters().Length == 2).GetParameters();

      Assert.IsFalse(Extractor.CheckMethodParameters(parameters, null));
    }

    [TestMethod()]
    public void ConvertTest()
    {
      HttpMethod h = new HttpMethod(HttpMethod.Get.Method);

      Assert.AreEqual(HttpVerbProxy.GET, Extractor.Convert(h));
    }
    [TestMethod()]
    public void ConvertFailTest()
    {
      HttpMethod h = new HttpMethod("random");

      Assert.AreEqual(HttpVerbProxy.NONE, Extractor.Convert(h));
    }

    [TestMethod()]
    public void ExtractBaseTest()
    {
      var extractor = new Extractor();
      var b = extractor.ExtractBase(GetType().GetMethods().First(x => x.Name == nameof(Stub_Overloaded_CheckMethodParametersTest) && x.GetParameters().Length == 2));

      Assert.IsNotNull(b);
    }

    [TestMethod()]
    public void ExtractBaseTest1()
    {
      var extractor = new Extractor();
      var b = extractor.ExtractBase(GetType(), nameof(Stub_Overloaded_CheckMethodParametersTest), new Type[] { typeof(string), typeof(int) });

      Assert.IsNotNull(b);
    }
  }
}