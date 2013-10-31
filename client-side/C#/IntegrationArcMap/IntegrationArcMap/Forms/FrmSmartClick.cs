using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IntegrationArcMap.Symbols;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using GlobeSpotterAPI;

using MeasurementPoint = GlobeSpotterAPI.MeasurementPoint;
using MeasurementPoint2 = IntegrationArcMap.Symbols.MeasurementPoint;

namespace IntegrationArcMap.Forms
{
  public partial class FrmSmartClick : UserControl
  {
    // =========================================================================
    // Members
    // =========================================================================
    private static FrmSmartClick _frmSmartClick;
    private static IDockableWindow _window;

    private readonly Dictionary<int, string> _bitmapImageId; 
    private readonly Dictionary<string, Color> _imageIdColor;

    private MeasurementPoint _measurementPoint;
    private FrmGlobespotter _frmGlobespotter;
    private int _count;
    private bool _finished;
    private bool _readyState;
    private bool _opened;
    private int _entityId;
    private int _pointId;

    public FrmSmartClick()
    {
      InitializeComponent();
      _frmGlobespotter = null;
      _count = 0;
      _finished = false;
      _readyState = false;
      _opened = false;
      _entityId = 0;
      _pointId = 0;
      _measurementPoint = null;
      _imageIdColor = new Dictionary<string, Color>();
      _bitmapImageId = new Dictionary<int, string>();
    }

    // =========================================================================
    // Properties
    // =========================================================================
    private static FrmSmartClick Instance
    {
      get { return _frmSmartClick ?? (_frmSmartClick = new FrmSmartClick()); }
    }

    private static IDockableWindow Window
    {
      get { return _window ?? (_window = GetDocWindow()); }
    }

    public static IntPtr FrmHandle
    {
      get { return Instance.Handle; }
    }

    // =========================================================================
    // Static Functions
    // =========================================================================
    private static IDockableWindow GetDocWindow()
    {
      IApplication application = ArcMap.Application;
      ICommandItem tool = application.CurrentTool;
      const string windowName = "IntegrationArcMap_GsFrmSmartClick";
      IDockableWindow result = ArcUtils.GetDocWindow(windowName);
      application.CurrentTool = tool;
      return result;
    }

    public static void DisposeFrm(bool disposing)
    {
      if (_frmSmartClick != null)
      {
        _frmSmartClick.Dispose(disposing);
      }
    }

    public static void CheckVisible()
    {
      Instance.CheckVis();
    }

    public static void LoadBitmap(Bitmap bitmap, FrmGlobespotter frmGlobespotter, int entityId, int pointId,
                                  MeasurementObservation observation)
    {
      if (!Window.IsVisible())
      {
        Window.Show(true);
      }

      Instance.AddBitmap(bitmap, frmGlobespotter, entityId, pointId, observation);
    }

    public static void RemovedObservation(int entityId, int pointId, string imageId)
    {
      Instance.ObsRemoved(entityId, pointId, imageId);
    }

    public static void AddObservation(int entityId, int pointId, MeasurementObservation observation)
    {
      Instance.AddObs(entityId, pointId, observation);
    }

    public static void UpdateObservation(int entityId, int pointId, MeasurementObservation observation)
    {
      Instance.UpdateObs(entityId, pointId, observation);
    }

    public static void FinishedBitmap(MeasurementPoint measurementPoint)
    {
      Instance.Finished(measurementPoint);
    }

    public static void Close()
    {
      Window.Show(false);
    }

    public static void Enable()
    {
      Instance.EnableButtons();
    }

    public static void OpenMeasurementPoint(int entityId, int pointId, MeasurementPoint measurementPoint)
    {
      Instance.OpenMeasurementP(entityId, pointId, measurementPoint);
    }

    public static void CloseMeasurementPoint(int entityId, int pointId)
    {
      Instance.CloseMeasurementP(entityId, pointId);
    }

    public static void SetImageIdColor(string imageId, Color color)
    {
      Instance.AddImageIdColor(imageId, color);
    }

    public static void DeleteImageIdColor(string imageId)
    {
      Instance.RemoveImageIdColor(imageId);
    }

