using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dynamite.LoveOAS.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Reflection;
using Dynamite.LoveOAS.Model;
using Dynamite.LoveOAS.Attributes;

namespace Dynamite.LoveOAS.Filters.Tests
{
  [TestClass()]
  public class BaseFilterTests
  {
    Mock<IDiscoverer> discoverer;
    Mock<IAuthorization> authorization;
    Mock<IParser> parser;
    Mock<ISerializer> serializer;
    Mock<ISettings> settings;

    Mock<IOrchestrator> processor;
    MethodInfo method;

    class Stub
    {
      public int MyProperty1 { get; set; }
      public string MyProperty2 { get; set; }
    }

    public void Test()
    {

    }

    [TestInitialize]
    public void Setup()
    {
      discoverer = new Mock<IDiscoverer>();
      authorization = new Mock<IAuthorization>();
      parser = new Mock<IParser>();
      serializer = new Mock<ISerializer>();
      settings = new Mock<ISettings>();
      processor = new Mock<IOrchestrator>();
      method = GetType().GetMethod(nameof(Test));
    }

    [TestMethod]
    public void PrepareNoInfo()
    {
      var filter = new BaseFilter(processor.Object);

      var links = filter.Prepare(null, new Stub());

      Assert.IsNull(links);
    }

    [TestMethod()]
    public void PrepareIsBaseNoOutputTest()
    {
      var data = new Stub { MyProperty1 = 1, MyProperty2 = "test" };
      var key = "type#func#params";

      discoverer.Setup(x => x.GetKey(It.IsAny<MethodInfo>())).Returns(key);

      processor.Setup(x => x.Nodes).Returns(new List<Node> { new Node { Key = key, Metadata = new Endpoint { Base = new BaseAttribute() } } });
      processor.Setup(x => x.Discoverer).Returns(discoverer.Object);
      processor.Setup(x => x.Authorization).Returns(authorization.Object);
      processor.Setup(x => x.Parser).Returns(parser.Object);
      processor.Setup(x => x.Serializer).Returns(serializer.Object);
      processor.Setup(x => x.Settings).Returns(settings.Object);

      var filter = new BaseFilter(processor.Object);

      var links = filter.Prepare(key, data);

      Assert.IsNull(links);
    }

    [TestMethod()]
    public void PrepareIsBaseOutputTest()
    {
      var data = new Output { Payload = new Stub(), Links = new List<Link>() };
      var key = "type#func#params";

      discoverer.Setup(x => x.GetKey(It.IsAny<MethodInfo>())).Returns(key);

      processor.Setup(x => x.Nodes).Returns(new List<Node> { new Node { Key = key, Metadata = new Endpoint { Base = new BaseAttribute() } } });
      processor.Setup(x => x.Discoverer).Returns(discoverer.Object);
      processor.Setup(x => x.Authorization).Returns(authorization.Object);
      processor.Setup(x => x.Parser).Returns(parser.Object);
      processor.Setup(x => x.Serializer).Returns(serializer.Object);
      processor.Setup(x => x.Settings).Returns(settings.Object);

      var filter = new BaseFilter(processor.Object);

      var links = filter.Prepare(key, data);

      Assert.IsNull(links);
    }

    [TestMethod()]
    public void PrepareNoBaseOutputTest()
    {
      var data = new Output { Payload = new Stub(), Links = new List<Link>() };
      var key = "type#func#params";

      discoverer = new Mock<IDiscoverer>();
      authorization = new Mock<IAuthorization>();
      parser = new Mock<IParser>();
      serializer = new Mock<ISerializer>();
      settings = new Mock<ISettings>();
      processor = new Mock<IOrchestrator>();

      discoverer.Setup(x => x.GetKey(It.IsAny<MethodInfo>())).Returns(key);

      processor.Setup(x => x.Nodes).Returns(new List<Node> { new Node { Key = key } });
      processor.Setup(x => x.Discoverer).Returns(discoverer.Object);
      processor.Setup(x => x.Authorization).Returns(authorization.Object);
      processor.Setup(x => x.Parser).Returns(parser.Object);
      processor.Setup(x => x.Serializer).Returns(serializer.Object);
      processor.Setup(x => x.Settings).Returns(settings.Object);

      var filter = new BaseFilter(processor.Object);

      var links = filter.Prepare(key, data);

      Assert.IsNull(links);
    }

