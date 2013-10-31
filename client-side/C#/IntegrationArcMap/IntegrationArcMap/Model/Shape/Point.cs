using System.Globalization;
using System.Xml.Linq;

namespace IntegrationArcMap.Model.Shape
{
  /// <summary>
  /// GSML2 Point representation
  /// </summary>
  public class Point : BaseShape
  {
    private readonly CultureInfo _ci;

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Point()
    {
      _ci = CultureInfo.InvariantCulture;
      Type = ShapeType.Point;
      X = 0.0;
      Y = 0.0;
      Z = 0.0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mappedFeatureElement"></param>
    public Point(XElement mappedFeatureElement)
    {
      _ci = CultureInfo.InvariantCulture;
      Type = ShapeType.Point;
      Update(mappedFeatureElement);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mappedFeatureElement"></param>
    public new void Update(XElement mappedFeatureElement)
    {
      XElement pointElement = mappedFeatureElement.Element(Namespaces.GmlNs + "Point");

      if (pointElement != null)
      {
        XElement posElement = pointElement.Element(Namespaces.GmlNs + "pos");
        XAttribute idAttribute = pointElement.Attribute(Namespaces.GmlNs + "id");
        XAttribute srsAttribute = pointElement.Attribute("srsName");

        if (idAttribute != null)
        {
          Id = idAttribute.Value;
        }

        if (srsAttribute != null)
        {
          SrsName = srsAttribute.Value;
        }

        if (posElement != null)
        {
          string position = posElement.Value.Trim();
          string[] values = position.Split(' ');
          X = (values.Length >= 1) ? double.Parse(values[0], _ci) : 0.0;
          Y = (values.Length >= 2) ? double.Parse(values[1], _ci) : 0.0;
          Z = (values.Length >= 3) ? double.Parse(values[2], _ci) : 0.0;
        }
      }
    }
  }
}
