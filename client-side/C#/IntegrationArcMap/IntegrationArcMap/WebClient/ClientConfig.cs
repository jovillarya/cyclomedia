using System;
using System.IO;
using System.Xml.Serialization;
using IntegrationArcMap.Utilities;

namespace IntegrationArcMap.WebClient
{
  public class ClientConfig
  {
    private static readonly XmlSerializer XmlClientConfig;
    private static ClientConfig _clientConfig;

    private string _baseUrl;

    static ClientConfig()
    {
      XmlClientConfig = new XmlSerializer(typeof(ClientConfig));
    }

    public static string FileName
    {
      get { return Path.Combine(ArcUtils.FileDir, "GSClientConfig.xml"); }
    }

    public static ClientConfig Instance
    {
      get
      {
        if (_clientConfig == null)
        {
          Load();
        }

        return _clientConfig ?? (_clientConfig = Create());
      }
    }

    public static ClientConfig Load()
    {
      if (File.Exists(FileName))
      {
        var streamFile = new FileStream(FileName, FileMode.OpenOrCreate);
        _clientConfig = (ClientConfig) XmlClientConfig.Deserialize(streamFile);
        streamFile.Close();
      }

      return _clientConfig;
    }

    public void Save()
    {
      FileStream streamFile = File.Open(FileName, FileMode.Create);
      XmlClientConfig.Serialize(streamFile, this);
      streamFile.Close();
    }

    /// <summary>
    /// Service info
    /// </summary>
    public string BaseUrl
    {
      get { return (_baseUrl = (string.IsNullOrEmpty(_baseUrl) ? "https://atlas.cyclomedia.com" : _baseUrl)); }
      set { _baseUrl = value; }
    }

    /// <summary>
    /// Cyclorama values
    /// </summary>
    public uint MaxViewers { get; set; }

    /// <summary>
    /// Measurements
    /// </summary>
    public bool SmartClickEnabled { get; set; }

    /// <summary>
    /// year from / to
    /// </summary>
    public int YearFrom { get; set; }
    public int YearTo { get; set; }

    /// <summary>
    /// Cyclorama vector layer values
    /// </summary>
    public uint DistanceCycloramaVectorLayer { get; set; }

    /// <summary>
    /// Detail images
    /// </summary>
    public bool DetailImagesEnabled { get; set; }

    /// <summary>
    /// Spatial references
    /// </summary>
    public SpatialReference SpatialReference { get; set; }

    private static ClientConfig Create()
    {
      DateTime dateTime = DateTime.Now;
      int year = dateTime.Year;

      var result = new ClientConfig
        {
          BaseUrl = "https://atlas.cyclomedia.com",
          MaxViewers = 3,
          DistanceCycloramaVectorLayer = 30,
          SmartClickEnabled = false,
          DetailImagesEnabled = false,
          YearFrom = year - 3,
          YearTo = year - 1
        };

      result.Save();
      return result;
    }
  }
}
