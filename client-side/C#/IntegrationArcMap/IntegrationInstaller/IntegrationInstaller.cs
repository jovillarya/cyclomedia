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
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Threading;

namespace IntegrationInstaller
{
  [RunInstaller(true)]
  public partial class IntegrationInstaller : Installer
  {
    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    public IntegrationInstaller()
    {
      InitializeComponent();
    }

    #endregion

    #region install / uninstall

    // =========================================================================
    // Install / Uninstall
    // =========================================================================
    public override void Install(IDictionary stateSaver)
    {
      base.Install(stateSaver);
      string cmd1 = string.Format("\"{0}\\ArcGIS\\bin\\ESRIRegAsm.exe" + "\"",
                                  Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
      Thread.Sleep(1000);
      string part1 = Context.Parameters["arg1"];
      const string part2 = " /p:Desktop /s";
      string cmd2 = string.Format("\"{0}\"{1}", part1, part2);
      ExecuteCommand(cmd1, cmd2, 10000);
    }

    public override void Uninstall(IDictionary savedState)
    {
      base.Uninstall(savedState);
      string cmd1 = string.Format("\"{0}\\ArcGIS\\bin\\ESRIRegAsm.exe" + "\"",
                                  Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
      string part1 = Context.Parameters["arg1"];
      const string part2 = " /p:Desktop /u /s";
      string cmd2 = string.Format("\"{0}\"{1}", part1, part2);
      ExecuteCommand(cmd1, cmd2, 10000);
    }

    #endregion

    #region execute command

    // =========================================================================
    // Execute command
    // =========================================================================
    private static void ExecuteCommand(string command1, string command2, int timeout)
    {
      var processInfo = new ProcessStartInfo(command1, command2) {CreateNoWindow = true, UseShellExecute = false};
      Process process = Process.Start(processInfo);

      if (process != null)
      {
        process.WaitForExit(timeout);
        process.Close();
      }
    }

    #endregion
  }
}
