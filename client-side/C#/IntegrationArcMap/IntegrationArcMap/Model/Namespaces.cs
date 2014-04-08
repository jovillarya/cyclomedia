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

using System.Xml.Linq;

namespace IntegrationArcMap.Model
{
  /// <summary>
  /// Stores the name spaces used to query the input xml document.
  /// Using GeoSciML 2.0
  /// </summary>
  public class Namespaces
  {
    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    /// <summary>
    /// The Web Feature Service namespace
    /// </summary>
    public static XNamespace WfsNs = XNamespace.Get("http://www.opengis.net/wfs");

    /// <summary>
    ///
    /// </summary>
    public static XNamespace OwsNs = XNamespace.Get("http://www.opengis.net/ows");

    /// <summary>
    /// The Geography Markup Language namespace
    /// </summary>
    public static XNamespace GmlNs = XNamespace.Get("http://www.opengis.net/gml");

    /// <summary>
    /// The GeoSciML namespace - version 2.0
    /// </summary>
    public static XNamespace AtlasNs = XNamespace.Get("http://www.cyclomedia.com/atlas");

    /// <summary>
    /// The xlink namespace
    /// </summary>
    public static XNamespace XlinkNs = XNamespace.Get("http://www.w3.org/1999/xlink");

    /// <summary>
    /// The CycloMedia namespace
    /// </summary>
    public static XNamespace CycloMediaNs = XNamespace.Get("http://atlas.cyclomedia.com");

    /// <summary>
    /// The GlobeSpotter namespace
    /// </summary>
    public static XNamespace GlobeSpotterNs = XNamespace.Get("https://www.globespotter.com/gsc");

    #endregion
  }
}
