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
    public string RecordingsService { get; set; }

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

    private static ClientConfig Create()
    {
      DateTime dateTime = DateTime.Now;
      int year = dateTime.Year;

      var result = new ClientConfig
        {
          RecordingsService = "https://atlas.cyclomedia.com/recordings/wfs",
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
