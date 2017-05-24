using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

using Moq;

using Dynamite.LoveOAS.Attributes;

namespace Dynamite.LoveOAS.Discovery.Tests
{
  [TestClass()]
  public class OrchestratorTests
  {
    Mock<IAuthorization> authMoq = new Mock<IAuthorization>();
    Mock<IRouteSelector> routeMoq = new Mock<IRouteSelector>();

    [TestMethod]
    public void GetBaseTest()
    {
      authMoq.Setup(x => x.IsAuthorized(It.IsAny<AuthorizeAttributes>())).Returns(false);

      var settings = new Settings { };
      var extractor = new Extractor();
      var discoverer = new Discoverer(extractor, settings);
      var orchestrator = new Orchestrator(extractor, discoverer, authMoq.Object);

      orchestrator.Setup(settings, System.Reflection.Assembly.GetAssembly(GetType()).FullName);
      var output = orchestrator.GetBase();

      Assert.IsTrue(orchestrator.Nodes.Count() > 0);
      Assert.AreEqual(0, output.Links.Count());
    }
  }
}