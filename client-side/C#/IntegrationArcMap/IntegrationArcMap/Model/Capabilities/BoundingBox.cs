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

namespace IntegrationArcMap.Model.Capabilities
{
  /// <summary>
  /// This class contains the bbox of a wfs layer
  /// </summary>
  public class BBoundingBox
  {
    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public Point LowerCorner { get; set; }
    public Point UpperCorner { get; set; }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    /// <summary>
    /// Default empty constructor
    /// </summary>
    public BBoundingBox()
    {
      // empty
    }

    /// <summary>
    /// Constructor with xml parsing
    /// </summary>
    /// <param name="element">xml</param>
    public BBoundingBox(XElement element)
    {
      Update(element);
    }

    #endregion

    #region functions

    // =========================================================================
    // Functions
    // =========================================================================
    /// <summary>
    /// xml parsing
    /// </summary>
    /// <param name="element">xml</param>
    public void Update(XElement element)
    {
      XElement wgs84BoundingBoxElement = element.Element(Namespaces.OwsNs + "WGS84BoundingBox");

      if (wgs84BoundingBoxElement != null)
      {
        XElement lowerCornerElement = wgs84BoundingBoxElement.Element(Namespaces.OwsNs + "LowerCorner");
        XElement upperCornerElement = wgs84BoundingBoxElement.Element(Namespaces.OwsNs + "UpperCorner");
        LowerCorner = new Point(lowerCornerElement);
        UpperCorner = new Point(upperCornerElement);
      }
    }

    #endregion
  }
}