    // =========================================================================
    // Private Functions
    // =========================================================================
    private void CloseMeasurementP(int entityId, int pointId)
    {
      if ((entityId == _entityId) && (pointId == _pointId))
      {
        _opened = false;
      }
    }

    private void OpenMeasurementP(int entityId, int pointId, MeasurementPoint measurementPoint)
    {
      _measurementPoint = measurementPoint;
      _entityId = entityId;
      _pointId = pointId;
      _opened = true;
    }

    private void CheckVis()
    {
      if (_readyState)
      {
        if (!Window.IsVisible())
        {
          Window.Show(true);
        }
      }
    }

    private void EnableButtons()
    {
      btnCheck.Enabled = true;
      btnDelete.Enabled = true;
    }

    private void ObsRemoved(int entityId, int pointId, string imageId)
    {
      if ((_entityId == entityId) && (pointId == _pointId))
      {
        bool found = false;

        for (int i = 0; ((i < lvObservations.Items.Count) && (!found)); i++)
        {
          ListViewItem item = lvObservations.Items[i];

          if (item.Text == imageId)
          {
            if (_bitmapImageId.ContainsValue(imageId))
            {
              int id = _bitmapImageId.Aggregate(-1,
                                                (current, bitmapImageId) =>
                                                (bitmapImageId.Value == imageId) ? bitmapImageId.Key : current);

              if (id != -1)
              {
                _bitmapImageId.Remove(id);
                SetBitmap(id, null);
              }
            }

            lvObservations.Items.Remove(item);
            found = true;
          }
        }

        RedrawObservationList();
      }
    }

    private void AddImageIdColor(string imageId, Color color)
    {
      if (_imageIdColor.ContainsKey(imageId))
      {
        _imageIdColor[imageId] = color;
      }
      else
      {
        _imageIdColor.Add(imageId, color);
      }

      RedrawObservationList();
    }

    private void RemoveImageIdColor(string imageId)
    {
      if (_imageIdColor.ContainsKey(imageId))
      {
        _imageIdColor.Remove(imageId);
      }

      RedrawObservationList();
    }

    private void SetBitmap(int id, Bitmap bitmap)
    {
      switch (id)
      {
        case 0:
          pctImageResult1.Image = bitmap;
          break;
        case 1:
          pctImageResult2.Image = bitmap;
          break;
        case 2:
          pctImageResult3.Image = bitmap;
          break;
      }
    }

    private void AddBitmap(Bitmap bitmap, FrmGlobespotter frmGlobespotter, int entityId, int pointId,
                           MeasurementObservation observation)
    {
      string imageId = observation.imageId;

      if (!_bitmapImageId.ContainsKey(_count))
      {
        _bitmapImageId.Add(_count, imageId);
      }
      else
      {
        _bitmapImageId[_count] = imageId;
      }

      SetBitmap(_count, bitmap);

      if (!(_opened && ((_entityId == entityId) && (_pointId == pointId))))
      {
        _entityId = entityId;
        _pointId = pointId;
        _opened = false;
      }

      _frmGlobespotter = frmGlobespotter;
      _finished = false;
      _count++;
      btnCheck.Enabled = false;
      btnDelete.Enabled = false;
      _readyState = false;

      lvObservations.Visible = true;
      AddItemListView(observation);
      lvObservations.Visible = false;
    }

    private void AddItemListView(MeasurementObservation observation)
    {
      string imageId = observation.imageId;
      var items = new[] {imageId, "X"};
      var listViewItem = new ListViewItem(items) {Tag = observation};
      lvObservations.Items.Add(listViewItem);
    }

    private void RedrawObservationList()
    {
      if (lvObservations.Items.Count >= 1)
      {
        lvObservations.RedrawItems(0, lvObservations.Items.Count - 1, true);
      }
    }

    private void AddObs(int entityId, int pointId, MeasurementObservation observation)
    {
      if ((_entityId == entityId) && (_pointId == pointId))
      {
        AddItemListView(observation);
        RedrawObservationList();
      }
    }

