using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web;
using System.Web.Http;

using Moq;

using Dynamite.LoveOAS.Attributes;
using Dynamite.LoveOAS.Model;
using Dynamite.LoveOAS.Discovery;

namespace Dynamite.LoveOAS.Tests
{
  [TestClass()]
  public class AuthorizationTests : ApiController
  {

    [TestMethod()]
    public void IsAuthorizedNoAttributesTest()
    {
      var auth = new Authorization(new Settings { CheckAuthorization = true }, null);
      var check = auth.IsAuthorized(new AuthorizeAttributes());

      Assert.IsTrue(check);
    }

    [TestMethod()]
    public void IsAuthorizedNoSettingsTest()
    {
      var auth = new Authorization(new Settings { CheckAuthorization = false }, null);
      var check = auth.IsAuthorized(new AuthorizeAttributes());

      Assert.IsTrue(check);
    }

    [TestMethod()]
    public void IsAuthorizedMethodTrueTest()
    {
      var auth = new Authorization(new Settings { CheckAuthorization = true }, null);
      var attribute = new Mock<ExtendedAuthorizeAttribute>();
      attribute.Setup(x => x.ForwardIsAuthorized(It.IsAny<HttpActionContext>())).Returns(true);

      var check = auth.IsAuthorized(new AuthorizeAttributes { MethodAuthorize = new List<ExtendedAuthorizeAttribute>() { attribute.Object} });

      Assert.IsTrue(check);
    }

    [TestMethod()]
    public void IsAuthorizedMethodFalseTest()
    {
      var auth = new Authorization(new Settings { CheckAuthorization = true }, null);
      var attribute = new Mock<ExtendedAuthorizeAttribute>();
      attribute.Setup(x => x.ForwardIsAuthorized(It.IsAny<HttpActionContext>())).Returns(false);

      var check = auth.IsAuthorized(new AuthorizeAttributes { MethodAuthorize = new List<ExtendedAuthorizeAttribute>() { attribute.Object } });

      Assert.IsFalse(check);
    }

    [TestMethod()]
    public void IsAuthorizedMvcMethodTrueTest()
    {
      var auth = new Authorization(new Settings { CheckAuthorization = true }, null);
      var attribute = new Mock<ExtendedAuthorizeAttribute>();
      attribute.Setup(x => x.ForwardIsAuthorized(It.IsAny<HttpActionContext>())).Returns(true);

      var check = auth.IsAuthorized(new AuthorizeAttributes { MethodAuthorize = new List<ExtendedAuthorizeAttribute>() { attribute.Object } });

      Assert.IsTrue(check);
    }

    [TestMethod()]
    public void IsAuthorizedMvcMethodFalseTest()
    {
      var auth = new Authorization(new Settings { CheckAuthorization = true }, null);
      var attribute = new Mock<ExtendedAuthorizeAttribute>();
      attribute.Setup(x => x.ForwardIsAuthorized(It.IsAny<HttpActionContext>())).Returns(false);

      var check = auth.IsAuthorized(new AuthorizeAttributes { MethodAuthorize = new List<ExtendedAuthorizeAttribute>() { attribute.Object } });

      Assert.IsFalse(check);
    }

    [TestMethod()]
    public void IsAuthorizedClassTrueTest()
    {
      var auth = new Authorization(new Settings { CheckAuthorization = true }, null);
      var attribute = new Mock<ExtendedAuthorizeAttribute>();
      attribute.Setup(x => x.ForwardIsAuthorized(It.IsAny<HttpActionContext>())).Returns(true);

      var check = auth.IsAuthorized(new AuthorizeAttributes { ClassAuthorize = new List<ExtendedAuthorizeAttribute>() { attribute.Object } });

      Assert.IsTrue(check);
    }

    [TestMethod()]
    public void IsAuthorizedClassFalseTest()
    {
      var auth = new Authorization(new Settings { CheckAuthorization = true }, null);
      var attribute = new Mock<ExtendedAuthorizeAttribute>();
      attribute.Setup(x => x.ForwardIsAuthorized(It.IsAny<HttpActionContext>())).Returns(false);

      var check = auth.IsAuthorized(new AuthorizeAttributes { ClassAuthorize = new List<ExtendedAuthorizeAttribute>() { attribute.Object } });

      Assert.IsFalse(check);
    }

    [TestMethod()]
    public void IsAuthorizedMvcClassTrueTest()
    {
      var auth = new Authorization(new Settings { CheckAuthorization = true }, null);
      var attribute = new Mock<ExtendedAuthorizeAttribute>();
      attribute.Setup(x => x.ForwardIsAuthorized(It.IsAny<HttpActionContext>())).Returns(true);

      var check = auth.IsAuthorized(new AuthorizeAttributes { ClassAuthorize = new List<ExtendedAuthorizeAttribute>() { attribute.Object } });

      Assert.IsTrue(check);
    }

    [TestMethod()]
    public void IsAuthorizedMvcClassFalseTest()
    {
      var auth = new Authorization(new Settings { CheckAuthorization = true }, null);
      var attribute = new Mock<ExtendedAuthorizeAttribute>();
      attribute.Setup(x => x.ForwardIsAuthorized(It.IsAny<HttpActionContext>())).Returns(false);

      var check = auth.IsAuthorized(new AuthorizeAttributes { ClassAuthorize = new List<ExtendedAuthorizeAttribute>() { attribute.Object } });

      Assert.IsFalse(check);
    }
  }
}