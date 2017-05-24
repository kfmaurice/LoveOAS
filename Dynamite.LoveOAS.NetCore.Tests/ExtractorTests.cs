using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

using Dynamite.LoveOAS.NetCore;
using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS.NetCoreTests
{
  [TestClass]
  public class ExtractorTests
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

      [AcceptVerbs("get", Route = "myalternative/{id}")]
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
    
    //[Base]
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

    //[Entry("Entry")]
    //[Exit(nameof(Stub_Overloaded_CheckMethodParametersTest), typeof(ExtractorTests), "Test", new Type[] { })]
    public void Stub_AttributesTest(int p1, string p2)
    {

    }

    //[Entry("Entry")]
    //[Exit(nameof(Stub_Overloaded_CheckMethodParametersTest), typeof(ExtractorTests), "Test", new Type[] { })]
    public void PostAttributesTest(int p1, string p2)
    {

    }

    [TestMethod]
    public void ExtractRouteTest()
    {
      var extractor = new Extractor();
      var routes = extractor.ExtractRoutes(typeof(MyClass).GetMethods().First(x => x.Name == nameof(MyClass.Get)));

      Assert.AreEqual("api/[controller]", routes.Prefix);
      Assert.IsNull(routes.ActionName);
      Assert.AreEqual("Get", routes.MethodName);
      Assert.IsTrue(routes.HasExplicitVerbAttribute);
      Assert.IsTrue(routes.HasImplicitVerbAttribute);
    }

    [TestMethod]
    public void ExtractRouteHasExplicitVerbTest()
    {
      var extractor = new Extractor();
      var routes = extractor.ExtractRoutes(typeof(MyClass).GetMethods().First(x => x.Name == nameof(MyClass.Random)));

      Assert.AreEqual("myaction", routes.ActionName);
      Assert.AreEqual("Random", routes.MethodName);
      Assert.IsFalse(routes.HasExplicitVerbAttribute);
      Assert.IsFalse(routes.HasImplicitVerbAttribute);
    }

    [TestMethod]
    public void ExtractRouteAttributesTest()
    {
      var extractor = new Extractor();
      var routes = extractor.ExtractRoutes(typeof(MyClass).GetMethods().First(x => x.Name == nameof(MyClass.Details)));

      Assert.AreEqual(3, routes.Attributes.Count());
      Assert.IsTrue(routes.Attributes.Any(x => x.Template == "myroute"));
      Assert.IsTrue(routes.Attributes.Any(x => x.Template == "route/{id}"));
      Assert.IsTrue(routes.Attributes.Any(x => x.Template == "myalternative/{id}"));
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
  }
}
