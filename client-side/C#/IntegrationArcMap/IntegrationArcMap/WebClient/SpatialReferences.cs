using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace IntegrationArcMap.WebClient
{
  [XmlTypeAttribute(AnonymousType = true, Namespace = "https://www.globespotter.com/gsc")]
  [XmlRootAttribute(Namespace = "https://www.globespotter.com/gsc", IsNullable = false)]
  public class SpatialReferences : List<SpatialReference>
  {
    private static readonly XmlSerializer XmlSpatialReferences;
    private static SpatialReferences _spatialReferences;

    static SpatialReferences()
    {
      XmlSpatialReferences = new XmlSerializer(typeof (SpatialReferences));
    }

    public static SpatialReferences Instance
    {
      get
      {
        if (_spatialReferences == null)
        {
          Load();
        }

        return _spatialReferences ?? (_spatialReferences = Create());
      }
    }

    public static SpatialReferences Load()
    {
      Assembly thisAssembly = Assembly.GetExecutingAssembly();
      const string manualPath = @"IntegrationArcMap.Doc.GSSpatialReferences.xml";
      Stream manualStream = thisAssembly.GetManifestResourceStream(manualPath);

      if (manualStream != null)
      {
        _spatialReferences = (SpatialReferences) XmlSpatialReferences.Deserialize(manualStream);
        manualStream.Close();
      }

      return _spatialReferences;
    }

    public SpatialReference[] SpatialReference
    {
      get { return ToArray(); }
      set
      {
        if (value != null)
        {
          AddRange(value);
        }
      }
    }

    public SpatialReference GetItem(string srsName)
    {
      return this.Aggregate<SpatialReference, SpatialReference>
        (null, (current, spatialReference) => (spatialReference.SRSName == srsName) ? spatialReference : current);
    }

    private static SpatialReferences Create()
    {
      return new SpatialReferences();
    }
  }
}
