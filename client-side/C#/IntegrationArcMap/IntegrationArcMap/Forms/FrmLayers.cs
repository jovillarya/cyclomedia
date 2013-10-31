using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using IntegrationArcMap.AddIns;
using IntegrationArcMap.Layers;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;

namespace IntegrationArcMap.Forms
{
  [Guid("09DCD7A9-90BF-4515-9823-E763035762E6")]
  [ClassInterface(ClassInterfaceType.None)]
  [ProgId("TOCCycloMediaLayersCS.TOCCycloMediaLayers")]
  public partial class FrmLayers : UserControl, IContentsView3
  {
    private IDocumentEvents_Event _docEvents;
    private IApplication _application;
    private object _contextItem;

    private static string RegKey(Type registerType)
    {
      return string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
    }

    [ComRegisterFunction]
    [ComVisible(false)]
    static void RegisterFunction(Type registerType)
    {
      // Required for ArcGIS Component Category Registrar support
      ContentsViews.Register(RegKey(registerType));
    }

    [ComUnregisterFunction]
    [ComVisible(false)]
    static void UnregisterFunction(Type registerType)
    {
      // Required for ArcGIS Component Category Registrar support
      ContentsViews.Unregister(RegKey(registerType));
    }

    public FrmLayers()
    {
      InitializeComponent();
    }

    private Image GetImage(string imagePath)
    {
      Assembly thisAssembly = Assembly.GetAssembly(GetType());
      Stream imageStream = thisAssembly.GetManifestResourceStream(imagePath);
      return (imageStream != null) ? Image.FromStream(imageStream) : null;
    }

    public int Bitmap
    {
      get
      {
        const string imagePath = "IntegrationArcMap.Images.GsCycloMediaLayers.png";
        Image image = GetImage(imagePath);
        var bmp = image as Bitmap;
        return (bmp != null) ? bmp.GetHbitmap().ToInt32() : 0;
      }
    }

    public string Tooltip
    {
      get { return "CycloMedia Layers"; }
    }

    public new bool Visible { get; set; }

    string IContentsView3.Name
    {
      get { return "CycloMedia Layers"; }
    }

    public int hWnd
    {
      get { return Handle.ToInt32(); }
    }

    public object ContextItem
    {
      get { return _contextItem; }
      set { }
    }

    public object SelectedItem
    {
      get { return (tvLayers.SelectedNode != null) ? tvLayers.SelectedNode.Tag : null; }
      set { }
    }

    public bool ProcessEvents { get; set; }

    public bool ShowLines
    {
      get { return tvLayers.ShowLines; }
      set { tvLayers.ShowLines = value; }
    }

    public void Activate(int parentHWnd, IMxDocument mxDocument)
    {
      if (_application == null)
      {
        var document = mxDocument as IDocument;

        if (document != null)
        {
          _application = document.Parent;
          RefreshList();
          _docEvents = document as IDocumentEvents_Event;

          if (_docEvents != null)
          {
            _docEvents.OpenDocument += NewDocument;
            _docEvents.NewDocument += NewDocument;
          }
        }
      }
    }

    public void BasicActivate(int parentHWnd, IDocument document)
    {
    }

    public void Refresh(object item)
    {
      if (item != this)
      {
        //when items are added, removed, reordered
        tvLayers.SuspendLayout();
        RefreshList();
        tvLayers.ResumeLayout();
      }
    }

    public void Deactivate()
    {
    }

    public void AddToSelectedItems(object item)
    {
    }

    public void RemoveFromSelectedItems(object item)
    {
    }

    private void tvLayers_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        //Set item for context menu commands to work with
        _contextItem = e.Node.Tag;

        //Show context menu
        UID menuID = new UIDClass();

        if (_contextItem is IMap) //data frame
        {
          menuID.Value = "{F42891B5-2C92-11D2-AE07-080009EC732A}"; //Data Frame Context Menu (TOC) 
        }
        else //Layer - custom menu
        {
          // ToDo
          // menuID.Value = 
        }

