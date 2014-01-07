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

using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace IntegrationArcMap.Client
{
  // ReSharper disable InconsistentNaming
  public class APIKey
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private static readonly XmlSerializer XmlAPIKey;
    private static APIKey _apiKey;

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    static APIKey()
    {
      XmlAPIKey = new XmlSerializer(typeof (APIKey));
    }

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    /// <summary>
    /// API Key
    /// </summary>
    [XmlElement("APIKey")]
    public string Value { get; set; }

    public static APIKey Instance
    {
      get
      {
        if (_apiKey == null)
        {
          Load();
        }

        return _apiKey ?? (_apiKey = Create());
      }
    }

    #endregion

    #region functions (static)

    // =========================================================================
    // Functions (Static)
    // =========================================================================
    public static APIKey Load()
    {
      Assembly thisAssembly = Assembly.GetExecutingAssembly();
      const string manualPath = @"IntegrationArcMap.Doc.APIKey.xml";
      Stream manualStream = thisAssembly.GetManifestResourceStream(manualPath);

      if (manualStream != null)
      {
        _apiKey = (APIKey) XmlAPIKey.Deserialize(manualStream);
        manualStream.Close();
      }

      return _apiKey;
    }

    #endregion

    #region functions (private static)

    // =========================================================================
    // Functions (Private static)
    // =========================================================================
    private static APIKey Create()
    {
      return new APIKey
        {
          Value = string.Empty
        };
    }

    #endregion
  }
  // ReSharper restore InconsistentNaming
}
