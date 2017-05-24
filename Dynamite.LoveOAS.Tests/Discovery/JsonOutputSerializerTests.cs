using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS.Discovery.Tests
{
  [TestClass()]
  public class JsonOutputSerializerTests
  {
    class PayloadStub
    {
      public int Prop1 { get; set; }
      public string Prop2 { get; set; }
      public bool Prop3 { get; set; }
      public List<PayloadStub> Prop4 { get; set; }
    }

    [TestMethod()]
    public void SerializeLinksTest()
    {
      var links = new List<Link>()
      {
        new Link { Href = "a/b/c", Rel= "first", Method = "Get" },
        new Link { Href = "another/link", Rel= "other", Method = "Post" }
      };
      var serializer = new JsonOutputSerializer(new Settings { });
      var str = serializer.SerializeLinks(links);
      var obj = JsonConvert.DeserializeObject<List<Link>>(str);

      Assert.IsFalse(str.Contains("Href"));
      Assert.IsTrue(str.Contains("href"));
      Assert.AreEqual(links.Count, obj.Count);
      Assert.AreEqual(links[0].Href, obj[0].Href);
      Assert.AreEqual(links[1].Href, obj[1].Href);
    }

    [TestMethod()]
    public void SerializeLinksWithBaseUrlTest()
    {
      var baseUrl = "http://base.url";
      var links = new List<Link>()
      {
        new Link { Href = "a/b/c", Rel= "first", Method = "Get" },
        new Link { Href = "another/link", Rel= "other", Method = "Post" }
      };
      var serializer = new JsonOutputSerializer(new Settings { UseAbsoluteUrl = true, AbsoluteBaseUrl = baseUrl });
      var str = serializer.SerializeLinks(links);
      var obj = JsonConvert.DeserializeObject<List<Link>>(str);

      Assert.IsFalse(str.Contains("Href"));
      Assert.IsTrue(str.Contains("href"));
      Assert.AreEqual(links.Count, obj.Count);
      Assert.AreEqual(baseUrl + "/" + links[0].Href, obj[0].Href);
      Assert.AreEqual(baseUrl + "/" + links[1].Href, obj[1].Href);
    }

    [TestMethod()]
    public void SerializeLinksWithBaseUrlTest2()
    {
      var baseUrl = "http://base.url";
      var links = new List<Link>()
      {
        new Link { Href = "a/b/c", Rel= "first", Method = "Get" },
        new Link { Href = "another/link", Rel= "other", Method = "Post" }
      };
      var serializer = new JsonOutputSerializer(new Settings { UseAbsoluteUrl = true, AbsoluteBaseUrl = baseUrl + "/" });
      var str = serializer.SerializeLinks(links);
      var obj = JsonConvert.DeserializeObject<List<Link>>(str);

      Assert.IsFalse(str.Contains("Href"));
      Assert.IsTrue(str.Contains("href"));
      Assert.AreEqual(links.Count, obj.Count);
      Assert.AreEqual(baseUrl + "/" + links[0].Href, obj[0].Href);
      Assert.AreEqual(baseUrl + "/" + links[1].Href, obj[1].Href);
    }

    [TestMethod()]
    public void SerializePayloadTest()
    {
      var payload = new { property = "value", count = 2 };
      var serializer = new JsonOutputSerializer(new Settings { });
      var str = serializer.SerializePayload<dynamic>(payload);
      dynamic obj = JObject.Parse(str);

      Assert.AreEqual(payload.property, (string)obj.property);
      Assert.AreEqual(payload.count, (int)obj.count);
    }

    [TestMethod()]
    public void SerializePayloadObjectTest()
    {
      var payload = new PayloadStub
      {
        Prop1 = 1,
        Prop2 = "value",
        Prop3 = false,
        Prop4 = new List<PayloadStub>() {
          new PayloadStub { Prop2 = "nested" }
        }
      };
      var serializer = new JsonOutputSerializer(new Settings { });
      var str = serializer.SerializePayload<PayloadStub>(payload);
      var obj = JsonConvert.DeserializeObject<PayloadStub>(str);

      Assert.AreEqual(payload.Prop1, obj.Prop1);
      Assert.AreEqual(payload.Prop2, obj.Prop2);
      Assert.AreEqual(payload.Prop3, obj.Prop3);
      Assert.AreEqual(payload.Prop4.Count, obj.Prop4.Count);
      Assert.AreEqual(payload.Prop4[0].Prop2, obj.Prop4[0].Prop2);
    }

    [TestMethod]
    public void SerializePayloadArrayTest()
    {
      var payload = new List<PayloadStub>() {
        new PayloadStub
        {
          Prop1 = 1,
          Prop2 = "value",
          Prop3 = false,
          Prop4 = new List<PayloadStub>() {
            new PayloadStub { Prop2 = "nested" }
          }
        }
      };
      var serializer = new JsonOutputSerializer(new Settings { });
      var str = serializer.SerializePayload(payload);
      var obj = JsonConvert.DeserializeObject<List<PayloadStub>>(str);

      Assert.AreEqual(payload[0].Prop1, obj[0].Prop1);
      Assert.AreEqual(payload[0].Prop2, obj[0].Prop2);
      Assert.AreEqual(payload[0].Prop3, obj[0].Prop3);
      Assert.AreEqual(payload[0].Prop4.Count, obj[0].Prop4.Count);
      Assert.AreEqual(payload[0].Prop4[0].Prop2, obj[0].Prop4[0].Prop2);
    }

    class Packet<T>
    {
      public T Payload { get; set; }
      public List<Link> Links { get; set; }
    }
    [TestMethod()]
    public void MergeAsStringTest()
    {
      var payload = new PayloadStub
      {
        Prop1 = 1,
        Prop2 = "value",
        Prop3 = false,
        Prop4 = new List<PayloadStub>() {
          new PayloadStub { Prop2 = "nested" }
        }
      };
      var links = new List<Link>()
      {
        new Link { Href = "a/b/c", Rel= "first", Method = "Get" },
        new Link { Href = "another/link", Rel= "other", Method = "Post" }
      };
      var serializer = new JsonOutputSerializer(new Settings { });
      var str = serializer.MergeAsString(payload, links);
      Packet<PayloadStub> obj = JsonConvert.DeserializeObject<Packet<PayloadStub>>(str);

      Assert.IsTrue(str.Contains("payload"));
      Assert.IsTrue(str.Contains("links"));
      Assert.AreEqual(payload.Prop4[0].Prop2, obj.Payload.Prop4[0].Prop2);
      Assert.AreEqual(links[1].Href, obj.Links[1].Href);
    }

    [TestMethod()]
    public void MergeTest()
    {
      var payload = new PayloadStub
      {
        Prop1 = 1,
        Prop2 = "value",
        Prop3 = false,
        Prop4 = new List<PayloadStub>() {
          new PayloadStub { Prop2 = "nested" }
        }
      };
      var links = new List<Link>()
      {
        new Link { Href = "a/b/c", Rel= "first", Method = "Get" },
        new Link { Href = "another/link", Rel= "other", Method = "Post" }
      };
      var serializer = new JsonOutputSerializer(new Settings { });
      var obj = serializer.Merge(payload, links);

      Assert.AreEqual(payload, obj.Payload);
      Assert.AreEqual(links.Count, obj.Links.Count());
      Assert.AreEqual(links[0].Href, obj.Links.First().Href);
      Assert.AreEqual(links[1].Href, obj.Links.Last().Href);
    }
  }
}