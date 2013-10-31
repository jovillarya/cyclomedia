using System.Xml.Linq;

namespace IntegrationArcMap.Model
{
  /// <summary>
  /// Stores the name spaces used to query the input xml document.
  /// Using GeoSciML 2.0
  /// </summary>
  public class Namespaces
  {
    /// <summary>
    /// The Web Feature Service namespace
    /// </summary>
    public static XNamespace WfsNs = XNamespace.Get("http://www.opengis.net/wfs");

    /// <summary>
    ///
    /// </summary>
    public static XNamespace OwsNs = XNamespace.Get("http://www.opengis.net/ows");

    /// <summary>
    /// The Geography Markup Language namespace
    /// </summary>
    public static XNamespace GmlNs = XNamespace.Get("http://www.opengis.net/gml");

    /// <summary>
    /// The GeoSciML namespace - version 2.0
    /// </summary>
    public static XNamespace AtlasNs = XNamespace.Get("http://www.cyclomedia.com/atlas");

    /// <summary>
    /// The xlink namespace
    /// </summary>
    public static XNamespace XlinkNs = XNamespace.Get("http://www.w3.org/1999/xlink");
  }
}
