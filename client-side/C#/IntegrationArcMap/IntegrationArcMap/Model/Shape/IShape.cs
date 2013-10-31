namespace IntegrationArcMap.Model.Shape
{
  /// <summary>
  /// GSML2 shape elmenent interface definition.
  /// </summary>
  public interface IShape
  {
    string Id { get; set; }
    string SrsName { get; set; }
    ShapeType Type { get; set; }
  }
}
