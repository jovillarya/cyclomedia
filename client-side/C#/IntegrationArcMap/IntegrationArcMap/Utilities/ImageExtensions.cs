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

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Drawing.Imaging;
using System.Drawing;

namespace IntegrationArcMap.Utilities
{
  public static class ImageExtensions
  {
    #region functions (static)

    // =========================================================================
    // Functions (Static)
    // =========================================================================
    public static ColorPalette InsertPallette(this ColorPalette palette, List<Color> colors)
    {
      if (colors != null)
      {
        Color[] entries = palette.Entries;
        int i = 0;

        foreach (var color in colors)
        {
          if (i < entries.Length)
          {
            entries[i++] = color;
          }
        }
      }

      return palette;
    }

    public static Bitmap To8BppIndexed(this Bitmap original)
    {
      var rect = new Rectangle(Point.Empty, original.Size);
      const PixelFormat pixelFormat = PixelFormat.Format8bppIndexed;
      var destination = new Bitmap(rect.Width, rect.Height, pixelFormat);

      using (var source = original.Clone(rect, PixelFormat.Format32bppArgb))
      {
        var destinationData = destination.LockBits(rect, ImageLockMode.WriteOnly, pixelFormat);
        var sourceData = source.LockBits(rect, ImageLockMode.ReadOnly, source.PixelFormat);

        var destinationSize = destinationData.Stride * destinationData.Height;
        var destinationBuffer = new byte[destinationSize];

        var sourceSize = sourceData.Stride * sourceData.Height;
        var sourceBuffer = new byte[sourceSize];

        Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, sourceSize);
        source.UnlockBits(sourceData);
        var colors = new List<Color>();

        for (var y = destination.Height; y-- > 0; )
        {
          for (var x = destination.Width; x-- > 0; )
          {
            var sourceIndex = 4 * ((y * destination.Width) + x);
            var color = Color.FromArgb(sourceBuffer[3 + sourceIndex], sourceBuffer[2 + sourceIndex],
                                       sourceBuffer[1 + sourceIndex], sourceBuffer[0 + sourceIndex]);

            if (!colors.Contains(color))
            {
              colors.Add(color);
            }
          }
        }

        destination.Palette = destination.Palette.InsertPallette(colors);

        if (destination.Palette != null)
        {
          var list = destination.Palette.Entries.ToList();

          for (var y = destination.Height; y-- > 0;)
          {
            for (var x = destination.Width; x-- > 0;)
            {
              var pixelIndex = y*destination.Width + x;
              var sourceIndex = 4*pixelIndex;
              var color = Color.FromArgb(sourceBuffer[3 + sourceIndex], sourceBuffer[2 + sourceIndex],
                sourceBuffer[1 + sourceIndex], sourceBuffer[0 + sourceIndex]);
              destinationBuffer[pixelIndex] = (byte) list.IndexOf(color);
            }
          }
        }

        Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, destinationSize);
        destination.UnlockBits(destinationData);
      }

      return destination;
    }

    #endregion
  }
}
