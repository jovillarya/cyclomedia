using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using IntegrationArcMap.Layers;
using IntegrationArcMap.Model;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.Geometry;

namespace IntegrationArcMap.WebClient
{
  public class ClientWeb
  {
    #region Requests

    private const string RecordingRequest =
      "{0}?service=WFS&version=1.1.0&request=GetFeature&srsname=EPSG:4258&featureid={1}&TYPENAME=atlas:Recording";

    private const string WfsBboxRequest =
      "{0}?SERVICE=WFS&VERSION={1}&REQUEST=GetFeature&SRSNAME={2}&BBOX={3},{4},{5},{6},{7}&TYPENAME={8}";

    private const string CapabilityString = "{0}?REQUEST=GetCapabilities&VERSION={1}&SERVICE=WFS";

    private const string AuthorizationService = "https://atlas.cyclomedia.com/Authorization/GlobeSpotter";

    private const int BufferImageLengthService = 2048;
    private const int XmlConfig = 0;
    private const int DownloadImageConfig = 1;

    private readonly int[] _waitTimeInternalServerError = {5000, 0};
    private readonly int[] _timeOutService = {3000, 1000};
    private readonly int[] _retryTimeService = {3, 1};

    #endregion

    #region Members

    private static ClientWeb _clientWfs;
    private readonly CultureInfo _ci;
    private readonly ClientLogin _clientLogin;
    private readonly ClientConfig _clientConfig;
    private readonly ClientAPIKey _clientApiKey;

    #endregion

    private string RecordingService
    {
      get { return string.Format("{0}/recordings/wfs", _clientConfig.BaseUrl); }
    }

    #region Constructor

    private ClientWeb()
    {
      _ci = CultureInfo.InvariantCulture;
      _clientLogin = ClientLogin.Instance;
      _clientConfig = ClientConfig.Instance;
      _clientApiKey = ClientAPIKey.Instance;
    }

    #endregion

    #region Instance

    public static ClientWeb Instance
    {
      get { return _clientWfs ?? (_clientWfs = new ClientWeb()); }
    }

    #endregion

    #region interface functions

    public List<XElement> GetByImageId(string imageId)
    {
      string remoteLocation = string.Format(RecordingRequest, RecordingService, imageId);
      var xml = (string) GetRequest(remoteLocation, GetXmlCallback, XmlConfig);
      return ParseXml(xml, (Namespaces.GmlNs + "featureMembers"));
    }

