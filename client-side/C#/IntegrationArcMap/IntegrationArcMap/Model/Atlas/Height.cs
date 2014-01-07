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

namespace IntegrationArcMap.Model.Atlas
{
  /// <summary>
  /// This class contains the height information
  /// </summary>
  public class Height
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
    public string System { get; set; }
    public double Value { get; set; }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    /// <summary>
    /// Default empty constructor
    /// </summary>
    public Height()
    {
      _ci = CultureInfo.InvariantCulture;
    }

    /// <summary>
    /// Constructor with xml parsing
    /// </summary>
    /// <param name="mappedFeatureElement">xml</param>
    public Height(XElement mappedFeatureElement)
    {
      _ci = CultureInfo.InvariantCulture;
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
    public void Update(XElement mappedFeatureElement)
    {
      if (mappedFeatureElement != null)
      {
        XAttribute systemAttribute = mappedFeatureElement.Attribute(Namespaces.GmlNs + "system");
        Value = double.Parse(mappedFeatureElement.Value.Trim(), _ci);

        if (systemAttribute != null)
        {
          System = systemAttribute.Value.Trim();
        }
      }
    }

    #endregion
  }
}
