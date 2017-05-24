using Dynamite.LoveOAS;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Dynamite.LoveOAS.Attributes;
using Dynamite.LoveOAS.Discovery;
using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS.Tests
{
  [TestClass()]
  public class ParserTests
  {
    Mock<IAuthorization> authMoq = new Mock<IAuthorization>();
    Mock<IRouteSelector> routeMoq = new Mock<IRouteSelector>();

    [TestMethod()]
    public void GetBaseAuthorizedTest()
    {
      authMoq.Setup(x => x.IsAuthorized(It.IsAny<AuthorizeAttributes>())).Returns(true);

      var explorer = new Discoverer(new Extractor(), new Settings { });
      var nodes = explorer.Discover(System.Reflection.Assembly.GetAssembly(GetType()).FullName);
      var local = nodes.Where(x => x.Key.Contains(nameof(Discovery.Tests.DiscovererTests)));
      var parser = new Parser(authMoq.Object, routeMoq.Object);
      var entries = parser.GetBase(local);

      Assert.AreEqual(9, entries.Count());
    }

    [TestMethod()]
    public void GetBaseNotAuthorizedTest()
    {
      authMoq.Setup(x => x.IsAuthorized(It.IsAny<AuthorizeAttributes>())).Returns(false);

      var explorer = new Discoverer(new Extractor(), new Settings { });
      var nodes = explorer.Discover(System.Reflection.Assembly.GetAssembly(GetType()).FullName);
      var local = nodes.Where(x => x.Key.Contains(nameof(Discovery.Tests.DiscovererTests)));
      var parser = new Parser(authMoq.Object, routeMoq.Object);
      var entries = parser.GetBase(local);

      Assert.AreEqual(0, entries.Count());
    }

    [TestMethod()]
    public void GetAuthorizedLinksTest()
    {
      authMoq.Setup(x => x.IsAuthorized(It.IsAny<AuthorizeAttributes>())).Returns(true);
      routeMoq.Setup(x => x.GetRoute(It.IsAny<RouteAttributes>())).Returns("route");

      var explorer = new Discoverer(new Extractor(), new Settings { });
      var nodes = explorer.Discover(System.Reflection.Assembly.GetAssembly(GetType()).FullName);
      var local = nodes.Where(x => x.Key.Contains(nameof(Discovery.Tests.DiscovererTests)));
      var parser = new Parser(authMoq.Object, routeMoq.Object);
      var method = typeof(Discovery.Tests.DiscovererTests).GetMethods().First(x => x.Name == nameof(Discovery.Tests.DiscovererTests.Stub_Overloaded_CheckMethodParametersTest) && x.GetParameters().Length == 0);
      var entries = parser.GetLinks(explorer.GetKey(method), local);

      Assert.AreEqual(4, entries.Count());
    }

    [TestMethod()]
    public void GetNotAuthorizedLinksTest()
    {
      authMoq.Setup(x => x.IsAuthorized(It.IsAny<AuthorizeAttributes>())).Returns(false);

      var explorer = new Discoverer(new Extractor(), new Settings { });
      var nodes = explorer.Discover(System.Reflection.Assembly.GetAssembly(GetType()).FullName);
      var local = nodes.Where(x => x.Key.Contains(nameof(Discovery.Tests.DiscovererTests)));
      var parser = new Parser(authMoq.Object, routeMoq.Object);
      var method = typeof(Discovery.Tests.DiscovererTests).GetMethods().First(x => x.Name == nameof(Discovery.Tests.DiscovererTests.Stub_Overloaded_CheckMethodParametersTest) && x.GetParameters().Length == 0);
      var entries = parser.GetLinks(explorer.GetKey(method), local);

      Assert.AreEqual(0, entries.Count());
    }
  }
}