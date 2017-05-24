using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Web.Http;

using Dynamite.LoveOAS.Attributes;
using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS.Discovery.Tests
{
  [TestClass]
  public class DiscovererTests
  {
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

    [Entry("Entry3")]
    [Exit(nameof(Stub_Overloaded_CheckMethodParametersTest), typeof(DiscovererTests), "Test2", new Type[] { typeof(string), typeof(int) })]
    [HttpPost]
    [HttpGet]
    [AcceptVerbs("DELETE", "HEAD", "POST")]
    [Route("abc/test/{p1:alpha}", Order = 2)]
    [Route("abc/route", Order = 1)]
    [Route("abc/noorder")]
    public void Stub_Overloaded_CheckMethodParametersTest()
    {

    }

    [Entry("Entry2")]
    [Exit(nameof(Stub_AttributesTest), typeof(DiscovererTests), "Test2")]
    [HttpPost]
    [HttpGet]
    [ExtendedAuthorize]
    [ExtendedAuthorize]
    [AcceptVerbs("DELETE", "HEAD", "POST")]
    [Route("abc/test/{p1:alpha}", Order = 2)]
    [Route("abc/route", Order = 1)]
    [Route("abc/noorder")]
    public void Stub_CheckMethodParametersTest(string p1, int p2)
    {

    }

    [Entry("Entry1")]
    [Exit(nameof(Stub_Overloaded_CheckMethodParametersTest), typeof(DiscovererTests), "Test1", new Type[] { })]
    [Exit(nameof(Stub_AttributesTest), typeof(DiscovererTests), "Self")]
    public void Stub_AttributesTest(int p1, string p2)
    {

    }

    [Exit(nameof(Stub_Overloaded_CheckMethodParametersTest), typeof(DiscovererTests), "Test1", new Type[] { })]
    public void Stub_Orphan(int p1, string p2)
    {

    }

    [TestMethod()]
    public void DiscoverTest()
    {
      var explorer = new Discoverer(new Extractor(), new Settings { });

      var nodes = explorer.Discover(System.Reflection.Assembly.GetAssembly(GetType()).FullName);

      var local = nodes.Where(x => x.Key.Contains(nameof(DiscovererTests)));
      var allExits = local.SelectMany(x => x.Exits);

      Assert.AreEqual(4, allExits.Count());
      Assert.AreEqual(7, nodes.Count());
      Assert.AreEqual(4, local.Count());
      Assert.AreEqual(3, nodes.Where(x => x.Key.Contains(nameof(ExtractorTests))).Count());
      Assert.AreEqual(3, local.Where(x => x.Metadata.IsEntry).Count());
      Assert.IsTrue(local.Single(x => x.Metadata.IsLeaf).Key.Contains(nameof(Stub_Overloaded_CheckMethodParametersTest))); // leaf
      Assert.AreEqual(1, local.Single(x => x.Key.Contains(nameof(Stub_CheckMethodParametersTest))).Exits.Count()); // leaf
    }

    [TestMethod()]
    public void DiscoverOrphansTest()
    {
      var explorer = new Discoverer(new Extractor(), new Settings { AllowOrphans = true });

      var nodes = explorer.Discover(System.Reflection.Assembly.GetAssembly(GetType()).FullName);

      var local = nodes.Where(x => x.Key.Contains(nameof(DiscovererTests)));
      var allExits = local.SelectMany(x => x.Exits).ToList();
      var test = allExits.ToArray();

      Assert.AreEqual(5, allExits.Count());
      Assert.AreEqual(8, nodes.Count());
      Assert.AreEqual(5, local.Count());
      Assert.AreEqual(3, nodes.Where(x => x.Key.Contains(nameof(ExtractorTests))).Count());
      Assert.AreEqual(3, local.Where(x => x.Metadata.IsEntry).Count());
      Assert.AreEqual(1, local.Single(x => x.Key.Contains(nameof(Stub_Orphan))).Exits.Count()); // leaf
    }

    [TestMethod()]
    public void GetKeyTest()
    {
      var info = GetType().GetMethod(nameof(Stub_CheckMethodParametersTest));
      var explorer = new Discoverer(new Extractor(), new Settings { });

      var key = explorer.GetKey(info);

      Assert.AreEqual(GetType().FullName + Discoverer.KeySeparator +
        nameof(Stub_CheckMethodParametersTest) + Discoverer.KeySeparator +
        String.Join(Discoverer.KeySeparator, info.GetParameters().Select(x => x.ParameterType.Name)),
        key);
    }
  }
}