using System.Xml.Linq;

namespace IntegrationArcMap.Model.Capabilities
{
  /// <summary>
  /// GSML2 Point representation
  /// </summary>
  public class Name
  {
    public XNamespace Namespace { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }

    public XName XName
    {
      get { return (Namespace + Type); }
    }

    /// <summary>
    /// 
    /// </summary>
    public Name()
    {
      // empty
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    public Name(XElement element)
    {
      Update(element);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    public void Update(XElement element)
    {
      XElement name = element.Element(Namespaces.WfsNs + "Name");

      if (name != null)
      {
        Value = name.Value;
        string[] valueSplit = Value.Split(':');

        if (valueSplit.Length >= 2)
        {
          string namespaceName = valueSplit[0].Trim();
          Namespace = name.GetNamespaceOfPrefix(namespaceName);
          Type = valueSplit[1].Trim();
        }
      }
    }
  }
}