    public List<XElement> GetByBbox(IEnvelope envelope, CycloMediaLayer cycloMediaLayer)
    {
      string epsgCode = ArcUtils.EpsgCode;
      List<XElement> result;

      if (cycloMediaLayer is WfsLayer)
      {
        var wfsLayer = cycloMediaLayer as WfsLayer;
        string remoteLocation = string.Format(_ci, WfsBboxRequest, wfsLayer.Url, wfsLayer.Version, epsgCode,
                                              envelope.XMin, envelope.YMin, envelope.XMax, envelope.YMax,
                                              epsgCode, wfsLayer.TypeName);
        var xml = (string) GetRequest(remoteLocation, GetXmlCallback, XmlConfig);
        result = ParseXml(xml, (Namespaces.GmlNs + "featureMember"));
      }
      else
      {
        string postItem = string.Format(_ci, cycloMediaLayer.WfsRequest, epsgCode, envelope.XMin, envelope.YMin,
                                        envelope.XMax,
                                        envelope.YMax, DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:00-00:00"));
        var xml = (string)PostRequest(RecordingService, GetXmlCallback, postItem, XmlConfig);
        result = ParseXml(xml, (Namespaces.GmlNs + "featureMembers"));
      }

      return result;
    }

    public List<XElement> CheckAuthorization()
    {
      const string postItem = @"<Authorization />";
      var xml = (string)PostRequest(AuthorizationService, GetXmlCallback, postItem, XmlConfig);
      return ParseXml(xml, (Namespaces.CycloMediaNs + "Permission"));
    }

    public List<XElement> GetByBbox(IEnvelope envelope, string wfsRequest)
    {
      string epsgCode = ArcUtils.EpsgCode;
      string postItem = string.Format(_ci, wfsRequest, epsgCode, envelope.XMin, envelope.YMin, envelope.XMax,
                                      envelope.YMax, DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:00-00:00"));
      var xml = (string)PostRequest(RecordingService, GetXmlCallback, postItem, XmlConfig);
      return ParseXml(xml, (Namespaces.GmlNs + "featureMembers"));
    }

    public List<XElement> GetCapabilities(string wfsService, string version)
    {
      string remotelocation = string.Format(CapabilityString, wfsService, version);
      var xml = (string) GetRequest(remotelocation, GetXmlCallback, XmlConfig);
      return ParseXml(xml, (Namespaces.WfsNs + "FeatureTypeList"));
    }

    public Image DownloadImage(string url)
    {
      var imageStream = GetRequest(url, GetImageCallback, DownloadImageConfig) as Stream;
      return (imageStream == null) ? null : Image.FromStream(imageStream);
    }

    #endregion

    #region parseXML

    private static List<XElement> ParseXml(string xml, XName xName)
    {
      var stringReader = new StringReader(xml);
      var xmlTextReader = new XmlTextReader(stringReader);
      XDocument xmlDoc = XDocument.Load(xmlTextReader);
      IEnumerable<XElement> elements = xmlDoc.Descendants(xName);
      return elements.ToList();
    }

    #endregion

    #region wfs request functions

    private object GetRequest(string remoteLocation, AsyncCallback asyncCallback, int configId)
    {
      object result = null;
      bool download = false;
      int retry = 0;
      WebRequest request = OpenWebRequest(remoteLocation, WebRequestMethods.Http.Get, 0);
      var state = new ClientState {Request = request};

      while ((download == false) && (retry < _retryTimeService[configId]))
      {
        try
        {
          lock (this)
          {
            ManualResetEvent waitObject = state.OperationComplete;
            request.BeginGetResponse(asyncCallback, state);

            if (!waitObject.WaitOne(_timeOutService[configId]))
            {
              throw new Exception("Time out download item");
            }

            if (state.OperationException != null)
            {
              throw state.OperationException;
            }

            result = state.Result;
            download = true;
          }
        }
        catch (WebException ex)
        {
          retry++;
          var responce = ex.Response as HttpWebResponse;

          if (responce != null)
          {
            if ((responce.StatusCode == HttpStatusCode.InternalServerError) && (retry < _retryTimeService[configId]))
            {
              Thread.Sleep(_waitTimeInternalServerError[configId]);
            }
          }

          if (retry == _retryTimeService[configId])
          {
            throw;
          }
        }
        catch (Exception)
        {
          retry++;

          if (retry == _retryTimeService[configId])
          {
            throw;
          }
        }
      }

      return result;
    }

    private object PostRequest(string remoteLocation, AsyncCallback asyncCallback, string postItem, int configId)
    {
      object result = null;
      bool download = false;
      int retry = 0;
      var bytes = (new UTF8Encoding()).GetBytes(postItem);
      WebRequest request = OpenWebRequest(remoteLocation, WebRequestMethods.Http.Post, bytes.Length);
      var state = new ClientState {Request = request};
      var reqstream = request.GetRequestStream();
      reqstream.Write(bytes, 0, bytes.Length);
      reqstream.Close();

      while ((download == false) && (retry < _retryTimeService[configId]))
      {
        try
        {
          lock (this)
          {
            ManualResetEvent waitObject = state.OperationComplete;
            request.BeginGetResponse(asyncCallback, state);

            if (!waitObject.WaitOne(_timeOutService[configId]))
            {
              throw new Exception("Time out download item.");
            }

            if (state.OperationException != null)
            {
              throw state.OperationException;
            }

            result = state.Result;
            download = true;
          }
        }
        catch (WebException ex)
        {
          retry++;
          var responce = ex.Response as HttpWebResponse;

          if (responce != null)
          {
            if ((responce.StatusCode == HttpStatusCode.InternalServerError) && (retry < _retryTimeService[configId]))
            {
              Thread.Sleep(_waitTimeInternalServerError[configId]);
            }
          }

          if (retry == _retryTimeService[configId])
          {
            throw;
          }
        }
        catch (Exception)
        {
          retry++;

          if (retry == _retryTimeService[configId])
          {
            throw;
          }
        }
      }

      return result;
    }

    private WebRequest OpenWebRequest(string remoteLocation, string webRequest, int length)
    {
      var request = (HttpWebRequest) WebRequest.Create(remoteLocation);
      request.Credentials = new NetworkCredential(_clientLogin.Username, _clientLogin.Password);
      request.Method = webRequest;
      request.ContentLength = length;
      request.KeepAlive = true;
      request.Pipelined = true;
      request.PreAuthenticate = true;
      request.ContentType = "text/xml";
      request.Headers.Add("ApiKey", _clientApiKey.APIKey);
      return request;
    }

    private static void GetXmlCallback(IAsyncResult ar)
    {
      var state = (ClientState) ar.AsyncState;

      try
      {
        var response = state.Request.EndGetResponse(ar);
        Stream responseStream = response.GetResponseStream();

        if (responseStream != null)
        {
          var reader = new StreamReader(responseStream);
          state.Result = reader.ReadToEnd();
        }

        response.Close();
        state.OperationComplete.Set();
      }
      catch (Exception e)
      {
        state.OperationException = e;
        state.OperationComplete.Set();
      }
    }

    private void GetImageCallback(IAsyncResult ar)
    {
      var state = (ClientState) ar.AsyncState;

      try
      {
        var response = state.Request.EndGetResponse(ar);
        Stream responseStream = response.GetResponseStream();

        if (responseStream != null)
        {
          var readFile = new BinaryReader(responseStream);
          state.Result = new MemoryStream();
          var writeFile = new BinaryWriter((Stream) state.Result);
          var buffer = new byte[BufferImageLengthService];
          int readBytes;

          do
          {
            readBytes = readFile.Read(buffer, 0, BufferImageLengthService);
            writeFile.Write(buffer, 0, readBytes);
          } while (readBytes != 0);

          writeFile.Flush();
        }

        response.Close();
        state.OperationComplete.Set();
      }
      catch (Exception e)
      {
        state.OperationException = e;
        state.OperationComplete.Set();
      }
    }

    #endregion
  }
}
