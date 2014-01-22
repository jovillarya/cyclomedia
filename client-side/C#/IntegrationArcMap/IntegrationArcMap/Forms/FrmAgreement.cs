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
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using IntegrationArcMap.Client;
using IntegrationArcMap.Utilities;

namespace IntegrationArcMap.Forms
{
  public partial class FrmAgreement : Form
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private static FrmAgreement _frmAgreement;
    private readonly Config _config;

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    public FrmAgreement()
    {
      InitializeComponent();
      _config = Config.Instance;
      Font font = SystemFonts.MenuFont;
      txtAgreement.Font = (Font) font.Clone();
    }

    #endregion

    #region functions

    // =========================================================================
    // Functions
    // =========================================================================
    public static void OpenForm()
    {
      if (_frmAgreement == null)
      {
        _frmAgreement = new FrmAgreement();
        var application = ArcMap.Application;
        int hWnd = application.hWnd;
        IWin32Window parent = new WindowWrapper(hWnd);
        _frmAgreement.Show(parent);
      }
    }

    #endregion

    #region event handlers

    // =========================================================================
    // Eventhandlers
    // =========================================================================
    private void FrmHelp_FormClosed(object sender, FormClosedEventArgs e)
    {
      _frmAgreement = null;
    }

    private void FrmHelp_Load(object sender, EventArgs e)
    {
      Type type = GetType();
      Assembly assembly = type.Assembly;
      const string agreementPath = "IntegrationArcMap.Doc.Agreement.txt";
      Stream agreementStream = assembly.GetManifestResourceStream(agreementPath);

      if (agreementStream != null)
      {
        var reader = new StreamReader(agreementStream);
        string agreement = reader.ReadToEnd();
        reader.Close();
        txtAgreement.Text = agreement;
      }

      ckAgreement.Checked = _config.Agreement;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      _config.Agreement = ckAgreement.Checked;
      _config.Save();
      Close();
    }

    private void ckAgreement_CheckedChanged(object sender, EventArgs e)
    {
      btnOk.Enabled = ckAgreement.Checked;
    }

    #endregion
  }
}
