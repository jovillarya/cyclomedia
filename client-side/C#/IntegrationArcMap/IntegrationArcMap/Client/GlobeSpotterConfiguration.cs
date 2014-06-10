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
using System.Net;
using System.Xml.Serialization;

namespace IntegrationArcMap.Client
{
  [XmlType(AnonymousType = true, Namespace = "https://www.globespotter.com/gsc")]
  [XmlRoot(Namespace = "https://www.globespotter.com/gsc", IsNullable = false)]
  public class GlobeSpotterConfiguration
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private static readonly XmlSerializer XmlGlobeSpotterconfiguration;
    private static readonly Web Web;

    private static GlobeSpotterConfiguration _globeSpotterConfiguration;

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    static GlobeSpotterConfiguration()
    {
      XmlGlobeSpotterconfiguration = new XmlSerializer(typeof (GlobeSpotterConfiguration));
      Web = Web.Instance;
    }

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    /// <summary>
    /// ApplicationConfiguration
    /// </summary>
    public ApplicationConfiguration ApplicationConfiguration { get; set; }

    [XmlIgnore]
    public bool LoginFailed { get; private set; }

    [XmlIgnore]
    public bool LoadException { get; private set; }

    [XmlIgnore]
    public Exception Exception { get; private set; }

    [XmlIgnore]
    public bool Credentials
    {
      get { return ((ApplicationConfiguration != null) && (ApplicationConfiguration.Functionalities.Length >= 1)); }
    }

    public static bool MeasureSmartClick
    {
      get { return Instance.CheckFunctionality("MeasureSmartClick"); }
    }

    public static bool MeasurePoint
    {
      get { return Instance.CheckFunctionality("MeasurePoint"); }
    }

    public static bool MeasureLine
    {
      get { return Instance.CheckFunctionality("MeasureLine"); }
    }

    public static bool MeasurePolygon
    {
      get { return Instance.CheckFunctionality("MeasurePolygon"); }
    }

    public static bool AddLayerWfs
    {
      get { return Instance.CheckFunctionality("AddLayerWFS"); }
    }

    public static bool MeasurePermissions
    {
      get { return MeasurePoint || MeasureLine || MeasurePolygon || MeasureSmartClick; }
    }

    public static GlobeSpotterConfiguration Instance
    {
      get
      {
        bool loadException = false;
        bool loginFailed = false;
        Exception exception = null;

        if (_globeSpotterConfiguration == null)
        {
          try
          {
            Load();
          }
          catch (WebException ex)
          {
            exception = ex;
            var responce = ex.Response as HttpWebResponse;

            if (responce != null)
            {
              if ((responce.StatusCode == HttpStatusCode.Unauthorized) ||
                  (responce.StatusCode == HttpStatusCode.Forbidden) ||
                  (responce.StatusCode == HttpStatusCode.NotFound))
              {
                loginFailed = true;
              }
              else
              {
                loadException = true;
              }
            }
            else
            {
              loadException = true;
            }
          }
          catch (Exception ex)
          {
            exception = ex;
            loadException = true;
          }
        }

        return _globeSpotterConfiguration ?? (_globeSpotterConfiguration = new GlobeSpotterConfiguration
        {
          LoadException = loadException,
          LoginFailed = loginFailed,
          Exception = exception
        });
      }
    }

    #endregion

    // =========================================================================
    // Functions (Public)
    // =========================================================================
    #region functions (public)

    private bool CheckFunctionality(string name)
    {
      return ((ApplicationConfiguration != null) && (ApplicationConfiguration.GetFunctionality(name) != null));
    }

    #endregion

    #region functions (static)

    // =========================================================================
    // Functions (Static)
    // =========================================================================
    public static GlobeSpotterConfiguration Load()
    {
      Stream globeSpotterConf = Web.DownloadGlobeSpotterConfiguration();

      if (globeSpotterConf != null)
      {
        globeSpotterConf.Position = 0;
        _globeSpotterConfiguration =
          (GlobeSpotterConfiguration) XmlGlobeSpotterconfiguration.Deserialize(globeSpotterConf);
        globeSpotterConf.Close();
      }

      return _globeSpotterConfiguration;
    }

    public static void Delete()
    {
      _globeSpotterConfiguration = null;
    }

    public static bool CheckCredentials()
    {
      Delete();
      return Instance.Credentials;
    }

    #endregion
  }
}
