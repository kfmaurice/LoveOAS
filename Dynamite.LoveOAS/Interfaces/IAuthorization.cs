using Dynamite.LoveOAS.Attributes;

namespace Dynamite.LoveOAS
{
  /// <summary>
  /// Handle endpoint authorization
  /// </summary>
  public interface IAuthorization
  {
    /// <summary>
    /// Calculate authorization on a specific endpoint
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    bool IsAuthorized(AuthorizeAttributes attributes);
  }
}
