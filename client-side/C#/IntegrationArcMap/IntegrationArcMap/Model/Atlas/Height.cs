using System.Globalization;
using System.Xml.Linq;

namespace IntegrationArcMap.Model.Atlas
{
  /// <summary>
  /// 
  /// </summary>
  public class Height
  {
    private readonly CultureInfo _ci;

    public string System { get; set; }
    public double Value { get; set; }

    /// <summary>
    /// Default empty constructor
    /// </summary>
    public Height()
    {
      _ci = CultureInfo.InvariantCulture;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mappedFeatureElement"></param>
    public Height(XElement mappedFeatureElement)
    {
      _ci = CultureInfo.InvariantCulture;
      Update(mappedFeatureElement);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mappedFeatureElement"></param>
    public void Update(XElement mappedFeatureElement)
    {
      if (mappedFeatureElement != null)
      {
        XAttribute systemAttribute = mappedFeatureElement.Attribute(Namespaces.GmlNs + "system");
        Value = double.Parse(mappedFeatureElement.Value.Trim(), _ci);

        if (systemAttribute != null)
        {
          System = systemAttribute.Value.Trim();
        }
      }
    }
  }
}
