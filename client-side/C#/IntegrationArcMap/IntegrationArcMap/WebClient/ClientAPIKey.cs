using System;
using System.IO;
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

    public static string FileName
    {
      get
      {
        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return string.Format(@"{0}\ArcGIS\GSClientAPIKey.xml", folderPath);
      }
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
      if (File.Exists(FileName))
      {
        var streamFile = new FileStream(FileName, FileMode.OpenOrCreate);
        _clientAPIKey = (ClientAPIKey) XmlClientAPIKey.Deserialize(streamFile);
        streamFile.Close();
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
