using System.Xml.Linq;

namespace IntegrationArcMap.Model.Capabilities
{
  /// <summary>
  /// GSML2 Point representation
  /// </summary>
  public class BBoundingBox
  {
    public Point LowerCorner { get; set; }
    public Point UpperCorner { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public BBoundingBox()
    {
      // empty
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    public BBoundingBox(XElement element)
    {
      Update(element);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    public void Update(XElement element)
    {
      XElement wgs84BoundingBoxElement = element.Element(Namespaces.OwsNs + "WGS84BoundingBox");

      if (wgs84BoundingBoxElement != null)
      {
        XElement lowerCornerElement = wgs84BoundingBoxElement.Element(Namespaces.OwsNs + "LowerCorner");
        XElement upperCornerElement = wgs84BoundingBoxElement.Element(Namespaces.OwsNs + "UpperCorner");
        LowerCorner = new Point(lowerCornerElement);
        UpperCorner = new Point(upperCornerElement);
      }
    }
  }
}