    [TestMethod()]
    public void PrepareNoBaseNoOutputTest()
    {
      var data = new Stub { MyProperty1 = 1, MyProperty2 = "test" };
      var key = "type#func#params";
      var links = new List<Link>() { new Link { Href = "url", Method = "endpoint", Rel = "testing" } };

      discoverer = new Mock<IDiscoverer>();
      authorization = new Mock<IAuthorization>();
      parser = new Mock<IParser>();
      serializer = new Mock<ISerializer>();
      settings = new Mock<ISettings>();
      processor = new Mock<IOrchestrator>();

      discoverer.Setup(x => x.GetKey(It.IsAny<MethodInfo>())).Returns(key);
      parser.Setup(x => x.GetLinks(It.IsAny<string>(), It.IsAny<IEnumerable<Node>>())).Returns(links);
      settings.Setup(x => x.UseAbsoluteUrl).Returns(false);

      processor.Setup(x => x.Nodes).Returns(new List<Node> { new Node { Key = key } });
      processor.Setup(x => x.Discoverer).Returns(discoverer.Object);
      processor.Setup(x => x.Authorization).Returns(authorization.Object);
      processor.Setup(x => x.Parser).Returns(parser.Object);
      processor.Setup(x => x.Serializer).Returns(serializer.Object);
      processor.Setup(x => x.Settings).Returns(settings.Object);

      var filter = new BaseFilter(processor.Object);

      var result = filter.Prepare(key, data);

      Assert.IsNotNull(result);
      CollectionAssert.AreEqual(result.ToList(), links);
    }

    [TestMethod()]
    public void PrepareTreatCollectionTest()
    {
      var data = new Stub { MyProperty1 = 1, MyProperty2 = "test" };
      var key = "type#func#params";
      var links = new List<Link>() { new Link { Href = "url", Method = "endpoint", Rel = "testing" } };

      discoverer = new Mock<IDiscoverer>();
      authorization = new Mock<IAuthorization>();
      parser = new Mock<IParser>();
      serializer = new Mock<ISerializer>();
      settings = new Mock<ISettings>();
      processor = new Mock<IOrchestrator>();

      discoverer.Setup(x => x.GetKey(It.IsAny<MethodInfo>())).Returns(key);
      parser.Setup(x => x.GetLinks(It.IsAny<string>(), It.IsAny<IEnumerable<Node>>())).Returns(links);
      settings.Setup(x => x.UseAbsoluteUrl).Returns(false);
      settings.Setup(x => x.TreatCollectionAsPayload).Returns(true);

      processor.Setup(x => x.Nodes).Returns(new List<Node> { new Node { Key = key } });
      processor.Setup(x => x.Discoverer).Returns(discoverer.Object);
      processor.Setup(x => x.Authorization).Returns(authorization.Object);
      processor.Setup(x => x.Parser).Returns(parser.Object);
      processor.Setup(x => x.Serializer).Returns(serializer.Object);
      processor.Setup(x => x.Settings).Returns(settings.Object);

      var filter = new BaseFilter(processor.Object);

      var result = filter.Prepare(key, data);

      Assert.IsNotNull(result);
      CollectionAssert.AreEqual(result.ToList(), links);
    }

    [TestMethod()]
    public void PrepareCollectionTest()
    {
      var data = new List<Stub>() { new Stub { MyProperty1 = 1, MyProperty2 = "test" }, new Stub { MyProperty1 = 2, MyProperty2 = "test2" } };
      var key = "type#func#params";
      var links = new List<Link>() { new Link { Href = "url", Method = "endpoint", Rel = "testing" } };

      discoverer = new Mock<IDiscoverer>();
      authorization = new Mock<IAuthorization>();
      parser = new Mock<IParser>();
      serializer = new Mock<ISerializer>();
      settings = new Mock<ISettings>();
      processor = new Mock<IOrchestrator>();

      discoverer.Setup(x => x.GetKey(It.IsAny<MethodInfo>())).Returns(key);
      parser.Setup(x => x.GetLinks(It.IsAny<string>(), It.IsAny<IEnumerable<Node>>())).Returns(links);
      settings.Setup(x => x.UseAbsoluteUrl).Returns(false);
      settings.Setup(x => x.TreatCollectionAsPayload).Returns(false);

      processor.Setup(x => x.Nodes).Returns(new List<Node> { new Node { Key = key } });
      processor.Setup(x => x.Discoverer).Returns(discoverer.Object);
      processor.Setup(x => x.Authorization).Returns(authorization.Object);
      processor.Setup(x => x.Parser).Returns(parser.Object);
      processor.Setup(x => x.Serializer).Returns(serializer.Object);
      processor.Setup(x => x.Settings).Returns(settings.Object);

      var filter = new BaseFilter(processor.Object);

      var result = filter.Prepare(key, data);

      Assert.IsNull(result);
    }

