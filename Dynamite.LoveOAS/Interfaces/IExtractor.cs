using System;
using System.Reflection;

using Dynamite.LoveOAS.Model;

namespace Dynamite.LoveOAS
{
  /// <summary>
  /// Extracts metadata
  /// </summary>
  public interface IExtractor
  {
    /// <summary>
    /// Extract endpoint data
    /// </summary>
    /// <param name="type">Type of the class with the endpoint</param>
    /// <param name="method">Method which represents the endpoint</param>
    /// <param name="parameters">Parameters to distinguish method overloads</param>
    /// <returns></returns>
    Endpoint ExtractEndpoint(Type type, string method, Type[] parameters = null);

    /// <summary>
    /// Extract base endpoint data
    /// </summary>
    /// <param name="info">Endpoint method</param>
    /// <returns></returns>
    Endpoint ExtractEndpoint(MethodInfo info);
  }
}
