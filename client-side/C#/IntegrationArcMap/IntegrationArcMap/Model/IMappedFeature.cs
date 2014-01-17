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

using System.Collections.Generic;
using System.Xml.Linq;
using IntegrationArcMap.Model.Shape;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace IntegrationArcMap.Model
{
  /// <summary>
  /// This is the interface for feature information
  /// </summary>
  public interface IMappedFeature
  {
    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    Dictionary<string, esriFieldType> Fields { get; }
    string ObjectId { get; }
    XName Name { get; }
    string ShapeFieldName { get; }
    esriGeometryType EsriGeometryType { get; }
    IShape Shape { get; }

    #endregion

    #region functions

    // =========================================================================
    // Functions
    // =========================================================================
    object FieldToItem(string name);
    void UpdateItem(string name, object item);
    void Update(XElement mappedFeatureElement);

    #endregion
  }
}
