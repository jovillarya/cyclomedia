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

namespace IntegrationArcMap.Model.Atlas
{
  /// <summary>
  /// This class contains the collection of all the images of one recording
  /// </summary>
  public class Images : List<Image>
  {
    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    /// <summary>
    /// Default empty constructor
    /// </summary>
    public Images()
    {
      // empty
    }

    /// <summary>
    /// Constructor with xml parsing
    /// </summary>
    /// <param name="mappedFeatureElement">xml</param>
    public Images(XElement mappedFeatureElement)
    {
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
      IEnumerable<XElement> elements = mappedFeatureElement.Descendants(Namespaces.AtlasNs + "Image");

      foreach (var xElement in elements)
      {
        var image = new Image(xElement);
        Add(image);
      }
    }

    #endregion
  }
}
