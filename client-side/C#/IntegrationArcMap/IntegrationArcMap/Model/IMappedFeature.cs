using System.Collections.Generic;
using System.Xml.Linq;
using IntegrationArcMap.Model.Shape;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace IntegrationArcMap.Model
{
  public interface IMappedFeature
  {
    Dictionary<string, esriFieldType> Fields { get; }
    string ObjectId { get; }
    XName Name { get; }
    string ShapeFieldName { get; }
    esriGeometryType EsriGeometryType { get; }
    IShape Shape { get; }
    object FieldToItem(string name);
    void Update(XElement mappedFeatureElement);
  }
}
