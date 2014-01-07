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

using System.Drawing;
using System.Drawing.Imaging;

namespace IntegrationArcMap.Utilities
{
  public class MakeTransparant
  {
    #region functions (static)

    // =========================================================================
    // Functions (Static)
    // =========================================================================
    public static Bitmap ApplySrc(Bitmap input)
    {
      var newBitmap = (Bitmap) input.Clone();
      BitmapData newData = LockImage(newBitmap);
      BitmapData oldData = LockImage(input);
      int newPixelSize = GetPixelSize(newData);
      int oldPixelSize = GetPixelSize(oldData);

      for (int x = 0; x < input.Width; ++x)
      {
        for (int y = 0; y < input.Height; ++y)
        {
          Color pixel = GetPixel(oldData, x, y, oldPixelSize);
          Color newPixel = IsTransparant(pixel) ? Color.White : ToPixelSrc(pixel);
          SetPixel(newData, x, y, newPixel, newPixelSize);
        }
      }

      UnlockImage(newBitmap, newData);
      UnlockImage(input, oldData);
      return newBitmap;
    }

    public static Bitmap ApplyDst(Bitmap input)
    {
      var newBitmap = (Bitmap)input.Clone();
      BitmapData newData = LockImage(newBitmap);
      BitmapData oldData = LockImage(input);
      int newPixelSize = GetPixelSize(newData);
      int oldPixelSize = GetPixelSize(oldData);

      for (int x = 0; x < input.Width; ++x)
      {
        for (int y = 0; y < input.Height; ++y)
        {
          Color pixel = GetPixel(oldData, x, y, oldPixelSize);
          Color newPixel = ((x <= 1) || (x >= (input.Width - 2)) || (y <= 1) || (y >= (input.Height - 2)))
                             ? ToPixelDst(pixel)
                             : pixel;
          SetPixel(newData, x, y, newPixel, newPixelSize);
        }
      }

      UnlockImage(newBitmap, newData);
      UnlockImage(input, oldData);
      return newBitmap;
    }

    #endregion

    #region functions (private static)

    // =========================================================================
    // Functions (Private Static)
    // =========================================================================
    private static bool IsTransparant(Color pixel)
    {
      return ((pixel.A <= 35) && (pixel.R >= 93) && (pixel.R <= 163) && (pixel.B >= 93) && (pixel.B <= 163) &&
              (pixel.G >= 93) && (pixel.G <= 168));
    }

    private static Color ToPixelSrc(Color pixel)
    {
      return ((pixel.A > 240) && (pixel.R > 240) && (pixel.B > 240) && (pixel.G > 240))
               ? Color.FromArgb(192, pixel.R, pixel.G, pixel.B)
               : pixel;
    }

    private static Color ToPixelDst(Color pixel)
    {
      return ((pixel.R > 250) && (pixel.B > 250) && (pixel.G > 250)) ? Color.White : pixel;
    }

    internal static BitmapData LockImage(Bitmap image)
    {
      return (image == null) ? null : image.LockBits
        (new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
    }

    internal static void UnlockImage(Bitmap image, BitmapData imageData)
    {
      if ((image != null) && (imageData != null))
      {
        image.UnlockBits(imageData);
      }
    }

    internal static int GetPixelSize(BitmapData data)
    {
      int result = 0;

      if (data != null)
      {
        if (data.PixelFormat == PixelFormat.Format24bppRgb)
        {
          result = 3;
        }
        else
        {
          if (data.PixelFormat == PixelFormat.Format32bppArgb
              || data.PixelFormat == PixelFormat.Format32bppPArgb
              || data.PixelFormat == PixelFormat.Format32bppRgb)
          {
            result = 4;
          }
        }
      }

      return result;
    }

    internal static unsafe Color GetPixel(BitmapData data, int x, int y, int pixelSizeInBytes)
    {
      Color result = Color.Black;

      if (data != null)
      {
        var dataPointer = (byte*)data.Scan0;
        dataPointer = dataPointer + (y * data.Stride) + (x * pixelSizeInBytes);
        result = (pixelSizeInBytes == 3) ? Color.FromArgb(dataPointer[2], dataPointer[1], dataPointer[0])
                   : Color.FromArgb(dataPointer[3], dataPointer[2], dataPointer[1], dataPointer[0]);
      }

      return result;
    }

    internal static unsafe void SetPixel(BitmapData data, int x, int y, Color pixelColor, int pixelSizeInBytes)
    {
      if (data != null)
      {
        var dataPointer = (byte*)data.Scan0;
        dataPointer = dataPointer + (y * data.Stride) + (x * pixelSizeInBytes);

        if (pixelSizeInBytes == 3)
        {
          dataPointer[2] = pixelColor.R;
          dataPointer[1] = pixelColor.G;
          dataPointer[0] = pixelColor.B;
        }
        else
        {
          dataPointer[3] = pixelColor.A;
          dataPointer[2] = pixelColor.R;
          dataPointer[1] = pixelColor.G;
          dataPointer[0] = pixelColor.B;
        }
      }
    }

    #endregion
  }
}
