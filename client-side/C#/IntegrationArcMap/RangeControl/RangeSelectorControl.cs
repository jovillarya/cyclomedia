using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using RangeSelector;

namespace RangeControl
{
  /// <summary>
  /// This is a custom control that allows the user of the control  to select a range of values 
  /// using two "thumbs".  The control allows the client to change the appearance of the control. 
  /// For example, there are design time options to control the size of thumb, the size of the 
  /// middle bar and background color, in-focus color and disabled color of the control etc...
  /// </summary>
  /// 
  [ToolboxBitmap(typeof(RangeSelectorControl), "RangeScale.bmp")]
  public sealed class RangeSelectorControl : UserControl
  {
    /// <ControlVariables>
    /// The Below are Design time (Also Runtime) Control Variables.  These variables 
    /// can be used by the client to change the appearance of the control.  These are
    /// private varibles.  The user of the control will be using the public properties
    /// to change/modify the values.
    /// </ControlVariables>
    /// 
    #region Design Time Control Variables -- Private Variables

    private string _strXmlFileName; // XML File Name that is used for picking up the Label Values
    private string _strRangeString; // The String that is displayed at the bottom of the control.  
    private string _strRange; // An alternate to the XML File Name where the Range Label values are stored
    private Font _fntLabelFont; // Font of the Label
    private readonly FontStyle _fntLabelFontStyle; // Font Style of the Label 
    private readonly float _fLabelFontSize; // Size of the Label 
    private readonly FontFamily _fntLabelFontFamily; // Font Family of the Label 
    private string _strLeftImagePath; // Left Thumb Image Path
    private string _strRightImagePath; // Right Thumb Image Path
    private float _fHeightOfThumb; // Height Of  the Thumb
    private float _fWidthOfThumb; // Width of the Thumb
    private Color _clrThumbColor; // Color of the Thumb, If not Image
    private Color _clrInFocusBarColor; // In Focus Bar Colour
    private Color _clrDisabledBarColor; // Disabled Bar Color
    private Color _clrInFocusRangeLabelColor; // In Focus Range Label Color
    private Color _clrDisabledRangeLabelColor; // Disabled Range label Color
    private uint _unSizeOfMiddleBar; // Thickness of the Middle bar
    private uint _unGapFromLeftMargin; // Gap from the Left Margin to draw the Bar
    private uint _unGapFromRightMargin; // Gap from the Right Margin to draw the Bar
    private string _strDelimiter; // Delimiter used to seperate the Labels in strRange variable
    private string _strRange1; // Thumb 1 Position bar
    private string _strRange2; // Thumb 2 Position in the bar
    private string _strRange1Temp;
    private string _strRange2Temp;
    private readonly Font _fntRangeOutputStringFont; // Range Output string font
    private readonly float _fStringOutputFontSize; // String Output Font Size
    private Color _clrStringOutputFontColor; // Color of the Output Font 
    private readonly FontFamily _fntStringOutputFontFamily; // Font Family to display the Range string

    /// <ControlVariables>
    /// The Above are Design time Control Variables.  These variables can be used by the client
    /// to change the appearance of the control.
    /// </ControlVariables>
    /// 
    #endregion

    /// <ControlProperties>
    /// The Below are Design time (Also Runtime) Control Variable properties.  These variables 
    /// can be used by the client to change the appearance of the control.
    /// </ControlProperties>
    /// 
    #region Design Time Control Properties -- Public -- Design Time User properites  - Can also be changed runtime

    // ReSharper disable CSharpWarnings::CS1570
    /// <XMLFileName>
    /// XMLFileName is a property that can be used to set the Range Labels
    /// For Example:
    /// <?xml version="1.0" encoding="utf-8" ?>
    /// <RangeController>
    ///		<Values>
    /// 		<Value> Excellent</Value>
    /// 		<Value> Good</Value>
    /// 		<Value> Fair</Value>
    /// 		<Value> Poor</Value>
    ///		</Values>
    /// </RangeController>
    /// 
    /// Here the values Excellent, Good, Fair and Poor will be taken as Labels for the 
    /// Control.  
    /// </XMLFileName>
    /// 
    // ReSharper restore CSharpWarnings::CS1570
    public string XmlFileName
    {
      set
      {
        try
        {
          _strXmlFileName = value;

          if (null != _strXmlFileName)
          {
            _xmlTextReader = new XmlTextReader(_strXmlFileName);
            _strRange = null;

            while (_xmlTextReader.Read())
            {
              switch (_xmlTextReader.NodeType)
              {
                case XmlNodeType.Text:
                  _strRange += _xmlTextReader.Value.Trim();
                  _strRange += _strDelimiter;
                  break;
              }
            }

            if (_strRange != null)
            {
              _strRange = _strRange.Remove(_strRange.Length - _strDelimiter.Length, _strDelimiter.Length);
            }

            CalculateValues();
            Refresh();
            OnPaint(_ePaintArgs);
          }
        }
        catch
        {
          _strXmlFileName = null;
          //strRange = "";
          //CalculateValues();
          //Refresh();
          //OnPaint(ePaintArgs);

          MessageBox.Show("The XML Path entered may be invalid (or) The XML file is not well formed", "Error!");
        }
      }
      get
      {
        return _strXmlFileName;
      }
    }

