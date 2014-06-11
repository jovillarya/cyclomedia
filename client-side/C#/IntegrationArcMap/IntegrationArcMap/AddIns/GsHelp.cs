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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using IntegrationArcMap.Properties;
using IntegrationArcMap.Utilities;
using Microsoft.Win32;
using Button = ESRI.ArcGIS.Desktop.AddIns.Button;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// This button shows the help document
  /// </summary>
  public class GsHelp : Button
  {
    private Process _process;

    public GsHelp()
    {
      _process = null;
    }

    #region event handlers

    protected override void OnClick()
    {
      try
      {
        OnUpdate();
        var adobePath = Registry.GetValue(@"HKEY_CLASSES_ROOT\Software\Adobe\Acrobat\Exe", string.Empty, string.Empty);

        if (adobePath != null)
        {
          if (_process == null)
          {
            Type thisType = GetType();
            Assembly thisAssembly = Assembly.GetAssembly(thisType);
            const string manualPath = @"IntegrationArcMap.Doc.GlobeSpotter for ArcGIS Desktop User Manual.pdf";
            Stream manualStream = thisAssembly.GetManifestResourceStream(manualPath);
            string fileName = Path.Combine(ArcUtils.FileDir, "Help.pdf");

            if (File.Exists(fileName))
            {
              File.Delete(fileName);
            }

            if (manualStream != null)
            {
              var fileStream = new FileStream(fileName, FileMode.CreateNew);
              const int readBuffer = 2048;
              var buffer = new byte[readBuffer];
              int readBytes;

              do
              {
                readBytes = manualStream.Read(buffer, 0, readBuffer);
                fileStream.Write(buffer, 0, readBytes);
              } while (readBytes != 0);

              fileStream.Flush();
              fileStream.Close();

              var processInfo = new ProcessStartInfo
              {
                FileName = fileName,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
              };

              _process = Process.Start(processInfo);

              if (_process != null)
              {
                _process.EnableRaisingEvents = true;
                _process.Exited += ExitProcess;
              }
            }
          }
          else
          {
            _process.Kill();
          }
        }
        else
        {
          MessageBox.Show(Resources.GsHelp_OnClick_Adobe_reader_is_not_installed_on_your_system__please_first_install_adobe_reader,
            Resources.GsHelp_OnClick_adobe_reader_is_not_installed);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, Resources.GsCycloMediaOptions_OnClick_Globespotter_integration_Addin_Error_);
      }
    }

    protected override void OnUpdate()
    {
      try
      {
        GsExtension extension = GsExtension.GetExtension();
        Enabled = ((ArcMap.Application != null) && extension.Enabled);
        Checked = (_process != null);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsHelp.OnUpdate");
      }
    }

    #endregion

    private void ExitProcess(object sender, EventArgs e)
    {
      _process.Exited -= ExitProcess;
      _process = null;
      OnUpdate();
    }
  }
}