        var cmdBar = (ICommandBar) _application.Document.CommandBars.Find(menuID);
        cmdBar.Popup();
      }
    }

    private void NewDocument()
    {
      IActiveViewEvents_Event activeViewEvents = ArcUtils.ActiveViewEvents;
      activeViewEvents.ContentsChanged += RefreshList;
    }

    private void RefreshList()
    {
      tvLayers.Nodes.Clear();
      var document = _application.Document as IMxDocument;

      if (document != null)
      {
        IMaps maps = document.Maps;
        // ReSharper disable UseIndexedProperty

        for (int i = 0; i < maps.Count; i++)
        {
          IMap map = maps.get_Item(i);
          IEnumLayer layers = map.get_Layers(null, false);
          var nodes = new List<TreeNode>();
          layers.Reset();
          ILayer layer;          

          while ((layer = layers.Next()) != null)
          {
            TreeNode[] children = GetCompositeNodes(layer as ICompositeLayer);
            var node = new TreeNode(layer.Name, children) {Tag = layer, Checked = layer.Visible};
            nodes.Add(node);
          }

          var mapNode = new TreeNode(map.Name, nodes.ToArray()) { Tag = map };
          tvLayers.Nodes.Add(mapNode);
          mapNode.ExpandAll();
        }

        // ReSharper restore UseIndexedProperty
      }
    }

    private TreeNode[] GetCompositeNodes(ICompositeLayer compLayer)
    {
      var result = new List<TreeNode>();

      if (compLayer != null)
      {
        for (int i = 0; i < compLayer.Count; i++)
        {
          ILayer layer = compLayer.Layer[i];
          TreeNode[] children = GetCompositeNodes(layer as ICompositeLayer);

          if (children.Length == 0)
          {
            var nodeList = new List<TreeNode>();
            GsExtension extension = GsExtension.GetExtension();
            CycloMediaGroupLayer cycloGroupLayer = extension.CycloMediaGroupLayer;
            CycloMediaLayer cycloLayer = cycloGroupLayer.GetLayer(layer);

            if (cycloLayer != null)
            {
              var cycloNode = new TreeNode("Cyclorama viewer")
              {
                Tag = cycloLayer,
                Checked = cycloLayer.IsVisibleInGlobespotter
              };

              nodeList.Add(cycloNode);
            }

            children = nodeList.ToArray();
          }

          var node = new TreeNode(layer.Name, children) {Tag = layer, Checked = layer.Visible};
          result.Add(node);
        }
      }

      return result.ToArray();
    }

    private void tvLayers_AfterCheck(object sender, TreeViewEventArgs e)
    {
      TreeNode node = e.Node;
      var layer = node.Tag as ILayer;
      var cycloLayer = node.Tag as CycloMediaLayer;

      if (layer != null)
      {
        layer.Visible = node.Checked;
        IActiveView activeView = ArcUtils.ActiveView;

        if (activeView != null)
        {
          activeView.Refresh();
        }
      }

      if (cycloLayer != null)
      {
        cycloLayer.IsVisibleInGlobespotter = node.Checked;
      }
    }

    private void tvLayers_DrawNode(object sender, DrawTreeNodeEventArgs e)
    {
      TreeNode node = e.Node;

      if (node.Tag is IMap)
      {
        Graphics g = e.Graphics;

        if ((e.State & TreeNodeStates.Selected) != 0)
        {
          Rectangle bounds = e.Bounds;
          bounds.Offset(24, 0);
          Brush brush = new SolidBrush(SystemColors.Highlight);
          g.FillRectangle(brush, bounds);
        }

        const string imagePath = "IntegrationArcMap.Images.GsContentsLayers.png";
        Image image = GetImage(imagePath);
        g.DrawImage(image, 24, 0);
        var tv = sender as TreeView;

        if (tv != null)
        {
          var font = new Font(tv.Font, FontStyle.Bold);
          g.DrawString(node.Text, font, Brushes.Black, 44, 0);
        }
      }
      else
      {
        e.DrawDefault = true;
      }
    }
  }
}