    /// <RangeString>
    /// RangeString is a property that can be used to set the Range String
    /// This is the string that is displayed at the bottom of the control
    /// </RangeString>
    /// 
    public string RangeString
    {
      set
      {
        _strRangeString = value;
        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _strRangeString;
      }
    }

    /// <RangeValues>
    /// Range Values are the values displayed as labels.  These values can be given by the user
    /// seperated by a Delimiter (Usually a comma ',');
    /// </RangeValues>
    /// 
    public string RangeValues
    {
      set
      {
        // Splitting the Range Value to display in the control
        _strSplitLabels = _strRange.Split(_strDelimiter.ToCharArray(), 1024);
        _nNumberOfLabels = _strSplitLabels.Length;
        _strRange = value;

        _strRange1 = _strRange1Temp;
        _strRange2 = _strRange2Temp;

        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _strRange;
      }
    }


    /// <LabelFont>
    /// The user can specify the font to use for the labels. The Setter and getter methods are as below
    /// </LabelFont>
    /// 
    public Font LabelFont
    {
      set
      {
        _fntLabelFont = value;

        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _fntLabelFont;
      }
    }

    /// <LeftThumbImagepath>
    /// The user can specify the Left Thumb Image path to use. The Setter and getter methods are as below
    /// </LeftThumbImagePath>
    /// 
    public string LeftThumbImagePath
    {
      set
      {
        try
        {
          _strLeftImagePath = value;

          CalculateValues();
          Refresh();
          OnPaint(_ePaintArgs);
        }
        catch
        {
          MessageBox.Show("Invalid Image Path.  Please Re-Enter", "Error!");
        }
      }
      get
      {
        return _strLeftImagePath;
      }
    }

    /// <RightThumbImagepath>
    /// The user can specify the Right Thumb Image path to use. The Setter and getter methods are as below
    /// </RightThumbImagePath>
    /// 
    public string RightThumbImagePath
    {
      set
      {
        try
        {
          _strRightImagePath = value;

          CalculateValues();
          Refresh();
          OnPaint(_ePaintArgs);
        }
        catch
        {
          _strRightImagePath = null;
          MessageBox.Show("Invalid Image Path.  Please Re-Enter", "Error!");
        }
      }
      get
      {
        return _strRightImagePath;
      }
    }

    /// <HeightOfThumb>
    /// The user can specify the Height of the Thumb Image path to use. The Setter and getter methods are as below
    /// </HeightOfThumb>
    /// 	   
    public float HeightOfThumb
    {
      set
      {
        _fHeightOfThumb = value;

        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _fHeightOfThumb;
      }
    }

    /// <WidthOfThumb>
    /// The user can specify the Width of the Thumb Image path to use. The Setter and getter methods are as below
    /// </WidthOfThumb>
    ///
    public float WidthOfThumb
    {
      set
      {
        _fWidthOfThumb = value;

        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _fWidthOfThumb;
      }
    }

    /// <InFocusBarColor>
    /// The user can specify the Infocus Bar Color to use. The Setter and getter methods are as below
    /// </InFocusBarColor>
    /// 
    public Color InFocusBarColor
    {
      set
      {
        _clrInFocusBarColor = value;

        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _clrInFocusBarColor;
      }
    }

    /// <DisabledBarColor>
    /// The user can specify the Disabled Bar Color to use. The Setter and getter methods are as below
    /// </DisabledBarColor>
    /// 
    public Color DisabledBarColor
    {
      set
      {
        _clrDisabledBarColor = value;

        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _clrDisabledBarColor;
      }
    }

    /// <ThumbColor>
    /// The user can specify the Thumb Color to use. The Setter and getter methods are as below
    /// </ThumbColor>
    /// 
    public Color ThumbColor
    {
      set
      {
        _clrThumbColor = value;

        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _clrThumbColor;
      }
    }

    /// <InFocusRangeLabelColor>
    /// The user can specify the InFocus Range Label Color to use. The Setter and getter methods are as below
    /// </InFocusRangeLabelColor>
    /// 
    public Color InFocusRangeLabelColor
    {
      set
      {
        _clrInFocusRangeLabelColor = value;

        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _clrInFocusRangeLabelColor;
      }
    }

    /// <DisabledRangeLabelColor>
    /// The user can specify the InFocus Range Label Color to use. The Setter and getter methods are as below
    /// </DisabledRangeLabelColor>
    /// 
    public Color DisabledRangeLabelColor
    {
      set
      {
        _clrDisabledRangeLabelColor = value;

        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _clrDisabledRangeLabelColor;
      }
    }

