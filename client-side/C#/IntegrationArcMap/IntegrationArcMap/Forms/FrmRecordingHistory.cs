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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using IntegrationArcMap.Client;
using IntegrationArcMap.Utilities;

namespace IntegrationArcMap.Forms
{
  public delegate void DateRangeChangedDelegate();

  public sealed partial class FrmRecordingHistory : Form
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    public static event DateRangeChangedDelegate DateRangeChanged;

    private static FrmRecordingHistory _frmRecordingHistory;
    private static SortedDictionary<int, int> _yearMonth;

    private readonly CultureInfo _ci;
    private readonly Config _config;

    #endregion

    #region functions (static)

    // =========================================================================
    // Functions (static)
    // =========================================================================
    public static void OnChangeDateRange(SortedDictionary<int, int> yearMonth)
    {
      _yearMonth = yearMonth;

      if (_frmRecordingHistory != null)
      {
        _frmRecordingHistory.ChangeDateRange();
      }
    }

    public static void OpenCloseSwitch()
    {
      if (_frmRecordingHistory == null)
      {
        _frmRecordingHistory = new FrmRecordingHistory();
        var application = ArcMap.Application;
        int hWnd = application.hWnd;
        IWin32Window parent = new WindowWrapper(hWnd);
        _frmRecordingHistory.Show(parent);
      }
      else
      {
        _frmRecordingHistory.Close();
      }
    }

    public static void CloseForm()
    {
      if (_frmRecordingHistory != null)
      {
        _frmRecordingHistory.Close();
      }
    }

    public static bool IsVisible
    {
      get { return _frmRecordingHistory != null; }
    }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    static FrmRecordingHistory()
    {
      _yearMonth = null;
    }

    public FrmRecordingHistory()
    {
      InitializeComponent();
      _config = Config.Instance;
      _ci = CultureInfo.InvariantCulture;
      Font font = SystemFonts.MenuFont;
      Font = (Font) font.Clone();
      rsRecordingSelector.LabelFont = (Font) font.Clone();
      ChangeDateRange();
    }

    #endregion

    #region event handlers

    // =========================================================================
    // Event handlers
    // =========================================================================
    private void rsRecordingSelector_MouseUp(object sender, MouseEventArgs e)
    {
      string yearFrom, yearTo;
      rsRecordingSelector.QueryRange(out yearFrom, out yearTo);
      int mFrom = CalculateMonth(yearFrom);
      int mTo = CalculateMonth(yearTo);
      yearFrom = CalculateYear(yearFrom);
      yearTo = CalculateYear(yearTo);
      int yFrom, yTo;

      if ((int.TryParse(yearFrom, out yFrom)) && (int.TryParse(yearTo, out yTo)))
      {
        if ((_config.YearFrom != yFrom) || (_config.YearTo != yTo || (_config.MonthFrom != mFrom) || (_config.MonthTo != mTo)))
        {
          _config.YearFrom = yFrom;
          _config.YearTo = yTo;
          _config.MonthFrom = mFrom;
          _config.MonthTo = mTo;
          _config.Save();

          if (DateRangeChanged != null)
          {
            DateRangeChanged();
          }
        }
      }
    }

    private void FrmRecordingHistory_FormClosed(object sender, FormClosedEventArgs e)
    {
      _frmRecordingHistory = null;
    }

    private void FrmRecordingHistory_Load(object sender, EventArgs e)
    {
      ChangeDateRange();
    }

    #endregion

    #region functions (private)

    // =========================================================================
    // Functions (private)
    // =========================================================================
    private void ChangeDateRange()
    {
      int yFrom = _config.YearFrom;
      int yTo = _config.YearTo;
      int mFrom = _config.MonthFrom;
      int mTo = _config.MonthTo;
      int? hyFrom = GetElementAt(true);
      int? hyTo = GetElementAt(false);
      string rangeValues = string.Empty;
      int yearFrom = (((hyFrom != null) && (hyFrom < yFrom)) ? (int) hyFrom : yFrom) - 1;
      int yearTo = (((hyTo != null) && (hyTo > yTo)) ? (int) hyTo : yTo) + 1;
      var step = (int) Math.Floor((double) (yearTo - yearFrom)/16) + 1;
      bool foundFrom = false;
      bool foundTo = false;
      int yearRange = yearTo - yearFrom;
      Graphics graphics = rsRecordingSelector.CreateGraphics();
      RectangleF rectangle = graphics.VisibleClipBounds;

      if (yearRange >= 1)
      {
        while ((!foundFrom) || (!foundTo))
        {
          rangeValues = string.Empty;
          var bitmap = new Bitmap((int) rectangle.Width, 10);
          var gapLeft = (int) rsRecordingSelector.GapFromLeftMargin;
          var calculateWidth = (int) (bitmap.Width - gapLeft - rsRecordingSelector.GapFromRightMargin);

          using (var g = Graphics.FromImage(bitmap))
          {
            g.Clear(Color.Transparent);

            if (lblPoints.Image != null)
            {
              lblPoints.Image.Dispose();
            }

            for (int i = yearFrom; i <= yearTo; i = i + step)
            {
              rangeValues = string.Format("{0}{1}{2}", rangeValues, ((i == yearFrom) ? string.Empty : ","), i);
              foundFrom = (i == yFrom) || foundFrom;
              foundTo = (i == yTo) || foundTo;

              for (int j = i; j < (i + step); j++)
              {
                if ((j >= (yearFrom + 1)) && (j <= (yearTo - 1)))
                {
                  int? month = ((_yearMonth != null) && _yearMonth.ContainsKey(j)) ? (int?) _yearMonth[j] : null;

                  if (month != null)
                  {
                    yearRange = yearTo - yearFrom;
                    int pos = j - yearFrom;
                    Color color = FrmGlobespotter.GetColor(j);
                    int x = ((calculateWidth*((12*pos) + (int) month))/(12*yearRange)) + gapLeft;
                    color = Color.FromArgb(255, color);
                    var pen = new Pen(color, 1);
                    Brush brush = new SolidBrush(color);
                    g.DrawEllipse(pen, (x - 3), 3, 6, 6);
                    g.FillEllipse(brush, (x - 3), 3, 6, 6);
                  }
                }
              }
            }
          }

          lblPoints.Image = bitmap;
          yFrom = foundFrom ? yFrom : (yFrom - 1);
          yTo = foundTo ? yTo : (yTo + 1);
        }

        rsRecordingSelector.RangeValues = rangeValues;
        string monthFrom = CalculateMonthString(mFrom);
        string monthTo = CalculateMonthString(mTo);
        rsRecordingSelector.Range1 = yFrom.ToString(_ci) + monthFrom;
        rsRecordingSelector.Range2 = yTo.ToString(_ci) + monthTo;
      }
    }

    private int? GetElementAt(bool start)
    {
      return ((_yearMonth == null) || (_yearMonth.Count == 0))
               ? null
               : (int?) _yearMonth.ElementAt(start ? 0 : (_yearMonth.Count - 1)).Key;
    }

    private int CalculateMonth(string year)
    {
      string month = (year.Length <= 5) ? string.Empty : year.Substring(5);
      return (month == "25") ? 4 : (month == "5" ? 7 : (month == "75" ? 10 : 1));
    }

    private string CalculateYear(string year)
    {
      return year.Substring(0, Math.Min(year.Length, 4));
    }

    private string CalculateMonthString(int month)
    {
      string monthString = (Math.Floor((double) (month - 1)/3)*25).ToString(_ci);
      return (monthString == "0") ? string.Empty : ((monthString == "50") ? ".5" : ("." + monthString));
    }

    #endregion
  }
}