    [TestMethod()]
    public void PrepareArrayTest()
    {
      var data = new Stub[] { new Stub { MyProperty1 = 1, MyProperty2 = "test" }, new Stub { MyProperty1 = 2, MyProperty2 = "test2" } };
      var key = "type#func#params";
      var links = new List<Link>() { new Link { Href = "url", Method = "endpoint", Rel = "testing" } };

      discoverer = new Mock<IDiscoverer>();
      authorization = new Mock<IAuthorization>();
      parser = new Mock<IParser>();
      serializer = new Mock<ISerializer>();
      settings = new Mock<ISettings>();
      processor = new Mock<IOrchestrator>();

      discoverer.Setup(x => x.GetKey(It.IsAny<MethodInfo>())).Returns(key);
      parser.Setup(x => x.GetLinks(It.IsAny<string>(), It.IsAny<IEnumerable<Node>>())).Returns(links);
      settings.Setup(x => x.UseAbsoluteUrl).Returns(false);
      settings.Setup(x => x.TreatCollectionAsPayload).Returns(true);
      serializer.Setup(x => x.Merge(It.IsAny<object>(), It.IsAny<IEnumerable<Link>>())).Returns(new Output { Payload = data, Links = links });

      processor.Setup(x => x.Nodes).Returns(new List<Node> { new Node { Key = key } });
      processor.Setup(x => x.Discoverer).Returns(discoverer.Object);
      processor.Setup(x => x.Authorization).Returns(authorization.Object);
      processor.Setup(x => x.Parser).Returns(parser.Object);
      processor.Setup(x => x.Serializer).Returns(serializer.Object);
      processor.Setup(x => x.Settings).Returns(settings.Object);

      var filter = new BaseFilter(processor.Object);

      var result = filter.Prepare(key, data);

      Assert.IsNotNull(result);
      CollectionAssert.AreEqual(result.ToList(), links);
    }

    [TestMethod()]
    public void PrepareTestBase()
    {
      var data = new Stub { MyProperty1 = 1, MyProperty2 = "test" };
      var key = "type#func#params";
      var links = new List<Link>() { new Link { Href = "url", Method = "endpoint", Rel = "testing" } };
      //var baseUrl = "base";

      discoverer = new Mock<IDiscoverer>();
      authorization = new Mock<IAuthorization>();
      parser = new Mock<IParser>();
      serializer = new Mock<ISerializer>();
      settings = new Mock<ISettings>();
      processor = new Mock<IOrchestrator>();

      discoverer.Setup(x => x.GetKey(It.IsAny<MethodInfo>())).Returns(key);
      parser.Setup(x => x.GetLinks(It.IsAny<string>(), It.IsAny<IEnumerable<Node>>())).Returns(links);
      settings.Setup(x => x.UseAbsoluteUrl).Returns(true);
      processor.Setup(x => x.Nodes).Returns(new List<Node> { new Node { Key = key } });
      processor.Setup(x => x.Discoverer).Returns(discoverer.Object);
      processor.Setup(x => x.Authorization).Returns(authorization.Object);
      processor.Setup(x => x.Parser).Returns(parser.Object);
      processor.Setup(x => x.Serializer).Returns(serializer.Object);
      processor.Setup(x => x.Settings).Returns(settings.Object);

      var filter = new BaseFilter(processor.Object);

      var result = filter.Prepare(key, data);

      Assert.IsNotNull(result);
      CollectionAssert.AreEqual(result.ToList(), links);
    }
  }
}