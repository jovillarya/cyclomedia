/*
 * Integration in ArcMap for Cycloramas
 * Copyright (c) 2014, CycloMedia, All rights reserved.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */

using System;
using System.IO;
using System.Xml.Serialization;
using IntegrationArcMap.Utilities;

namespace IntegrationArcMap.Client
{
  public delegate void AgreementChangedDelegate(bool value);

  [XmlRoot("ClientConfig")]
  public class Config
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    public static event AgreementChangedDelegate AgreementChangedDelegate;

    private static readonly XmlSerializer XmlConfig;
    private static Config _config;

    private bool? _baseUrlDefault;
    private bool? _recordingsServiceDefault;
    private bool? _swfUrlDefault;
    private bool? _cycloramaVectorLayerLocationDefault;
    private bool? _useProxyServer;
    private bool? _bypassProxyOnLocal;
    private bool? _proxyUseDefaultCredentials;
    private bool _agreement;
    private int _monthFrom;
    private int _monthTo;

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    static Config()
    {
      XmlConfig = new XmlSerializer(typeof(Config));
    }

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    /// <summary>
    /// Service info
    /// </summary>
    public string BaseUrl { get; set; }

    /// <summary>
    /// BaseUrlDefault flag
    /// </summary>
    public bool BaseUrlDefault
    {
      get
      {
        return (_baseUrlDefault == null)
          ? (string.IsNullOrEmpty(BaseUrl) || (BaseUrl == Urls.BaseUrl))
          : ((bool) _baseUrlDefault);
      }
      set { _baseUrlDefault = value; }
    }

    /// <summary>
    /// Recordings Url
    /// </summary>
    public string RecordingsService { get; set; }

    /// <summary>
    /// BaseUrlDefault flag
    /// </summary>
    public bool RecordingsServiceDefault
    {
      get
      {
        return (_recordingsServiceDefault == null)
          ? (string.IsNullOrEmpty(RecordingsService) || (RecordingsService == Urls.RecordingsServiceUrl))
          : ((bool) _recordingsServiceDefault);
      }
      set { _recordingsServiceDefault = value; }
    }

    /// <summary>
    /// swf name
    /// </summary>
    public string SwfUrl { get; set; }

    /// <summary>
    /// swfDefault flag
    /// </summary>
    public bool SwfUrlDefault
    {
      get { return (_swfUrlDefault == null) ? (string.IsNullOrEmpty(SwfUrl)) : ((bool) _swfUrlDefault); }
      set { _swfUrlDefault = value; }
    }

    /// <summary>
    /// Proxy Address
    /// </summary>
    public string ProxyAddress { get; set; }

    /// <summary>
    /// ProxyPort
    /// </summary>
    public int ProxyPort { get; set; }

    /// <summary>
    /// ProxyDefault
    /// </summary>
    public bool UseProxyServer
    {
      get { return (_useProxyServer != null) && ((bool) _useProxyServer); }
      set { _useProxyServer = value; }
    }

    /// <summary>
    /// Proxy username
    /// </summary>
    public string ProxyUsername { get; set; }

    /// <summary>
    /// Proxy password
    /// </summary>
    public string ProxyPassword { get; set; }

    /// <summary>
    /// Proxy domain
    /// </summary>
    public string ProxyDomain { get; set; }

    /// <summary>
    /// Default recording srs system
    /// </summary>
    public string DefaultRecordingSrs { get; set; }

    /// <summary>
    /// ProxyUseDefaultCredentials
    /// </summary>
    public bool ProxyUseDefaultCredentials
    {
      get { return (_proxyUseDefaultCredentials == null) || ((bool) _proxyUseDefaultCredentials); }
      set { _proxyUseDefaultCredentials = value; }
    }

    /// <summary>
    /// Bypass proxy
    /// </summary>
    public bool BypassProxyOnLocal
    {
      get { return (_bypassProxyOnLocal != null) && ((bool) _bypassProxyOnLocal); }
      set { _bypassProxyOnLocal = value; }
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

    public int MonthFrom
    {
      get { return Math.Min(Math.Max(_monthFrom, 1), 12); }
      set { _monthFrom = value; }
    }

    public int MonthTo
    {
      get { return Math.Min(Math.Max(_monthTo, 1), 12); }
      set { _monthTo = value; }
    }

    /// <summary>
    /// Cyclorama vector layer values
    /// </summary>
    public uint DistanceCycloramaVectorLayer { get; set; }

    /// <summary>
    /// CycloramaVectorLayer FGDB default flag
    /// </summary>
    public bool CycloramaVectorLayerLocationDefault
    {
      get
      {
        return (_cycloramaVectorLayerLocationDefault == null)
          ? (string.IsNullOrEmpty(CycloramaVectorLayerLocation))
          : ((bool) _cycloramaVectorLayerLocationDefault);
      }
      set { _cycloramaVectorLayerLocationDefault = value; }
    }

    /// <summary>
    /// Location to store the CycloramaVectorLayer FGDB
    /// </summary>
    public string CycloramaVectorLayerLocation { get; set; }

    /// <summary>
    /// Detail images
    /// </summary>
    public bool DetailImagesEnabled { get; set; }

    /// <summary>
    /// Spatial references
    /// </summary>
    public SpatialReference SpatialReference { get; set; }

    /// <summary>
    /// Agreement
    /// </summary>
    public bool Agreement
    {
      get { return _agreement; }
      set
      {
        _agreement = value;
        AgreementChangedDelegate(_agreement);
      }
    }

    public static string FileName
    {
      get { return Path.Combine(ArcUtils.FileDir, "GSClientConfig.xml"); }
    }

    public static Config Instance
    {
      get
      {
        if (_config == null)
        {
          Load();
        }

        return _config ?? (_config = Create());
      }
    }

    #endregion

    #region functions (public)

    // =========================================================================
    // Functions (Public)
    // =========================================================================
    public void Save()
    {
      FileStream streamFile = File.Open(FileName, FileMode.Create);
      XmlConfig.Serialize(streamFile, this);
      streamFile.Close();
    }

    #endregion

    #region functions (static)

    // =========================================================================
    // Functions (Static)
    // =========================================================================
    public static Config Load()
    {
      if (File.Exists(FileName))
      {
        var streamFile = new FileStream(FileName, FileMode.OpenOrCreate);
        _config = (Config) XmlConfig.Deserialize(streamFile);
        streamFile.Close();
      }

      return _config;
    }

    #endregion

    #region functions (private static)

    // =========================================================================
    // Functions (Private static)
    // =========================================================================
    private static Config Create()
    {
      DateTime dateTime = DateTime.Now;
      int year = dateTime.Year;

      var result = new Config
      {
        BaseUrl = string.Empty,
        _baseUrlDefault = true,
        RecordingsService = string.Empty,
        _recordingsServiceDefault = true,
        SwfUrl = string.Empty,
        _swfUrlDefault = true,
        MaxViewers = 3,
        SmartClickEnabled = true,
        YearFrom = year - 3,
        YearTo = year - 1,
        MonthFrom = 1,
        MonthTo = 1,
        DistanceCycloramaVectorLayer = 30,
        _cycloramaVectorLayerLocationDefault = true,
        CycloramaVectorLayerLocation = string.Empty,
        DetailImagesEnabled = false,
        SpatialReference = null,
        _agreement = false
      };

      result.Save();
      return result;
    }

    #endregion
  }
}
