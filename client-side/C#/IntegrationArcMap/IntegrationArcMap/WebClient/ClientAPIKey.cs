using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace IntegrationArcMap.WebClient
{
  // ReSharper disable InconsistentNaming
  public class ClientAPIKey
  {
    private static readonly XmlSerializer XmlClientAPIKey;
    private static ClientAPIKey _clientAPIKey;

    static ClientAPIKey()
    {
      XmlClientAPIKey = new XmlSerializer(typeof (ClientAPIKey));
    }

    public static ClientAPIKey Instance
    {
      get
      {
        if (_clientAPIKey == null)
        {
          Load();
        }

        return _clientAPIKey ?? (_clientAPIKey = Create());
      }
    }

    public static ClientAPIKey Load()
    {
      Assembly thisAssembly = Assembly.GetExecutingAssembly();
      const string manualPath = @"IntegrationArcMap.Doc.GSClientAPIKey.xml";
      Stream manualStream = thisAssembly.GetManifestResourceStream(manualPath);

      if (manualStream != null)
      {
        _clientAPIKey = (ClientAPIKey) XmlClientAPIKey.Deserialize(manualStream);
        manualStream.Close();
      }

      return _clientAPIKey;
    }

    /// <summary>
    /// API Key
    /// </summary>
    public string APIKey { get; set; }

    private static ClientAPIKey Create()
    {
      return new ClientAPIKey
        {
          APIKey = string.Empty
        };
    }

    // ReSharper restore InconsistentNaming
  }
}
