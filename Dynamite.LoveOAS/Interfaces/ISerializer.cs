using System.Collections.Generic;

using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS
{
  public interface ISerializer
  {
    /// <summary>
    /// Serialize payload
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="payload">Custom paylod object</param>
    /// <returns></returns>
    string SerializePayload<T>(T payload);

    /// <summary>
    /// Serialize links
    /// </summary>
    /// <param name="links">Links to be serialized</param>
    /// <returns></returns>
    string SerializeLinks(IEnumerable<Link> links);

    /// <summary>
    /// Serialize payload and link into a string
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="payload">Custom payload</param>
    /// <param name="links">Links</param>
    /// <returns></returns>
    string MergeAsString<T>(T payload, IEnumerable<Link> links);

    /// <summary>
    /// Merge payload and links into an object
    /// </summary>
    /// <param name="payload">Custom payload</param>
    /// <param name="links">Links</param>
    /// <returns></returns>
    Output Merge(object payload, IEnumerable<Link> links);
  }
}
