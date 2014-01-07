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

namespace RangeControl
{
  /// <RangeSelectorControl_Resize>
  /// The below is the small Notification class that can be used by the client
  /// </RangeSelectorControl_Resize>
  /// 
  #region Notification class for client to register with the control for changes

  public class NotifyClient
  {
    public string Range1 { get; set; }

    public string Range2 { get; set; }
  }

  #endregion
}
