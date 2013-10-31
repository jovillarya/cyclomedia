using System.Xml.Linq;

namespace IntegrationArcMap.Model.Shape
{
  /// <summary>
  /// Base class for GSML2 shapes (points, lines, polygons)
  /// </summary>
  public class BaseShape : IShape
  {
    /// <summary>
    /// The shape id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The spatial reference name
    /// </summary>
    public string SrsName { get; set; }

    /// <summary>
    /// Shape type getter and setter
    /// </summary>
    public ShapeType Type { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
      // empty
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mappedFeatureElement"></param>
    public void Update(XElement mappedFeatureElement)
    {
      // empty
    }

    /// <summary>
    /// A template method used to save the entity
    /// </summary>
    public void Save()
    {
      // empty
    }
  }
}
