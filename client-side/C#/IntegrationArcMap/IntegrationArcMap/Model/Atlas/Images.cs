using System.Collections.Generic;
using System.Xml.Linq;

namespace IntegrationArcMap.Model.Atlas
{
  /// <summary>
  /// 
  /// </summary>
  public class Images : List<Image>
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
      IEnumerable<XElement> elements = mappedFeatureElement.Descendants(Namespaces.AtlasNs + "Image");

      foreach (var xElement in elements)
      {
        var image = new Image(xElement);
        Add(image);
      }
    }
  }
}
