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
using IntegrationArcMap.Model.Capabilities;
using IntegrationArcMap.Model.Shape;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Point = IntegrationArcMap.Model.Shape.Point;

namespace IntegrationArcMap.Model.Wfs
{
  /// <summary>
  /// This class contains the wfs feature information
  /// </summary>
  public class WfsFeature : IMappedFeature
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private Dictionary<string, esriFieldType> _fields;
    private esriGeometryType _esriGeometryType;
    private string _shapeFieldName;

    private Dictionary<string, string> _values; 
    private readonly FeatureType _featureType;

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public Dictionary<string, esriFieldType> Fields
    {
      get { return _fields ?? (_fields = new Dictionary<string, esriFieldType>()); }
    }

    public Dictionary<string, string> Values
    {
      get { return _values ?? (_values = new Dictionary<string, string>()); }
    }

    public string ObjectId
    {
      get { return "Id"; }
    }

    public XName Name
    {
      get
      {
        XName result = null;

        if (_featureType != null)
        {
          var name = _featureType.Name;

          if (name != null)
          {
            result = name.XName;
          }
        }

        return result;
      }
    }

    public string ShapeFieldName
    {
      get { return _shapeFieldName; }
    }

    public esriGeometryType EsriGeometryType
    {
      get { return _esriGeometryType; }
    }

    public string Id { get; private set; }
    public IShape Shape { get; private set; }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    /// <summary>
    /// Default empty constructor
    /// </summary>
    public WfsFeature()
    {
      _shapeFieldName = string.Empty;
      _esriGeometryType = esriGeometryType.esriGeometryNull;
    }

    /// <summary>
    /// Constructor with xml parsing
    /// </summary>
    /// <param name="mappedFeatureElement">xml</param>
    /// <param name="featureType">featureType</param>
    public WfsFeature(XElement mappedFeatureElement, FeatureType featureType)
    {
      _featureType = featureType;
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
      Id = string.Empty;
      Shape = null;
      Values.Clear();

      if (mappedFeatureElement != null)
      {
        XAttribute mappedFeatureAttribute = mappedFeatureElement.Attribute(Namespaces.GmlNs + "id");
        Id = (mappedFeatureAttribute == null) ? null : mappedFeatureAttribute.Value.Trim();
        Values.Add("Id", Id);

        if (!Fields.ContainsKey("Id"))
        {
          Fields.Add("Id", esriFieldType.esriFieldTypeString);
        }

        var elements = mappedFeatureElement.Elements();

        foreach (var xElement in elements)
        {
          if (xElement != null)
          {
            XName xName = xElement.Name;
            string localName = xName.LocalName;
            var xSubElements = xElement.Elements();
            bool geom = false;

            foreach (var xSubElement in xSubElements)
            {
              if (xSubElement != null)
              {
                if (xSubElement.Name == (Namespaces.GmlNs + "Point"))
                {
                  _shapeFieldName = localName;
                  Shape = new Point(xElement);
                  geom = true;
                }

                if ((_esriGeometryType == esriGeometryType.esriGeometryNull) && (Shape != null))
                {
                  switch (Shape.Type)
                  {
                    case ShapeType.Line:
                      _esriGeometryType = esriGeometryType.esriGeometryLine;
                      break;
                    case ShapeType.Point:
                      _esriGeometryType = esriGeometryType.esriGeometryPoint;
                      break;
                    case ShapeType.Polygon:
                      _esriGeometryType = esriGeometryType.esriGeometryPolygon;
                      break;
                    case ShapeType.None:
                      _esriGeometryType = esriGeometryType.esriGeometryNull;
                      break;
                  }
                }
              }
            }

            if (!geom)
            {
              Values.Add(localName, xElement.Value);

              if (!Fields.ContainsKey(localName))
              {
                Fields.Add(localName, esriFieldType.esriFieldTypeString);
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// This function returns the value of a field.
    /// </summary>
    /// <param name="name">The name of the field</param>
    /// <returns>The value</returns>
    public object FieldToItem(string name)
    {
      object result = null;

      if (Values.ContainsKey(name))
      {
        result = Values[name];
      }

      return result;
    }

    #endregion
  }
}
