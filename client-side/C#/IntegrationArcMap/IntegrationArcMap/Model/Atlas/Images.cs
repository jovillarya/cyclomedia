using System.Xml.Linq;

namespace IntegrationArcMap.Model.Atlas
{
  /// <summary>
  /// 
  /// </summary>
  public class Images
  {
    /// <summary>
    /// Default empty constructor
    /// </summary>
    public Images()
    {
      // empty
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mappedFeatureElement"></param>
    public Images(XElement mappedFeatureElement)
    {
      Update(mappedFeatureElement);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mappedFeatureElement"></param>
    public void Update(XElement mappedFeatureElement)
    {
      // empty
    }
  }
}