    /// <SizeOfMiddleBar>
    /// The user can specify the Sizeof Middle Bar to use. The Setter and getter methods are as below
    /// </SizeOfMiddleBar>
    /// 
    public uint MiddleBarWidth
    {
      set
      {
        _unSizeOfMiddleBar = value;

        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _unSizeOfMiddleBar;
      }
    }

    /// <GapFromLeftMargin>
    /// The user can specify the Gap from Left margin. The Setter and getter methods are as below
    /// </GapFromLeftMargin>
    /// 	
    public uint GapFromLeftMargin
    {
      set
      {
        _unGapFromLeftMargin = value;

        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _unGapFromLeftMargin;
      }
    }

    /// <GapFromRightMargin>
    /// The user can specify the Gap from Left margin. The Setter and getter methods are as below
    /// </GapFromRightMargin>
    /// 	
    public uint GapFromRightMargin
    {
      set
      {
        _unGapFromRightMargin = value;

        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _unGapFromRightMargin;
      }
    }

    /// <DelimeterForRange>
    /// The user can specify the Delimiter for the Range Values. The Setter and getter methods are as below
    /// </DelimeterForRange>
    /// 	
    public string DelimiterForRange
    {
      set
      {
        try
        {
          _strDelimiter = value;
          const string strTempString = ")*~`!@#/?\"'][{}=-_+&^%$\\|";

          if (!_strDelimiter.Equals(_strDelimiter.TrimStart(strTempString.ToCharArray())))
          {
            MessageBox.Show("The Delimiter specified is not right", "Error!");
            _strDelimiter = ",";
          }

          CalculateValues();
          Refresh();
          OnPaint(_ePaintArgs);
        }
        catch
        {
          MessageBox.Show("The Delimiter specified is not right", "Error!");
          _strDelimiter = ",";
        }
      }
      get
      {
        return _strDelimiter;
      }
    }

    /// <Range1>
    /// The user can specify the Range1 Value. The Setter and getter methods are as below
    /// </Range1>
    /// 	
    public string Range1
    {
      set
      {
        _strRange1Temp = value;

        if (_strSplitLabels.Length != 0)
        {
          _strRange1 = value;
          CalculateValues();
          Refresh();
          OnPaint(_ePaintArgs);
        }
      }
      get
      {
        return _strRange1Temp;
      }
    }

    /// <Range2>
    /// The user can specify the Range2 Value. The Setter and getter methods are as below
    /// </Range2>
    /// 	
    public string Range2
    {
      set
      {
        _strRange2Temp = value;

        if (_strSplitLabels.Length != 0)
        {
          _strRange2 = value;
          CalculateValues();
          Refresh();
          OnPaint(_ePaintArgs);
        }
      }
      get
      {
        return _strRange2Temp;
      }
    }

    /// <OutputStringFontColor>
    /// The user can specify the Output String Font Color Value. The Setter and getter methods are as below
    /// </OutputStringFontColor>
    /// 	
    public Color OutputStringFontColor
    {
      set
      {
        _clrStringOutputFontColor = value;
        CalculateValues();
        Refresh();
        OnPaint(_ePaintArgs);
      }
      get
      {
        return _clrStringOutputFontColor;
      }
    }

    /// <ControlProperties>
    /// The Above are Design time (Also Runtime) Control Variable properties.  These variables 
    /// can be used by the client to change the appearance of the control.
    /// </ControlProperties>
    /// 
    #endregion

    /// <ProgramVariables>
    /// The Below are Variables used for computation.  
    /// </ProgramVariables>
    /// 
    #region Variables Used for Computation

    private Image _imImageLeft; // Variable for Left Image
    private Image _imImageRight; // Variable for Right Image
    private NotifyClient _objNotifyClient; // This is For Client Notification object
    private bool _bMouseEventThumb1; // Variable for Thumb1Click
    private bool _bMouseEventThumb2; // Variable for Thumb2Click
    private float _fThumb1Point; // Variable to hold Mouse point on Thumb1
    private float _fThumb2Point; // Variable to hold Mouse point on Thumb2

    private float _fLeftCol; // Left Column
    private float _fLeftRow; // Left Row
    private float _fRightCol; // Right Column
    private float _fRightRow; // Right Row
    private float _fTotalWidth; // Total Width
    private float _fDividedWidth; // Divided Width

    private PaintEventArgs _ePaintArgs; // Paint Args
    private int _nNumberOfLabels; // Total Number of Labels
    private string[] _strSplitLabels; // To store the Split Labels
    private readonly PointF[] _ptThumbPoints1; // To Store Thumb Point1
    private readonly PointF[] _ptThumbPoints2; // To Store Thumb2 Point
    private XmlTextReader _xmlTextReader; // XML Reader Class

    private bool _bAnimateTheSlider; // Animate the Control
    private float _fThumbPoint1Prev; // To Store Thumb Point1
    private float _fThumbPoint2Prev; // To Store Thumb2 Point

    private float _fThumbBefore1;
    private float _fThumbBefore2;

    #endregion

