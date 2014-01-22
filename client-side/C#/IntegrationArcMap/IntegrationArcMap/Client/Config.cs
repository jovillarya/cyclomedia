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

    private string _baseUrl;
    private bool _agreement;

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

    #endregion
  }
}
