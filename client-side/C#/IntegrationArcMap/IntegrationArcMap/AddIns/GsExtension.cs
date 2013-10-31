using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using ESRI.ArcGIS.ADF.COMSupport;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using IntegrationArcMap.Forms;
using IntegrationArcMap.Layers;
using IntegrationArcMap.Utilities;
using stdole;

namespace IntegrationArcMap.AddIns
{
  public delegate void LoginReloadDelegate();

  /// <summary>
  /// The extension of the cyclomedia layers
  /// </summary>
  public class GsExtension : Extension
  {
    public static event LoginReloadDelegate LoginReloadEvent;

    private static GsExtension _extension;

    public CycloMediaGroupLayer CycloMediaGroupLayer { get; private set; }

    public bool Enabled
    {
      get { return (State == ExtensionState.Enabled); }
    }

    protected override void OnStartup()
    {
      _extension = this;
      base.OnStartup();
    }

    protected override void OnShutdown()
    {
      try
      {
        _extension = null;

        if (Enabled)
        {
          Uninitialize();
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsExtension.OnShutdown");
      }

      base.OnShutdown();
    }

    protected override bool OnSetState(ExtensionState state)
    {
      try
      {
        State = state;

        if (Enabled)
        {
          var docEvents = ArcUtils.MxDocumentEvents;

          if (docEvents != null)
          {
            docEvents.OpenDocument += OpenDocument;
            docEvents.CloseDocument += CloseDocument;
            docEvents.ActiveViewChanged += OnActiveViewChanged;
          }
        }
        else
        {
          Uninitialize();
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsExtension.OnSetState");
      }

      return base.OnSetState(state);
    }

    protected override ExtensionState OnGetState()
    {
      return State;
    }

    internal bool InsideScale()
    {
      return (CycloMediaGroupLayer != null) && CycloMediaGroupLayer.InsideScale;
    }

    internal static GsExtension GetExtension()
    {
      if (_extension == null)
      {
        try
        {
          // ReSharper disable SuspiciousTypeConversion.Global
          UID extId = new UIDClass {Value = ThisAddIn.IDs.GsExtension};
          _extension = ArcMap.Application.FindExtensionByCLSID(extId) as GsExtension;
          // ReSharper restore SuspiciousTypeConversion.Global
        }
        catch (Exception ex)
        {
          Trace.WriteLine(ex.Message, "GsExtension.GetExtension");
        }
      }

      return _extension;
    }

    public void AddLayers()
    {
      AddLayers(null);
    }

    public void AddLayers(string name)
    {
      if (Enabled)
      {
        if (CycloMediaGroupLayer == null)
        {
          GsShowInCyclorama.AddToMenu();
          GsCycloMediaOptions.AddToMenu();
          FrmCycloMediaOptions.CheckOpenCredentials();
          CycloMediaGroupLayer = new CycloMediaGroupLayer();
        }

        if (!string.IsNullOrEmpty(name))
        {
          CycloMediaGroupLayer.AddLayer(name);
        }
      }
    }

    public void RemoveLayer(string name)
    {
      if (CycloMediaGroupLayer != null)
      {
        CycloMediaGroupLayer.RemoveLayer(name);
      }
    }

    public void RemoveLayers()
    {
      if (CycloMediaGroupLayer != null)
      {
        GsShowInCyclorama.RemoveFromMenu();
        GsCycloMediaOptions.RemoveFromMenu();
        FrmCycloMediaOptions.CloseForm();
        FrmSmartClick.Close();
        CycloMediaGroupLayer cycloLayer = CycloMediaGroupLayer;
        CycloMediaGroupLayer = null;
        cycloLayer.Dispose();
        FrmGlobespotter.ShutDown(true);
      }
    }

    public void Uninitialize()
    {
      RemoveLayers();
      var docEvents = ArcUtils.MxDocumentEvents;

      if (docEvents != null)
      {
        docEvents.OpenDocument -= OpenDocument;
        docEvents.CloseDocument -= CloseDocument;
      }

      FrmCycloMediaOptions.CloseForm();
    }

    private void OpenDocument()
    {
      try
      {
        var arcEvents = ArcUtils.ActiveViewEvents;

        if (arcEvents != null)
        {
          arcEvents.ItemDeleted += ItemDeleted;
          arcEvents.AfterDraw += Afterdraw;
        }

        if (LoginReloadEvent != null)
        {
          LoginReloadEvent();
        }

        if (ContainsCycloMediaLayer())
        {
          AddLayers();
        }

        CycloMediaLayer.LayerRemoveEvent += OnLayerRemoved;
        GsRecentDataLayer.AddToMenu();
        GsHistoricalDataLayer.AddToMenu();
        FrmSmartClick.Close();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsExtension.OpenDocument");
      }
    }

    private void CloseDocument()
    {
      try
      {
        CloseCycloMediaLayer(true);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsExtension.CloseDocument");
      }
    }

    private void ItemDeleted(object item)
    {
      try
      {
        CloseCycloMediaLayer(false);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsExtension.ItemDeleted");
      }
    }

    private bool ContainsCycloMediaLayer()
    {
      // ReSharper disable UseIndexedProperty
      bool result = false;
      IMap map = ArcUtils.Map;
      var layers = map.get_Layers();
      ILayer layer;

      while ((layer = layers.Next()) != null)
      {
        result = ((CycloMediaGroupLayer == null)
                    ? (layer.Name == "CycloMedia")
                    : CycloMediaGroupLayer.IsKnownName(layer.Name)) || result;
      }

      // ReSharper restore UseIndexedProperty
      return result;
    }

    private void CloseCycloMediaLayer(bool closeDocument)
    {
      if (CycloMediaGroupLayer != null)
      {
        if ((!ContainsCycloMediaLayer()) || closeDocument)
        {
          RemoveLayers();
        }
      }

      if (closeDocument)
      {
        var arcEvents = ArcUtils.ActiveViewEvents;

        if (arcEvents != null)
        {
          arcEvents.ItemDeleted -= ItemDeleted;
          arcEvents.AfterDraw -= Afterdraw;
        }

        CycloMediaLayer.LayerRemoveEvent -= OnLayerRemoved;
        GsRecentDataLayer.RemoveFromMenu();
        GsHistoricalDataLayer.RemoveFromMenu();
      }
    }

    private void Afterdraw(IDisplay display, esriViewDrawPhase drawPhase)
    {
      if (drawPhase == esriViewDrawPhase.esriViewForeground)
      {
        if (CycloMediaGroupLayer != null)
        {
          FrmCycloMediaOptions.OpenIfNoCredentials();
          FrmGlobespotter.CheckVisible();
          FrmSmartClick.CheckVisible();
        }
      }
    }

    private void OnLayerRemoved(CycloMediaLayer cycloMediaLayer)
    {
      if (CycloMediaGroupLayer != null)
      {
        if (!CycloMediaGroupLayer.ContainsLayers)
        {
          RemoveLayers();
        }
      }
    }

    private void OnActiveViewChanged()
    {
      try
      {
        IApplication application = ArcMap.Application;

        if (application != null)
        {
          var mxDocument = application.Document as IMxDocument;

          if (mxDocument != null)
          {
            IActiveView activeView = mxDocument.ActiveView;
            var pageLayout = activeView as IPageLayout3;

            if (pageLayout != null)
            {
              var graphicContainer = pageLayout as IGraphicsContainer;

              if (graphicContainer != null)
              {
                IElement mapFrame = null;
                graphicContainer.Reset();
                IElement gElement;

                while ((gElement = graphicContainer.Next()) != null)
                {
                  mapFrame = (gElement is IMapFrame) ? gElement : mapFrame;

                  if (!(gElement is IMapFrame))
                  {
                    graphicContainer.DeleteElement(gElement);
                    graphicContainer.Reset();
                  }
                }

                if (mapFrame != null)
                {
                  IEnvelope mapEnvelope = pageLayout.Page.PrintableBounds;

                  if (mapEnvelope != null)
                  {
                    double gap = mapFrame.Geometry.Envelope.XMin;
                    double xMin = mapEnvelope.XMin + gap;
                    double xMax = mapEnvelope.XMax - gap;
                    double yMin = mapEnvelope.YMin + gap;
                    List<Bitmap> bitmaps = FrmGlobespotter.GetViewerScreenShot();
                    int i = 0;

                    foreach (var bitmap in bitmaps)
                    {
                      double aspRat = (double) bitmap.Width/bitmap.Height;
                      double xDif = (xMax - xMin)/bitmaps.Count;
                      double yMax = (xDif/aspRat) + gap;

                      IEnvelope envelope = new EnvelopeClass
                        {
                          XMin = xMin + (xDif*i),
                          XMax = xMin + (xDif*(i + 1)),
                          YMin = yMin,
                          YMax = yMax,
                        };

                      mapFrame.Geometry = new EnvelopeClass
                        {
                          XMax = xMax,
                          XMin = xMin,
                          YMin = yMax,
                          YMax = mapEnvelope.YMax - gap
                        };

                      var geometry = envelope as IGeometry;
                      geometry.SpatialReference = new UnknownCoordinateSystemClass();

                      var pictElement = new PictureElementClass();
                      var olePictElem = pictElement as IOlePictureElement;

                      // ReSharper disable CSharpWarnings::CS0612
                      var pictDisp = OLE.GetIPictureDispFromBitmap(bitmap) as IPictureDisp;
                      olePictElem.ImportPicture(pictDisp);
                      // ReSharper restore CSharpWarnings::CS0612

                      var element = pictElement as IElement;
                      element.Geometry = geometry;
                      graphicContainer.AddElement(element, 0);
                      i++;
                    }

                    if (bitmaps.Count == 0)
                    {
                      mapFrame.Geometry = new EnvelopeClass
                        {
                          XMax = xMax,
                          XMin = xMin,
                          YMin = yMin,
                          YMax = mapEnvelope.YMax - gap
                        };
                    }
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsExtension.OnActiveViewChanged");
      }
    }
  }
}
