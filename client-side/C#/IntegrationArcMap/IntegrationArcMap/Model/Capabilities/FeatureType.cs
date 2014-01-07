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

namespace IntegrationArcMap.Model.Capabilities
{
  /// <summary>
  /// This class contains the feature type of a wfs layer
  /// </summary>
  public class FeatureType
  {
    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public Name Name { get; set; }
    public string Title { get; set; }
    public string DefaultSrs { get; set; }

    public List<string> OtherSrs { get; set; }
    public OutputFormats OutputFormats { get; set; }
    public BBoundingBox BBoundingBox { get; set; }

    public static XName TypeName
    {
      get { return (Namespaces.WfsNs + "FeatureType"); }
    }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    /// <summary>
    /// Default empty constructor
    /// </summary>
    public FeatureType()
    {
      // empty
    }

    /// <summary>
    /// Constructor with xml parsing
    /// </summary>
    /// <param name="mappedFeatureElement">xml</param>
    public FeatureType(XElement mappedFeatureElement)
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
    /// <param name="element">xml</param>
    public void Update(XElement element)
    {
      if (element != null)
      {
        Name = new Name(element);
        XElement titleElement = element.Element(Namespaces.WfsNs + "Title");
        XElement defaultSrsElement = element.Element(Namespaces.WfsNs + "DefaultSRS");
        var otherSrsElements = element.Elements(Namespaces.WfsNs + "OtherSRS");
        OutputFormats = new OutputFormats(element);
        BBoundingBox = new BBoundingBox(element);

        Title = (titleElement == null) ? null : titleElement.Value.Trim();
        DefaultSrs = (defaultSrsElement == null) ? null : defaultSrsElement.Value.Trim();
        OtherSrs = new List<string>();

        foreach (var otherSrsElement in otherSrsElements)
        {
          if (otherSrsElement != null)
          {
            OtherSrs.Add(otherSrsElement.Value.Trim());
          }
        }
      }
    }

    #endregion
  }
}
