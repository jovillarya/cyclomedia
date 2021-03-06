﻿/*
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
using System.Net;
using System.Threading;

namespace IntegrationArcMap.Client
{
  class State
  {
    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public ManualResetEvent OperationComplete { get; private set; }
    public WebRequest Request { get; set; }
    public object Result { get; set; }
    public Exception OperationException { get; set; }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    public State()
    {
      OperationComplete = new ManualResetEvent(false);
      OperationException = null;
    }

    #endregion
  }
}
