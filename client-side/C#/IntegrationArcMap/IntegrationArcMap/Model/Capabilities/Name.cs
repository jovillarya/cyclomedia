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
  /// This class contains the name of a wfs layer
  /// </summary>
  public class Name
  {
    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public XNamespace Namespace { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }

    public XName XName
    {
      get { return (Namespace + Type); }
    }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    /// <summary>
    /// Default empty constructor
    /// </summary>
    public Name()
    {
      // empty
    }

    /// <summary>
    /// Constructor with xml parsing
    /// </summary>
    /// <param name="element">xml</param>
    public Name(XElement element)
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
      XElement name = element.Element(Namespaces.WfsNs + "Name");

      if (name != null)
      {
        Value = name.Value;
        string[] valueSplit = Value.Split(':');

        if (valueSplit.Length >= 2)
        {
          string namespaceName = valueSplit[0].Trim();
          Namespace = name.GetNamespaceOfPrefix(namespaceName);
          Type = valueSplit[1].Trim();
        }
      }
    }

    #endregion
  }
}
