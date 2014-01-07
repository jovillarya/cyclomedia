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

using System.Globalization;
using System.Xml.Linq;

namespace IntegrationArcMap.Model.Shape
{
  /// <summary>
  /// GSML2 Point representation
  /// </summary>
  public class Point : BaseShape
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private readonly CultureInfo _ci;

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    /// <summary>
    /// Default empty constructor
    /// </summary>
    public Point()
    {
      _ci = CultureInfo.InvariantCulture;
      Type = ShapeType.Point;
      X = 0.0;
      Y = 0.0;
      Z = 0.0;
    }

    /// <summary>
    /// Constructor with xml parsing
    /// </summary>
    /// <param name="mappedFeatureElement">xml</param>
    public Point(XElement mappedFeatureElement)
    {
      _ci = CultureInfo.InvariantCulture;
      Type = ShapeType.Point;
      Update(mappedFeatureElement);
    }

    #endregion

    #region functions

    // =========================================================================
    // Functions
    // =========================================================================
    /// <summary>
    /// xml parsing
    /// </summary>
    /// <param name="mappedFeatureElement">xml</param>
    public new void Update(XElement mappedFeatureElement)
    {
      XElement pointElement = mappedFeatureElement.Element(Namespaces.GmlNs + "Point");

      if (pointElement != null)
      {
        XElement posElement = pointElement.Element(Namespaces.GmlNs + "pos");
        XAttribute idAttribute = pointElement.Attribute(Namespaces.GmlNs + "id");
        XAttribute srsAttribute = pointElement.Attribute("srsName");

        if (idAttribute != null)
        {
          Id = idAttribute.Value;
        }

        if (srsAttribute != null)
        {
          SrsName = srsAttribute.Value;
        }

        if (posElement != null)
        {
          string position = posElement.Value.Trim();
          string[] values = position.Split(' ');
          X = (values.Length >= 1) ? double.Parse(values[0], _ci) : 0.0;
          Y = (values.Length >= 2) ? double.Parse(values[1], _ci) : 0.0;
          Z = (values.Length >= 3) ? double.Parse(values[2], _ci) : 0.0;
        }
      }
    }

    #endregion
  }
}
