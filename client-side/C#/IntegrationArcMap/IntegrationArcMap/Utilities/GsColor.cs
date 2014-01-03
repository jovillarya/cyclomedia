using System;
using System.Collections.Generic;
using System.Drawing;

namespace IntegrationArcMap.Utilities
{
  class GsColor
  {
    private static readonly IList<Color> Colors = new List<Color>
    {
      Color.FromArgb(0x80B3FF),
      Color.FromArgb(0x0067FF),
      Color.FromArgb(0x405980),
      Color.FromArgb(0x001F4D),
      Color.FromArgb(0xFFD080),
      Color.FromArgb(0xFFA100),
      Color.FromArgb(0x806840),
      Color.FromArgb(0x4D3000),
      Color.FromArgb(0xDDFF80),
      Color.FromArgb(0xBBFF00),
      Color.FromArgb(0x6F8040),
      Color.FromArgb(0x384D00),
      Color.FromArgb(0xFF80D9),
      Color.FromArgb(0xFF00B2),
      Color.FromArgb(0x80406C),
      Color.FromArgb(0x4D0035),
      Color.FromArgb(0xF2F2F2),
      Color.FromArgb(0xBFBFBF),
      Color.FromArgb(0x404040),
      Color.FromArgb(0x000000)
    };

    public static Color GetColor(int year)
    {
      DateTime now = DateTime.Now;
      int nowYear = now.Year;
      int yearDiff = nowYear - year;
      int nrColors = Colors.Count;
      int index = Math.Min(yearDiff, (nrColors - 1));
      return Colors[index];
    }
  }
}
