using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using IntegrationArcMap.Forms;
using IntegrationArcMap.Layers;
using GlobeSpotterAPI;

namespace IntegrationArcMap.Symbols
{
  internal class Viewer : IDisposable
  {
    private static readonly Dictionary<uint, Viewer> Viewers;

    private Arrow _arrow;
    private RecordingLocation _location;
    private string _imageId;
    private CycloMediaLayer _layer;

    public string ImageId
    {
      get { return _imageId; }
    }

    static Viewer()
    {
      Viewers = new Dictionary<uint, Viewer>();
    }

    private Viewer(string imageId, CycloMediaLayer layer)
    {
      _arrow = null;
      _location = null;
      _imageId = imageId;
      _layer = layer;
    }

    public void Dispose()
    {
      if (_arrow != null)
      {
        _arrow.Dispose();
        _arrow = null;
      }
    }

    public void Set(RecordingLocation location, double angle, double hFov, Color color)
    {
      if (_arrow != null)
      {
        _arrow.Dispose();
      }

      _location = location;
      _arrow = Arrow.Create(location, angle, hFov, color);
    }

    public void Update(double angle, double hFov)
    {
      if (_arrow != null)
      {
        _arrow.Update(angle, hFov);
      }
    }

    public void Update(string imageId)
    {
      _imageId = imageId;
    }

    public void BlinkArrow()
    {
      _arrow.DrawBlinking();
    }

    public static List<string> ImageIds
    {
      get { return Viewers.Select(viewer => viewer.Value.ImageId).ToList(); }
    }

    public static List<RecordingLocation> Locations
    {
      get { return Viewers.Select(viewer => viewer.Value._location).ToList(); }
    }

    public static Viewer Get(uint viewerId)
    {
      return Viewers[viewerId];
    }

    public static void Clear(FrmGlobespotter globespotter)
    {
      foreach (var viewer in Viewers)
      {
        uint viewerId = viewer.Key;
        globespotter.CloseViewer(viewerId);
        Viewer myViewer = viewer.Value;
        myViewer.Dispose();
      }

      Viewers.Clear();
    }

    public static void Add(uint viewerId, string imageId, CycloMediaLayer layer)
    {
      Viewers.Add(viewerId, new Viewer(imageId, layer));
    }

    public static void Delete(uint viewerId)
    {
      Viewers[viewerId].Dispose();
      Viewers.Remove(viewerId);
    }
  }
}
