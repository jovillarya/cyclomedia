using System.Collections.Generic;
using System.Xml.Linq;

namespace IntegrationArcMap.Model.Capabilities
{
  /// <summary>
  /// 
  /// </summary>
  public class FeatureType
  {
    public Name Name { get; set; }
    public string Title { get; set; }
    public string DefaultSrs { get; set; }

    public List<string> OtherSrs;
    public OutputFormats OutputFormats;
    public BBoundingBox BBoundingBox;

    public static XName TypeName
    {
      get { return (Namespaces.WfsNs + "FeatureType"); }
    }

    /// <summary>
    /// Default empty constructor
    /// </summary>
    public FeatureType()
    {
      // empty
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mappedFeatureElement"></param>
    public FeatureType(XElement mappedFeatureElement)
    {
      Update(mappedFeatureElement);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    public void Update(XElement element)
    {
      if (element != null)
      {
        Name = new Name(element);
        XElement titleElement = element.Element(Namespaces.WfsNs + "Title");
        XElement defaultSrsElement = element.Element(Namespaces.WfsNs + "DefaultSRS");
        var otherSrsElements = element.Elements(Namespaces.WfsNs + "OtherSRS");
        OutputFormats = new OutputFormats(element);
        BBoundingBox = new BBoundingBox(element);

        Title = (titleElement == null) ? null : titleElement.Value.Trim();
        DefaultSrs = (defaultSrsElement == null) ? null : defaultSrsElement.Value.Trim();
        OtherSrs = new List<string>();

        foreach (var otherSrsElement in otherSrsElements)
        {
          if (otherSrsElement != null)
          {
            OtherSrs.Add(otherSrsElement.Value.Trim());
          }
        }
      }
    }
  }
}
