using System.Globalization;
using System.Xml.Linq;

namespace IntegrationArcMap.Model.Capabilities
{
  /// <summary>
  /// GSML2 Point representation
  /// </summary>
  public class Point
  {
    private readonly CultureInfo _ci;

    public double X { get; set; }
    public double Y { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Point()
    {
      _ci = CultureInfo.InvariantCulture;
      X = 0.0;
      Y = 0.0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    public Point(XElement element)
    {
      _ci = CultureInfo.InvariantCulture;
      Update(element);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    public void Update(XElement element)
    {
      if (element != null)
      {
        string position = element.Value.Trim();
        string[] values = position.Split(' ');
        X = (values.Length >= 1) ? double.Parse(values[0], _ci) : 0.0;
        Y = (values.Length >= 2) ? double.Parse(values[1], _ci) : 0.0;
      }
    }
  }
}