    /// <ProgramVariables>
    /// The Below are Variables used for computation.  
    /// </ProgramVariables>
    /// 
    private readonly Container _components = null;

    public RangeSelectorControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      #region Initialization of Variables to its Default Values

      if (null != _strLeftImagePath)
      {
        _imImageLeft = GetImage(_strLeftImagePath);
      }

      if (null != _strRightImagePath)
      {
        _imImageRight = GetImage(_strRightImagePath);
      }

      _objNotifyClient = null;
      _strRangeString = "Range";
      _strDelimiter = ",";  // Because in Germany decimal point is represented as , i.e., "10.50 in US" is "10,50 in Germany"
      _strRange = "0,10,20,30,Good,50,60,70,Great,90,100";
      _strRange1 = "10";
      _strRange2 = "90";
      _strLeftImagePath = null;
      _strRightImagePath = null;
      _fHeightOfThumb = 20.0f;
      _fWidthOfThumb = 10.0f;
      BackColor = Color.LightBlue;
      _clrInFocusBarColor = Color.Magenta;
      _clrDisabledBarColor = Color.Gray;
      _clrInFocusRangeLabelColor = Color.Green;
      _clrDisabledRangeLabelColor = Color.Gray;
      _clrThumbColor = Color.Purple;
      _fStringOutputFontSize = 10.0f;
      _clrStringOutputFontColor = Color.Black;
      _fntStringOutputFontFamily = FontFamily.GenericSerif;
      _fntRangeOutputStringFont = new Font(_fntStringOutputFontFamily, _fStringOutputFontSize);

      _unSizeOfMiddleBar = 3;
      _unGapFromLeftMargin = 10;
      _unGapFromRightMargin = 10;
      _fntLabelFontFamily = FontFamily.GenericSansSerif;
      _fLabelFontSize = 8.25f;
      _fntLabelFontStyle = FontStyle.Bold;
      _fntLabelFont = new Font(_fntLabelFontFamily, _fLabelFontSize, _fntLabelFontStyle);

      _strSplitLabels = new string[1024];
      _ptThumbPoints1 = new PointF[3];
      _ptThumbPoints2 = new PointF[3];

      _bMouseEventThumb1 = false;
      _bMouseEventThumb2 = false;
      _bAnimateTheSlider = false;

      _fThumbBefore1 = 0.0f;
      _fThumbBefore2 = 0.0f;
      #endregion
    }

    /// <InterfacesExposed>
    /// The Below are Interfaces/Methods exposed to the client
    /// </InterfacesExposed>
    /// 
    #region Methods Exposed to client at runtime

    /// <summary>
    /// Returns the image from a resourcePath
    /// </summary>
    /// <param name="resourcePath">the resource path</param>
    /// <returns>image</returns>
    private Image GetImage(string resourcePath)
    {
      Assembly thisAssembly = Assembly.GetAssembly(GetType());
      Stream imageStream = thisAssembly.GetManifestResourceStream(resourcePath);
      return (imageStream != null) ? Image.FromStream(imageStream) : null;
    }

    /// <QueryRange>
    /// The client can query this method to get the range
    /// </QueryRange>
    /// 
    public void QueryRange(out string strGetRange1, out string strGetRange2)
    {
      strGetRange1 = _strRange1;
      strGetRange2 = _strRange2;
    }

    /// <RegisterForChangeEvent>
    /// The client can Register for automatic update whenever the values are changing
    /// </RegisterForChangeEvent>
    /// 
    public void RegisterForChangeEvent(ref NotifyClient refNotifyClient)
    {
      // If there's a valid object, the values are copied.
      try
      {
        if (null != refNotifyClient)
        {
          _objNotifyClient = refNotifyClient;
          _objNotifyClient.Range1 = _strRange1;
          _objNotifyClient.Range2 = _strRange2;
        }
      }
      catch
      {
        MessageBox.Show("The Registered Event object has a Bad memory.  Please correct it", "Error!");
      }
    }

    #endregion

    /// <CalculateValues>
    /// The below is the method that calculates the values to be place while painting
    /// </CalculateValues>
    /// 
    #region This is a Private method that calculates the values to be placed while painting

