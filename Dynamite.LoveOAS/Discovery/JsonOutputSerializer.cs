using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS.Discovery
{
  public class JsonOutputSerializer : ISerializer
  {
    public ISettings Settings { get; private set; }

    public JsonOutputSerializer(ISettings settings)
    {
      Settings = settings;
      JsonConvert.DefaultSettings = () => new JsonSerializerSettings
      {
        Formatting = Formatting.Indented,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      };
    }

    #region ISerializer interface
    /// <summary>
    /// Merge payload and links into a json string
    /// </summary>
    /// <typeparam name="T">Payload object type</typeparam>
    /// <param name="payload">Payload</param>
    /// <param name="links">Links</param>
    /// <returns></returns>
    public string MergeAsString<T>(T payload, IEnumerable<Link> links)
    {
      return $"{{ \"payload\": {SerializePayload<T>(payload)}, \"links\": {SerializeLinks(links)}}}";
    }

    /// <summary>
    /// Merge payload and links into an object
    /// </summary>
    /// <typeparam name="T">Payload object type</typeparam>
    /// <param name="payload">Payload</param>
    /// <param name="links">Links</param>
    /// <returns></returns>
    public Output Merge(object payload, IEnumerable<Link> links)
    {
      return new Output { Payload = payload, Links = GetLinksWithAbsoluteUrl(links) };
    }

    /// <summary>
    /// Serialize Links
    /// </summary>
    /// <param name="links">Links</param>
    /// <returns></returns>
    public string SerializeLinks(IEnumerable<Link> links)
    {
      return JsonConvert.SerializeObject(GetLinksWithAbsoluteUrl(links));
    }

    /// <summary>
    /// Serialize paylod
    /// </summary>
    /// <typeparam name="T">Payload object type</typeparam>
    /// <param name="payload"></param>
    /// <returns></returns>
    public string SerializePayload<T>(T payload)
    {
      return JsonConvert.SerializeObject(payload);
    } 
    #endregion

    /// <summary>
    /// Append prefix to links href property
    /// </summary>
    /// <param name="links">Links to be modified</param>
    /// <returns></returns>
    private IEnumerable<Link> GetLinksWithAbsoluteUrl(IEnumerable<Link> links)
    {
      var temp = links.Select(x => new Link(x)).ToList(); // do no modify the links => copy first
      var prefix = !Settings.UseAbsoluteUrl || string.IsNullOrWhiteSpace(Settings.AbsoluteBaseUrl) ? string.Empty : Settings.AbsoluteBaseUrl.TrimEnd('/') + "/";

      foreach (var link in temp)
      {
        link.Href = $"{prefix}{link.Href}";
      }

      return temp;
    }
  }
}