    private void UpdateObs(int entityId, int pointId, MeasurementObservation observation)
    {
      if ((_entityId == entityId) && (_pointId == pointId))
      {
        int i = 0;
        bool found = false;
        MeasurementObservation obs = null;

        while ((i < lvObservations.Items.Count) && (!found))
        {
          ListViewItem item = lvObservations.Items[i];
          obs = item.Tag as MeasurementObservation;

          if (obs != null)
          {
            found = obs.imageId == observation.imageId;
          }

          if (!found)
          {
            i++;
          }
        }

        if (found)
        {
          lvObservations.Items[i].Tag = obs;
        }
      }
    }

    private void Finished(MeasurementPoint measurementPoint)
    {
      _measurementPoint = measurementPoint;

      if (!_finished)
      {
        pctImageResult1.Image = (_count == 0) ? null : pctImageResult1.Image;
        pctImageResult2.Image = (_count <= 1) ? null : pctImageResult2.Image;
        pctImageResult3.Image = (_count <= 2) ? null : pctImageResult3.Image;
        _finished = true;
        _count = 0;
        _frmGlobespotter.Disable();
        _readyState = true;
      }

      var circle = new Bitmap(16, 16);

      using (var graphics = Graphics.FromImage(circle))
      {
        Brush color = _measurementPoint.reliableEstimate ? Brushes.Green : Brushes.Red;
        graphics.DrawEllipse(new Pen(color, 2), 2, 2, 10, 10);
        graphics.FillEllipse(color, 2, 2, 10, 10);
      }

      RelO.Image = circle;
    }

    private void SelectItem(int id)
    {
      if (_bitmapImageId.ContainsKey(id))
      {
        lvObservations.Visible = true;
        _frmGlobespotter.Enable();
        _frmGlobespotter.DisableMeasurementSeries();
        _frmGlobespotter.OpenMeasurementPoint(_entityId, _pointId);
        lvObservations.SelectedIndices.Clear();
        string imageId = _bitmapImageId[id];

        for (int i = 0; i < lvObservations.Items.Count; i++)
        {
          ListViewItem item = lvObservations.Items[i];
          var observation = item.Tag as MeasurementObservation;

          if (observation != null)
          {
            if (observation.imageId == imageId)
            {
              lvObservations.SelectedIndices.Add(i);
            }
          }
        }
      }
    }

    // =========================================================================
    // Event handlers
    // =========================================================================
    private void btnCheck_Click(object sender, EventArgs e)
    {
      _bitmapImageId.Clear();
      lvObservations.Items.Clear();
      _frmGlobespotter.Enable();
      _frmGlobespotter.CloseMeasurementPoint(_entityId, _pointId);
      _frmGlobespotter.EnableMeasurementSeries(_entityId);
      _frmGlobespotter.MeasurementPointUpdated(_entityId, _pointId);
      _readyState = false;
      Close();
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      _frmGlobespotter.Enable();
      _frmGlobespotter.CloseMeasurementPoint(_entityId, _pointId);
      _frmGlobespotter.EnableMeasurementSeries(_entityId);

      if (_opened)
      {
        _opened = false;
        Measurement measurement = Measurement.Get(_entityId);
        var measurementPoints = new List<MeasurementPoint2>();
        bool found = false;

        foreach (var measurementPoint in measurement)
        {
          if (measurementPoint.Key == _pointId)
          {
            found = true;
          }
          else
          {
            if (found)
            {
              measurementPoints.Add(measurementPoint.Value);
            }
          }
        }

        _frmGlobespotter.RemoveMeasurementPoint(_entityId, _pointId);

        foreach (var measurementPoint in measurementPoints)
        {
          _frmGlobespotter.RemoveMeasurementPoint(_entityId, measurementPoint.PointId);
        }

        IPoint point = new PointClass
          {
            X = _measurementPoint.x,
            Y = _measurementPoint.y,
            Z = _measurementPoint.z
          };

        _pointId = _frmGlobespotter.CreateMeasurementPoint(_entityId, point);

        foreach (var measurementPoint in measurementPoints)
        {
          IPoint pointAdd = measurementPoint.Point;

          point = new PointClass
            {
              X = pointAdd.X,
              Y = pointAdd.Y,
              Z = pointAdd.Z
            };

          _frmGlobespotter.CreateMeasurementPoint(_entityId, point);
        }

        _frmGlobespotter.OpenMeasurementPoint(_entityId, _pointId);
      }
      else
      {
        _frmGlobespotter.RemoveMeasurementPoint(_entityId, _pointId);
      }

      _bitmapImageId.Clear();
      lvObservations.Items.Clear();
      _readyState = false;
      Close();
    }