    private void CalculateValues()
    {
      try
      {
        // Creating the Graphics object
        Graphics myGraphics = CreateGraphics();

        // Split the Labels to be displayed below the Bar
        _strSplitLabels = _strRange.Split(_strDelimiter.ToCharArray(), 1024);
        _nNumberOfLabels = _strSplitLabels.Length;

        // If there's an image load the Image from the file
        if (null != _strLeftImagePath)
        {
          _imImageLeft = GetImage(_strLeftImagePath);
        }

        if (null != _strRightImagePath)
        {
          _imImageRight = GetImage(_strRightImagePath);
        }

        // Calculate the Left, Right values based on the Clip region bounds
        RectangleF recRegion = myGraphics.VisibleClipBounds;
        _fLeftCol = _unGapFromLeftMargin;
        _fLeftRow = recRegion.Height / 2.0f;  // To display the Bar in the middle
        _fRightCol = recRegion.Width - _unGapFromRightMargin;
        _fRightRow = _fLeftRow;
        _fThumb1Point = _fLeftCol;
        _fThumb2Point = _fRightCol;
        _fTotalWidth = recRegion.Width - (_unGapFromRightMargin + _unGapFromLeftMargin);
        _fDividedWidth = _fTotalWidth / (_nNumberOfLabels - 1);

        int nRangeIndex1Selected = 0;
        int nRangeIndex2Selected = _nNumberOfLabels - 1;

        // This is used to calculate the Thumb Point from the  Range1, Range2 Value
        for (int nIndexer = 0; nIndexer < _nNumberOfLabels; nIndexer++)
        {
          if (_strRange1.Equals(_strSplitLabels[nIndexer]))
          {
            _fThumb1Point = _fLeftCol + _fDividedWidth * nIndexer;
            nRangeIndex1Selected = nIndexer;
          }

          if (_strRange2.Equals(_strSplitLabels[nIndexer]))
          {
            _fThumb2Point = _fLeftCol + _fDividedWidth * nIndexer;
            nRangeIndex2Selected = nIndexer;
          }
        }

        if (_strRange1 == _strRange2)
        {
          if (nRangeIndex1Selected != 0)
          {
            _fThumb1Point -= _fDividedWidth / 2.0f;
          }

          if (nRangeIndex2Selected != _nNumberOfLabels - 1)
          {
            _fThumb2Point += _fDividedWidth / 2.0f;
          }
        }

        // This is for Calculating the final Thumb points
        _ptThumbPoints1[0].X = _fThumb1Point;
        _ptThumbPoints1[0].Y = _fLeftRow - 3.0f;
        _ptThumbPoints1[1].X = _fThumb1Point;
        _ptThumbPoints1[1].Y = _fLeftRow - 3.0f - _fHeightOfThumb;
        _ptThumbPoints1[2].X = (_fThumb1Point + _fWidthOfThumb);
        _ptThumbPoints1[2].Y = _fLeftRow - 3.0f - _fHeightOfThumb / 2.0f;

        _ptThumbPoints2[0].X = _fThumb2Point;
        _ptThumbPoints2[0].Y = _fRightRow - 3.0f;
        _ptThumbPoints2[1].X = _fThumb2Point;
        _ptThumbPoints2[1].Y = _fRightRow - 3.0f - _fHeightOfThumb;
        _ptThumbPoints2[2].X = _fThumb2Point - _fWidthOfThumb;
        _ptThumbPoints2[2].Y = _fRightRow - 3.0f - _fHeightOfThumb / 2.0f;
      }
      catch
      {
        //throw;
        //MessageBox.Show("An unexpected Error occured.  Please contact the vendor of this control", "Error");
      }
    }

    /// <CalculateValues>
    /// The below is the method that calculates the values to be place while painting
    /// </CalculateValues>
    /// 
    #endregion

    /// <Paint >
    /// The below is the method that draws the control on the screen
    /// </Paint >
    /// 
    #region Paint Method Override -- This method draws the control on the screen

