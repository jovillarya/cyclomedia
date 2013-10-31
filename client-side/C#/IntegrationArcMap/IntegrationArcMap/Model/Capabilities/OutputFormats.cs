using System.Xml.Linq;

namespace IntegrationArcMap.Model.Capabilities
{
  /// <summary>
  /// GSML2 Point representation
  /// </summary>
  public class OutputFormats
  {
    public string Format { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public OutputFormats()
    {
      // empty
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    public OutputFormats(XElement element)
    {
      Update(element);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    public void Update(XElement element)
    {
      XElement outputformatElement = element.Element(Namespaces.WfsNs + "OutputFormats");

      if (outputformatElement != null)
      {
        XElement formatElement = outputformatElement.Element(Namespaces.WfsNs + "Format");

        if (formatElement != null)
        {
          Format = formatElement.Value;
        }
      }
    }
  }
}
