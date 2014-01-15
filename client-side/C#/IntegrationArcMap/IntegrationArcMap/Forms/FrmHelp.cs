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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using IntegrationArcMap.Utilities;
using PDFLibNet;

namespace IntegrationArcMap.Forms
{
  public partial class FrmHelp : Form
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private static FrmHelp _frmHelp;
    private PDFWrapper _pdfDoc;
    private readonly CultureInfo _ci;

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    public FrmHelp()
    {
      InitializeComponent();
      _ci = CultureInfo.InvariantCulture;
      Font font = SystemFonts.MenuFont;
      txtNumber.Font = (Font) font.Clone();
    }

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public static bool IsVisible
    {
      get { return _frmHelp != null; }
    }

    #endregion

    #region functions

    // =========================================================================
    // Functions
    // =========================================================================
    public static void OpenCloseSwitch()
    {
      if (_frmHelp == null)
      {
        OpenForm();
      }
      else
      {
        CloseForm();
      }
    }

    public static void CloseForm()
    {
      if (_frmHelp != null)
      {
        _frmHelp.Close();
      }
    }

    private static void OpenForm()
    {
      if (_frmHelp == null)
      {
        _frmHelp = new FrmHelp();
        var application = ArcMap.Application;
        int hWnd = application.hWnd;
        IWin32Window parent = new WindowWrapper(hWnd);
        _frmHelp.Show(parent);
      }
    }

    private void LoadPage()
    {
      if (_pdfDoc != null)
      {
        _pdfDoc.UseMuPDF = true;
        _pdfDoc.RenderPage(pctHelp.Handle);
        _pdfDoc.CurrentX = 0;
        _pdfDoc.CurrentY = 0;
        _pdfDoc.ClientBounds = new Rectangle(0, 0, _pdfDoc.PageWidth, _pdfDoc.PageHeight);
        var backbuffer = new Bitmap(_pdfDoc.PageWidth, _pdfDoc.PageHeight);

        using (Graphics g = Graphics.FromImage(backbuffer))
        {
          _pdfDoc.DrawPageHDC(g.GetHdc());
          g.ReleaseHdc();
        }

        pctHelp.Image = backbuffer;
        int page = _pdfDoc.CurrentPage;
        txtNumber.Text = page.ToString(_ci);
        btnPrev.Enabled = page > 1;
        btnNext.Enabled = page < _pdfDoc.PageCount;
      }
    }

    #endregion

    #region event handlers

    // =========================================================================
    // Eventhandlers
    // =========================================================================
    private void FrmHelp_FormClosed(object sender, FormClosedEventArgs e)
    {
      _frmHelp = null;
    }

    private void FrmHelp_Load(object sender, EventArgs e)
    {
      Type thisType = GetType();
      Assembly thisAssembly = Assembly.GetAssembly(thisType);
      const string manualPath = @"IntegrationArcMap.Doc.GlobeSpotter for ArcGIS Desktop User Manual.pdf";
      Stream manualStream = thisAssembly.GetManifestResourceStream(manualPath);

      if (manualStream != null)
      {
        _pdfDoc = new PDFWrapper();
        _pdfDoc.LoadPDF(manualStream);
        _pdfDoc.CurrentPage = 1;
        _pdfDoc.FitToHeight(pctHelp.Handle);
        pctHelp.Width = _pdfDoc.PageWidth;
        LoadPage();
      }
    }

    private void btnNext_Click(object sender, EventArgs e)
    {
      if (_pdfDoc != null)
      {
        _pdfDoc.NextPage();
        LoadPage();
      }
    }

    private void btnPrev_Click(object sender, EventArgs e)
    {
      if (_pdfDoc != null)
      {
        _pdfDoc.PreviousPage();
        LoadPage();
      }
    }

    #endregion
  }
}