    private void pctImageResult1_Click(object sender, EventArgs e)
    {
      SelectItem(0);
    }

    private void pctImageResult2_Click(object sender, EventArgs e)
    {
      SelectItem(1);
    }

    private void pctImageResult3_Click(object sender, EventArgs e)
    {
      SelectItem(2);
    }

    private void lvObservations_DoubleClick(object sender, EventArgs e)
    {
      foreach (ListViewItem selectedItem in lvObservations.SelectedItems)
      {
        ListViewItem.ListViewSubItem item = selectedItem.SubItems[0];

        if (item != null)
        {
          _frmGlobespotter.Enable();
          _frmGlobespotter.SelectImageMeasurement(item.Text);
        }
      }
    }

    private void lvObservations_MouseClick(object sender, MouseEventArgs e)
    {
      ListViewHitTestInfo info = lvObservations.HitTest(e.Location);

      if (info.SubItem.Text == "X")
      {
        _frmGlobespotter.RemoveMeasurementObservation(_entityId, _pointId, info.Item.Text);
      }
    }

    private void lvObservations_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
    {
      using (var sf = new StringFormat())
      {
        if (e.Header.Text == "Trash")
        {
          var bounds = e.Bounds;
          var size = new Rectangle(bounds.X + (bounds.Width/2) - 8, bounds.Y + (bounds.Height/2) - 8, 16, 16);
          e.Graphics.DrawImage(Properties.Resources.GsDelete, size);
        }

        if (e.Header.Text == "ImageId")
        {
          var bounds = e.Bounds;
          var size = new Rectangle(bounds.X + (bounds.Width/2) - 35, bounds.Y + (bounds.Height/2) - 7, 70, 13);
          e.Graphics.DrawRectangle(new Pen(Brushes.Black, 1), size);

          string imageId = e.SubItem.Text;
          Color color = _imageIdColor.ContainsKey(imageId) ? Color.FromArgb(255, _imageIdColor[imageId]) : Color.Gray;
          Brush brush = new SolidBrush(color);
          e.Graphics.FillRectangle(brush, size);
          sf.Alignment = StringAlignment.Center;

          // Draw the subitem text in red to highlight it. 
          e.Graphics.DrawString(imageId, lvObservations.Font, Brushes.Black, e.Bounds, sf);
        }
      }
    }

    private void lvObservations_DrawItem(object sender, DrawListViewItemEventArgs e)
    {
      if ((e.State & ListViewItemStates.Selected) != 0)
      {
        // Draw the background and focus rectangle for a selected item.
        e.Graphics.FillRectangle(Brushes.Blue, e.Bounds);
        e.DrawFocusRectangle();
      }

      // Draw the item text for views other than the Details view. 
      if (lvObservations.View != View.Details)
      {
        e.DrawText();
      }
    }

    private void lvObservations_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
    {
      using (var sf = new StringFormat())
      {
        // Draw the standard header background.
        e.DrawBackground();

        // Draw the header text. 
        using (var headerFont = new Font("Helvetica", 10, FontStyle.Bold))
        {
          if (e.Header.Text == "Trash")
          {
            var bounds = e.Bounds;
            var size = new Rectangle(bounds.X + (bounds.Width/2) - 8, bounds.Y + (bounds.Height/2) - 8, 16, 16);
            e.Graphics.DrawImage(Properties.Resources.GsDelete, size);
          }
          else
          {
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(e.Header.Text, headerFont, Brushes.Black, e.Bounds, sf);
          }
        }
      }
    }
  }
}
