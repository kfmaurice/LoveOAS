namespace Dynamite.LoveOAS
{
  public interface ISettings
  {
    /// <summary>
    /// Discovery mode
    /// </summary>
    ModeEnum Mode { get; set; }

    /// <summary>
    /// Whether to check authorization before publishing links. Unauthorized exits will not be published.
    /// </summary>
    bool CheckAuthorization { get; set; }

    /// <summary>
    /// Whether to use absolute urls for the links - "server-url".
    /// </summary>
    bool UseAbsoluteUrl { get; set; }

    /// <summary>
    /// This value changes constantly to reflect the absolute url i.e. "server-url/base/route".
    /// </summary>
    string AbsoluteBaseUrl { get; set; }

    /// <summary>
    /// Allow publishing of exits that are not an entry or do not have a marked api as predecessor.
    /// </summary>
    bool AllowOrphans { get; set; }

    /// <summary>
    /// Specify whether collections should be treated as a payload. If true then { payload: collection, links: [...] } is returned otherwise the collection is returned as it is.
    /// </summary>
    bool TreatCollectionAsPayload { get; set; }

    /// <summary>
    /// Specify whether only marked endpoint i.e. Exit, Base, Entry should be modified and return an object { payload, links } according to the other settings.
    /// </summary>
    bool HandleOnlyMarkedApis { get; set; }
  }
}