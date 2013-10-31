using System;
using System.Net;
using System.Threading;

namespace IntegrationArcMap.WebClient
{
  class ClientState
  {
    #region Properties

    public ManualResetEvent OperationComplete { get; private set; }
    public WebRequest Request { get; set; }
    public object Result { get; set; }
    public Exception OperationException { get; set; }

    #endregion

    #region Constructor

    public ClientState()
    {
      OperationComplete = new ManualResetEvent(false);
      OperationException = null;
    }

    #endregion
  }
}
