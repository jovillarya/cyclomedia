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

using System.Globalization;
using System.Xml.Linq;
using IntegrationArcMap.Model.Shape;
using Point = IntegrationArcMap.Model.Shape.Point;

namespace IntegrationArcMap.Model.Atlas
{
  /// <summary>
  /// This class contains the image information
  /// </summary>
  public class Image
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private readonly CultureInfo _ci;

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public string ImageId { get; private set; }
    public IShape Shape { get; private set; }
    public Height Height { get; private set; }
    public double? LatitudeStDev { get; private set; }
    public double? LongitudeStDev { get; private set; }
    public double? HeightStDev { get; private set; }
    public double? Yaw { get; private set; }
    public double? YawStDev { get; private set; }
    public double? Pitch { get; private set; }
    public double? PitchStDev { get; private set; }
    public double? Roll { get; private set; }
    public double? RollStDev { get; private set; }
    public double? FocalLength { get; private set; }
    public double? PrincipalPointX { get; private set; }
    public double? PrincipalPointY { get; private set; }
    public string ImageType { get; private set; }
    public double? ImageHeight { get; private set; }
    public double? ImageWidth { get; private set; }
    public bool? IsAuthorized { get; private set; }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    /// <summary>
    /// Default empty constructor
    /// </summary>
    public Image()
    {
      _ci = CultureInfo.InvariantCulture;
    }

    /// <summary>
    /// Constructor with xml parsing
    /// </summary>
    /// <param name="mappedFeatureElement">xml</param>
    public Image(XElement mappedFeatureElement)
    {
      _ci = CultureInfo.InvariantCulture;
      Update(mappedFeatureElement);
    }

    #endregion

    #region functions

    // =========================================================================
    // Functions
    // =========================================================================
    /// <summary>
    /// xml parsing
    /// </summary>
    /// <param name="mappedFeatureElement">xml</param>
    public void Update(XElement mappedFeatureElement)
    {
      if (mappedFeatureElement != null)
      {
        XElement imageIdElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "imageId");
        XElement locationElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "location");
        XElement heightElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "height");
        XElement latStDevElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "latitudeStDev");
        XElement lonStDevElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "longitudeStDev");
        XElement heighStDevElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "heightStDev");
        XElement yawElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "yaw");
        XElement yawStDevElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "yawStDev");
        XElement pitchElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "pitch");
        XElement pitchStDevElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "pitchStDev");
        XElement rollElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "roll");
        XElement rollStDevElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "rollStDev");
        XElement focalLengthElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "focalLength");
        XElement priPointXElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "principalPointX");
        XElement priPointYElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "principalPointY");
        XElement imageTypeElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "imageType");
        XElement imageHeightElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "imageHeight");
        XElement imageWidthElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "imageWidth");
        XElement isAuthorizedElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "isAuthorized");

        ImageId = (imageIdElement == null) ? null : imageIdElement.Value.Trim();
        Height = (heightElement == null) ? null : new Height(heightElement);
        LatitudeStDev = (latStDevElement == null) ? (double?) null : double.Parse(latStDevElement.Value.Trim(), _ci);
        LongitudeStDev = (lonStDevElement == null) ? (double?) null : double.Parse(lonStDevElement.Value.Trim(), _ci);
        HeightStDev = (heighStDevElement == null) ? (double?) null : double.Parse(heighStDevElement.Value.Trim(), _ci);
        Yaw = (yawElement == null) ? (double?) null : double.Parse(yawElement.Value.Trim(), _ci);
        YawStDev = (yawStDevElement == null) ? (double?) null : double.Parse(yawStDevElement.Value.Trim(), _ci);
        Pitch = (pitchElement == null) ? (double?) null : double.Parse(pitchElement.Value.Trim(), _ci);
        PitchStDev = (pitchStDevElement == null) ? (double?) null : double.Parse(pitchStDevElement.Value.Trim(), _ci);
        Roll = (rollElement == null) ? (double?) null : double.Parse(rollElement.Value.Trim(), _ci);
        RollStDev = (rollStDevElement == null) ? (double?) null : double.Parse(rollStDevElement.Value.Trim(), _ci);
        FocalLength = (focalLengthElement == null) ? (double?) null : double.Parse(focalLengthElement.Value.Trim(), _ci);
        PrincipalPointX = (priPointXElement == null) ? (double?) null : double.Parse(priPointXElement.Value.Trim(), _ci);
        PrincipalPointY = (priPointYElement == null) ? (double?)null : double.Parse(priPointYElement.Value.Trim(), _ci);
        ImageType = (imageTypeElement == null) ? null : imageTypeElement.Value.Trim();
        ImageHeight = (imageHeightElement == null) ? (double?) null : double.Parse(imageHeightElement.Value.Trim(), _ci);
        ImageWidth = (imageWidthElement == null) ? (double?) null : double.Parse(imageWidthElement.Value.Trim(), _ci);
        IsAuthorized = (isAuthorizedElement == null) ? (bool?) null : bool.Parse(isAuthorizedElement.Value.Trim());

        if (locationElement != null)
        {
          if (locationElement.Element(Namespaces.GmlNs + "Point") != null)
          {
            Shape = new Point(locationElement);
          }
        }
      }
    }

    #endregion
  }
}
