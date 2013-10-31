using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Configuration.Install;
using System.Collections;
using System.Threading;

namespace GsInstaller
{
  [RunInstaller(true)]
  public partial class GsInstaller : Installer
  {
    public GsInstaller()
    {
      InitializeComponent();
    }

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

    private static void ExecuteCommand(string command1, string command2, int timeout)
    {
      var processInfo = new ProcessStartInfo(command1, command2) {CreateNoWindow = true, UseShellExecute = false};
      Process process = Process.Start(processInfo);
      process.WaitForExit(timeout);
      process.Close();
    }
  }
}
