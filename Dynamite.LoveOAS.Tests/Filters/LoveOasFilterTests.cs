using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

using Moq;
using System.Web.Http.Controllers;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace Dynamite.LoveOAS.Filters.Tests
{
  [TestClass]
  public class LoveOasFilterTests
  {
    Mock<IDiscoverer> discoverer;
    Mock<IAuthorization> authorization;
    Mock<IParser> parser;
    Mock<ISerializer> serializer;
    Mock<ISettings> settings;

    Mock<IOrchestrator> processor;
    MethodInfo method;
    HttpActionContext context;
    Mock<HttpControllerDescriptor> descriptor;

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
      descriptor = new Mock<HttpControllerDescriptor>();

      context = new HttpActionContext();
      context.ActionDescriptor = new ReflectedHttpActionDescriptor(descriptor.Object, method);
    }

    [TestMethod()]
    public async Task OnActionExecutedTest()
    {
      var filter = new LoveOasFilter(null);

      var response = await filter.ExecuteActionFilterAsync(context, new CancellationToken(), () =>
        Task.Run(() => {
          return new HttpResponseMessage();
        }));

      Assert.IsNull(response.Content);
    }    
  }
}