    private void OnPaintDrawSliderAndBar(Graphics myGraphics, PaintEventArgs e)
    {
      Brush brSolidBrush;

      // If Interesting mouse event happened on the Thumb1 Draw Thumb1
      if (_bMouseEventThumb1)
      {
        brSolidBrush = new SolidBrush(BackColor);

        if (null != _strLeftImagePath)
        {
          myGraphics.FillRectangle(brSolidBrush, _ptThumbPoints1[0].X, _ptThumbPoints1[1].Y, _fWidthOfThumb, _fHeightOfThumb);
        }
        else
        {
          myGraphics.FillClosedCurve(brSolidBrush, _ptThumbPoints1, FillMode.Winding, 0f);
        }
      }

      //if interesting mouse event happened on Thumb2 draw thumb2
      if (_bMouseEventThumb2)
      {
        brSolidBrush = new SolidBrush(BackColor);

        if (null != _strRightImagePath)
        {
          myGraphics.FillRectangle(brSolidBrush, _ptThumbPoints2[2].X, _ptThumbPoints2[1].Y, _fWidthOfThumb, _fHeightOfThumb);
        }
        else
        {
          myGraphics.FillClosedCurve(brSolidBrush, _ptThumbPoints2, FillMode.Winding, 0f);
        }
      }

      // The Below lines are to draw the Thumb and the Lines 
      // The Infocus and the Disabled colors are drawn properly based
      // onthe  calculated values
      Pen myPen;

      _ptThumbPoints1[0].X = _fThumb1Point;
      _ptThumbPoints1[1].X = _fThumb1Point;
      _ptThumbPoints1[2].X = _fThumb1Point + _fWidthOfThumb;

      _ptThumbPoints2[0].X = _fThumb2Point;
      _ptThumbPoints2[1].X = _fThumb2Point;
      _ptThumbPoints2[2].X = _fThumb2Point - _fWidthOfThumb;

      if (Parent != null)
      {
        brSolidBrush = new SolidBrush(Parent.BackColor);
        myPen = new Pen(Parent.BackColor, _unSizeOfMiddleBar);

        if (Math.Abs(_fThumbBefore1) > 0)
        {
          myGraphics.DrawRectangle(myPen, _fThumbBefore1, _ptThumbPoints1[1].Y, _fWidthOfThumb, _fHeightOfThumb);
          myGraphics.FillRectangle(brSolidBrush, _fThumbBefore1, _ptThumbPoints1[1].Y, _fWidthOfThumb, _fHeightOfThumb);
        }

        if (Math.Abs(_fThumbBefore2) > 0)
        {
          myGraphics.DrawRectangle(myPen, _fThumbBefore2, _ptThumbPoints2[1].Y, _fWidthOfThumb, _fHeightOfThumb);
          myGraphics.FillRectangle(brSolidBrush, _fThumbBefore2, _ptThumbPoints2[1].Y, _fWidthOfThumb, _fHeightOfThumb);
        }

        myGraphics.DrawRectangle(myPen, _fLeftCol, _fLeftRow*2 - _fntLabelFont.Size - 5,
                                 _fntLabelFont.Size*_strRangeString.Length, _fntLabelFont.Size + 2);
        myGraphics.FillRectangle(brSolidBrush, _fLeftCol, _fLeftRow*2 - _fntLabelFont.Size - 5,
                                 _fntLabelFont.Size*_strRangeString.Length, _fntLabelFont.Size + 2);
      }

      myPen = new Pen(_clrDisabledBarColor, _unSizeOfMiddleBar);
      myGraphics.DrawLine(myPen, _fLeftCol, _ptThumbPoints1[2].Y, _fThumb1Point - (_fWidthOfThumb / 2), _ptThumbPoints1[2].Y);

      myGraphics.DrawLine(myPen, _fLeftCol, _ptThumbPoints1[2].Y, _fLeftCol, _ptThumbPoints1[2].Y + _fntLabelFont.SizeInPoints);
      myGraphics.DrawLine(myPen, _fRightCol, _ptThumbPoints1[2].Y, _fRightCol, _ptThumbPoints1[2].Y + _fntLabelFont.SizeInPoints);

      brSolidBrush = new SolidBrush(_clrStringOutputFontColor);
      myGraphics.DrawString(_strRangeString, _fntLabelFont, brSolidBrush, _fLeftCol,
                            _fLeftRow*2 - _fntLabelFont.Size - 7);

      myPen = new Pen(_clrInFocusBarColor, _unSizeOfMiddleBar);
      myGraphics.DrawLine(myPen, _ptThumbPoints1[2].X - (_fWidthOfThumb / 2), _ptThumbPoints1[2].Y, _fThumb2Point - (_fWidthOfThumb / 2), _ptThumbPoints1[2].Y);

      myPen = new Pen(_clrDisabledBarColor, _unSizeOfMiddleBar);
      myGraphics.DrawLine(myPen, _fThumb2Point + (_fWidthOfThumb / 2), _ptThumbPoints2[2].Y, _fRightCol, _ptThumbPoints2[2].Y);

      // This loop is to draw the Labels on the screen.
      for (int nIndexer = 0; nIndexer < _nNumberOfLabels; nIndexer++)
      {
        float fDividerCounter = _fLeftCol + _fDividedWidth * nIndexer;

        Color color = ((fDividerCounter >= _ptThumbPoints1[2].X) && (fDividerCounter <= _fThumb2Point))
                        ? _clrInFocusBarColor
                        : _clrDisabledBarColor;
        myPen = new Pen(color, _unSizeOfMiddleBar);
        myGraphics.DrawLine(myPen,
                            fDividerCounter,
                            _ptThumbPoints1[2].Y - _fntLabelFont.SizeInPoints,
                            fDividerCounter,
                            _ptThumbPoints1[2].Y + _fntLabelFont.SizeInPoints);
      }

      // If the Thumb is an Image it draws the Image or else it draws the Thumb
      if (null != _strLeftImagePath)
      {
        _fThumbBefore1 = _ptThumbPoints1[0].X - (_fWidthOfThumb / 2);
        myGraphics.DrawImage(_imImageLeft, _fThumbBefore1, _ptThumbPoints1[1].Y, _fWidthOfThumb, _fHeightOfThumb);
      }
      else
      {
        brSolidBrush = new SolidBrush(_clrThumbColor);
        myGraphics.FillClosedCurve(brSolidBrush, _ptThumbPoints1, FillMode.Winding, 0f);
      }

      // If the Thumb is an Image it draws the Image or else it draws the Thumb
      if (null != _strRightImagePath)
      {
        _fThumbBefore2 = _ptThumbPoints2[0].X - (_fWidthOfThumb / 2);
        myGraphics.DrawImage(_imImageRight, _fThumbBefore2, _ptThumbPoints2[1].Y, _fWidthOfThumb, _fHeightOfThumb);
      }
      else
      {
        brSolidBrush = new SolidBrush(_clrThumbColor);
        myGraphics.FillClosedCurve(brSolidBrush, _ptThumbPoints2, FillMode.Winding, 0f);
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      {
        try
        {
          // Declaration of the local variables that are used.

          // Initialization of the local variables.
          Graphics myGraphics = CreateGraphics();
          _ePaintArgs = e;
          Brush brSolidBrush = new SolidBrush(_clrDisabledRangeLabelColor);
          string strNewRange1 = null;
          string strNewRange2 = null;

          // This loop is to draw the Labels on the screen.
          for (int nIndexer = 0; nIndexer < _nNumberOfLabels; nIndexer++)
          {
            float fDividerCounter = _fLeftCol + _fDividedWidth * nIndexer;
            float fIsThumb1Crossed = fDividerCounter + _strSplitLabels[nIndexer].Length * _fntLabelFont.SizeInPoints / 2;
            float fIsThumb2Crossed = fDividerCounter - (_strSplitLabels[nIndexer].Length - 1) * _fntLabelFont.SizeInPoints / 2;

            if (fIsThumb1Crossed >= _fThumb1Point && strNewRange1 == null)
            {
              // If Thumb1 Crossed this Label Make it in Focus color
              brSolidBrush = new SolidBrush(_clrInFocusRangeLabelColor);
              strNewRange1 = _strSplitLabels[nIndexer];
            }

            if (fIsThumb2Crossed > _fThumb2Point)
            {
              // If Thumb2 crossed this draw the labes following this in disabled color
              brSolidBrush = new SolidBrush(_clrDisabledRangeLabelColor);
              //strNewRange2	= strSplitLabels[nIndexer];
            }
            else
            {
              strNewRange2 = _strSplitLabels[nIndexer];
            }

            myGraphics.DrawString(_strSplitLabels[nIndexer], _fntLabelFont, brSolidBrush,
                                  fDividerCounter - ((_fntLabelFont.SizeInPoints)*_strSplitLabels[nIndexer].Length)/2,
                                  _fLeftRow);
          }

          // This is to draw exactly the Range String like "Range 10 to 100" 
          // This will draw the information only if there is a change. 
          if (strNewRange1 != null && strNewRange2 != null &&
            (!_strRange1.Equals(strNewRange1) || !_strRange2.Equals(strNewRange2)) ||
            (!_bMouseEventThumb1 && !_bMouseEventThumb2))
          {
            string strRangeOutput = string.Format("{0} - {1}", _strRange1, _strRange2);

            if (Parent != null)
            {
              brSolidBrush = new SolidBrush(Parent.BackColor);
              myGraphics.DrawString(strRangeOutput, _fntLabelFont, brSolidBrush,
                                    _fLeftCol + _fntLabelFont.Size*_strRangeString.Length,
                                    _fLeftRow*2 - _fntLabelFont.Size - 7);
            }

            brSolidBrush = new SolidBrush(_clrStringOutputFontColor);
            strRangeOutput = string.Format("{0} - {1}", strNewRange1, strNewRange2);
            myGraphics.DrawString(strRangeOutput, _fntLabelFont, brSolidBrush,
                                  _fLeftCol + _fntLabelFont.Size * _strRangeString.Length,
                                  _fLeftRow * 2 - _fntLabelFont.Size - 7);

            _strRange1 = strNewRange1;
            _strRange2 = strNewRange2;
          }

          if (_bAnimateTheSlider)
          {
            float fTempThumb1Point = _fThumb1Point;
            float fTempThumb2Point = _fThumb2Point;
            int nToMakeItTimely = Environment.TickCount;

            for (_fThumb1Point = _fThumbPoint1Prev, _fThumb2Point = _fThumbPoint2Prev;
                _fThumb1Point <= fTempThumb1Point || _fThumb2Point >= fTempThumb2Point;
                _fThumb1Point += 3.0f, _fThumb2Point -= 3.0f)
            {
              _bMouseEventThumb1 = true;
              _bMouseEventThumb2 = true;

              if (_fThumb1Point > fTempThumb1Point)
              {
                _fThumb1Point = fTempThumb1Point;
              }

              if (_fThumb2Point < fTempThumb2Point)
              {
                _fThumb2Point = fTempThumb2Point;
              }

              OnPaintDrawSliderAndBar(myGraphics, e);

              if (Environment.TickCount - nToMakeItTimely >= 1000)
              {
                // Hey its not worth having animation for more than 1 sec.  
                break;
              }

              Thread.Sleep(1);
            }

            _fThumb1Point = fTempThumb1Point;
            _fThumb2Point = fTempThumb2Point;
            _bMouseEventThumb1 = true;
            _bMouseEventThumb2 = true;
            OnPaintDrawSliderAndBar(myGraphics, e);

            _bAnimateTheSlider = false;
            _bMouseEventThumb1 = false;
            _bMouseEventThumb2 = false;
            OnPaintDrawSliderAndBar(myGraphics, e);
          }
          else
          {
            OnPaintDrawSliderAndBar(myGraphics, e);
          }

          // calling the base class.
          base.OnPaint(e);
        }
        catch
        {
          //System.Windows.Forms.MessageBox.Show("An Unexpected Error occured. Please contact the tool vendor", "Error!");
          //throw;
        }
      }
    }

    /// <Paint >
    /// The Above is the method that draws the control on the screen
    /// </Paint >
    #endregion

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (_components != null)
        {
          _components.Dispose();
        }
      }

      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      // 
      // RangeSelectorControl
      // 
      this.Name = "RangeSelectorControl";
      this.Size = new System.Drawing.Size(360, 80);
      this.Resize += new System.EventHandler(this.RangeSelectorControl_Resize);
      this.Load += new System.EventHandler(this.RangeSelectorControl_Load);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RangeSelectorControl_MouseUp);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RangeSelectorControl_MouseMove);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RangeSelectorControl_MouseDown);
    }

    #endregion

    private void RangeSelectorControl_Load(object sender, EventArgs e)
    {
      CalculateValues();
    }

    /// <MouseEvents>
    /// The below are the methods used for handling Mouse Events
    /// </MouseEvents>
    /// 
    #region Methods used for handling Mouse Events

    private void RangeSelectorControl_MouseUp(object sender, MouseEventArgs e)
    {
      // If the Mouse is Up then set the Event to false
      _bMouseEventThumb1 = false;
      _bMouseEventThumb2 = false;

      // Storing these values for animating the slider
      _fThumbPoint1Prev = _fThumb1Point;
      _fThumbPoint2Prev = _fThumb2Point;

      CalculateValues();
      _bAnimateTheSlider = true;
      Refresh();
    }

    private void RangeSelectorControl_MouseDown(object sender, MouseEventArgs e)
    {
      float thumbh = _fWidthOfThumb/2;

      // If the Mouse is Down and also on the Thumb1
      if (e.X >= (_ptThumbPoints1[0].X - thumbh) && e.X <= (_ptThumbPoints1[2].X - thumbh) &&
        e.Y >= _ptThumbPoints1[1].Y && e.Y <= _ptThumbPoints1[0].Y)
      {
        _bMouseEventThumb1 = true;
      }
      // Else If the Mouse is Down and also on the Thumb2
      else if (e.X >= (_ptThumbPoints2[2].X + thumbh) && e.X <= (_ptThumbPoints2[0].X + thumbh) &&
        e.Y >= _ptThumbPoints2[1].Y && e.Y <= _ptThumbPoints2[0].Y)
      {
        _bMouseEventThumb2 = true;
      }

      _bAnimateTheSlider = false;
    }

    private void RangeSelectorControl_MouseMove(object sender, MouseEventArgs e)
    {
      // If the Mouse is moved pressing the left button on Thumb1
      if (_bMouseEventThumb1 && e.Button == MouseButtons.Left && e.X >= _fLeftCol)
      {
        // The below code is for handlling the Thumb1 Point
        if (_strRange1.Equals(_strRange2))
        {
          if (e.X < _fThumb1Point)
          {
            _fThumb1Point = e.X;
            OnPaint(_ePaintArgs);
          }
        }
        else if (_fThumb2Point - _fWidthOfThumb > e.X)
        {
          _fThumb1Point = e.X;
          OnPaint(_ePaintArgs);
        }
        else
        {
          _bMouseEventThumb1 = false;
        }
      }
      //Else If the Mouse is moved pressing the left button on Thumb2
      else if (_bMouseEventThumb2 && e.Button == MouseButtons.Left && e.X <= _fRightCol)
      {
        // The below code is for handlling the Thumb1 Point
        if (_strRange1.Equals(_strRange2))
        {
          if (e.X > _fThumb2Point)
          {
            _fThumb2Point = e.X;
            OnPaint(_ePaintArgs);
          }
        }
        else if (_fThumb1Point + _fWidthOfThumb < e.X)
        {
          _fThumb2Point = e.X;
          OnPaint(_ePaintArgs);
        }
        else
        {
          _bMouseEventThumb2 = false;
        }
      }

      // If there is an Object Notification
      if (null != _objNotifyClient)
      {
        _objNotifyClient.Range1 = _strRange1;
        _objNotifyClient.Range2 = _strRange2;
      }
    }

    /// <MouseEvents>
    /// The below are the methods used for handling Mouse Events
    /// </MouseEvents>
    /// 
    #endregion

    /// <RangeSelectorControl_Resize>
    /// The below are the method is used if the form is resized 
    /// </RangeSelectorControl_Resize>
    /// 
    private void RangeSelectorControl_Resize(object sender, EventArgs e)
    {
      CalculateValues();
      Refresh();
      OnPaint(_ePaintArgs);
    }
  }
}
