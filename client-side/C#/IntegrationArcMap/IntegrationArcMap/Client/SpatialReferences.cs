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
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace IntegrationArcMap.Client
{
  [XmlType(AnonymousType = true, Namespace = "https://www.globespotter.com/gsc")]
  [XmlRoot(Namespace = "https://www.globespotter.com/gsc", IsNullable = false)]
  public class SpatialReferences : List<SpatialReference>
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private static readonly XmlSerializer XmlSpatialReferences;
    private static readonly Web Web;

    private static SpatialReferences _spatialReferences;

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    static SpatialReferences()
    {
      XmlSpatialReferences = new XmlSerializer(typeof (SpatialReferences));
      Web = Web.Instance;
    }

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
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

    public static SpatialReferences Instance
    {
      get
      {
        if ((_spatialReferences == null) || (_spatialReferences.Count == 0))
        {
          try
          {
            Load();
          }
          // ReSharper disable once EmptyGeneralCatchClause
          catch
          {
          }
        }

        return _spatialReferences ?? (_spatialReferences = Create());
      }
    }

    #endregion

    #region functions (public)

    // =========================================================================
    // Functions (Public)
    // =========================================================================
    public SpatialReference GetItem(string srsName)
    {
      return this.Aggregate<SpatialReference, SpatialReference>
        (null, (current, spatialReference) => (spatialReference.SRSName == srsName) ? spatialReference : current);
    }

    #endregion

    #region functions (static)

    // =========================================================================
    // Functions (Static)
    // =========================================================================
    public static SpatialReferences Load()
    {
      Stream spatialRef = Web.DownloadSpatialReferences();

      if (spatialRef != null)
      {
        spatialRef.Position = 0;
        _spatialReferences = (SpatialReferences) XmlSpatialReferences.Deserialize(spatialRef);
        spatialRef.Close();
      }

      return _spatialReferences;
    }

    #endregion

    #region functions (private static)

    // =========================================================================
    // Functions (Private static)
    // =========================================================================
    private static SpatialReferences Create()
    {
      return new SpatialReferences();
    }

    #endregion
  }
}
