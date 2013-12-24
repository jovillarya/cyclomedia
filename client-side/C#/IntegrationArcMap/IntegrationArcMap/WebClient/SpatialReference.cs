using System;
using System.Xml.Serialization;
using ESRI.ArcGIS.Geometry;

namespace IntegrationArcMap.WebClient
{
  public class SpatialReference
  {
    private ISpatialReference _spatialReference;

    // ReSharper disable InconsistentNaming
    public string Name { get; set; }

    public string SRSName { get; set; }

    public string ESRICompatibleName { get; set; }
    // ReSharper restore InconsistentNaming

    [XmlIgnore]
    public ISpatialReference SpatialRef
    {
      get
      {
        if (_spatialReference == null)
        {
          int srs;
          string strsrs = SRSName.Replace("EPSG:", string.Empty);

          if (int.TryParse(strsrs, out srs))
          {
            ISpatialReferenceFactory3 spatialRefFactory = new SpatialReferenceEnvironmentClass();

            try
            {
              _spatialReference = spatialRefFactory.CreateProjectedCoordinateSystem(srs);
            }
            catch (ArgumentException)
            {
              try
              {
                _spatialReference = spatialRefFactory.CreateGeographicCoordinateSystem(srs);
              }
              catch (ArgumentException)
              {
                _spatialReference = null;
              }
            }
          }
        }

        return _spatialReference;
      }
    }

    [XmlIgnore]
    public bool KnownInArcMap
    {
      get { return SpatialRef != null; }
    }

    public override string ToString()
    {
      return string.IsNullOrEmpty(ESRICompatibleName) ? Name : ESRICompatibleName;
    }
  }
}
