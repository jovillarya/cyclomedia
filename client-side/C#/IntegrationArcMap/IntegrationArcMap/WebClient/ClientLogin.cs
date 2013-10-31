using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using Path = System.IO.Path;

namespace IntegrationArcMap.WebClient
{
  public class ClientLogin
  {
    private const string CheckWord = "1234567890!";
    private const int TestSize = 10;
    private const int HashSize = 32; // SHA256

    private const string WfsRequest =
      "<wfs:GetFeature service=\"WFS\" version=\"1.1.0\" resultType=\"results\" outputFormat=\"text/xml; subtype=gml/3.1.1\" xmlns:wfs=\"http://www.opengis.net/wfs\">" +
      "<wfs:Query typeName=\"atlas:Recording\" srsName=\"{0}\" xmlns:atlas=\"http://www.cyclomedia.com/atlas\"><ogc:Filter xmlns:ogc=\"http://www.opengis.net/ogc\">" +
      "<ogc:And><ogc:BBOX><gml:Envelope srsName=\"{0}\" xmlns:gml=\"http://www.opengis.net/gml\"><gml:lowerCorner>{1} {2}</gml:lowerCorner>" +
      "<gml:upperCorner>{3} {4}</gml:upperCorner></gml:Envelope></ogc:BBOX><ogc:PropertyIsNull><ogc:PropertyName>expiredAt</ogc:PropertyName></ogc:PropertyIsNull>" +
      "</ogc:And></ogc:Filter></wfs:Query></wfs:GetFeature>";

    private static readonly XmlSerializer XmlClientLogin;
    private static readonly byte[] Salt;

    private static ClientLogin _clientLogin;

    public ClientLogin()
    {
      Credentials = false;
    }

    static ClientLogin()
    {
      XmlClientLogin = new XmlSerializer(typeof(ClientLogin));
      Salt = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0};
    }

    public static string FileName
    {
      get { return Path.Combine(ArcUtils.FileDir, "GSClientLogin.xml"); }
    }

    public static ClientLogin Instance
    {
      get
      {
        if (_clientLogin == null)
        {
          Load();
        }

        return _clientLogin ?? (_clientLogin = new ClientLogin());
      }
    }

    public static ClientLogin Load()
    {
      if (File.Exists(FileName))
      {
        var streamFile = new FileStream(FileName, FileMode.OpenOrCreate);
        _clientLogin = (ClientLogin)XmlClientLogin.Deserialize(streamFile);
        streamFile.Close();
      }

      return _clientLogin;
    }

    public void SetLoginCredentials(string userName, string password)
    {
      Username = userName;
      Password = password;
      FileStream streamFile = File.Open(FileName, FileMode.Create);
      XmlClientLogin.Serialize(streamFile, this);
      streamFile.Close();
    }

    [XmlIgnore]
    public string Username { get; private set; }

    [XmlIgnore]
    public string Password { get; private set; }

    [XmlIgnore]
    public bool Credentials { get; private set; }

    public string Code
    {
      get
      {
        var ct1 = Encrypt(CheckWord, Salt, Encoding.UTF8.GetBytes(string.Format("{0};{1}", Username, Password)));
        return Convert.ToBase64String(ct1);
      }
      set
      {
        var ct1 = Convert.FromBase64String(value);
        var pt1 = Decrypt(CheckWord, Salt, ct1);
        string result = Encoding.UTF8.GetString(pt1);
        var values = result.Split(';');
        Username = (values.Length >= 1) ? values[0] : string.Empty;
        Password = (values.Length >= 2) ? values[1] : string.Empty;
      }
    }

    public bool Check()
    {
      Credentials = false;

      if ((!string.IsNullOrEmpty(Username)) && (!string.IsNullOrEmpty(Password)))
      {
        IActiveView activeView = ArcUtils.ActiveView;
        IEnvelope extent = activeView.Extent;

        IEnvelope testExtent = new EnvelopeClass
          {
            XMin = extent.XMin,
            YMin = extent.YMin,
            XMax = extent.XMin + TestSize,
            YMax = extent.YMin + TestSize
          };

        var webClient = ClientWeb.Instance;
        var recordings = webClient.GetByBbox(testExtent, WfsRequest);
        Credentials = (recordings.Count >= 0);
      }

      return Credentials;
    }

    /// <summary>Performs encryption with random IV (prepended to output), and includes hash of plaintext for verification.</summary>
    private static byte[] Encrypt(string password, byte[] passwordSalt, byte[] plainText)
    {
      // Construct message with hash
      var msg = new byte[HashSize + plainText.Length];
      var hash = ComputeHash(plainText, 0, plainText.Length);
      Buffer.BlockCopy(hash, 0, msg, 0, HashSize);
      Buffer.BlockCopy(plainText, 0, msg, HashSize, plainText.Length);

      // Encrypt
      using (var aes = CreateAes(password, passwordSalt))
      {
        aes.GenerateIV();

        using (var enc = aes.CreateEncryptor())
        {
          var encBytes = enc.TransformFinalBlock(msg, 0, msg.Length);

          // Prepend IV to result
          var res = new byte[aes.IV.Length + encBytes.Length];
          Buffer.BlockCopy(aes.IV, 0, res, 0, aes.IV.Length);
          Buffer.BlockCopy(encBytes, 0, res, aes.IV.Length, encBytes.Length);
          return res;
        }
      }
    }

    private static byte[] Decrypt(string password, byte[] passwordSalt, byte[] cipherText)
    {
      using (var aes = CreateAes(password, passwordSalt))
      {
        var iv = new byte[aes.IV.Length];
        Buffer.BlockCopy(cipherText, 0, iv, 0, iv.Length);
        aes.IV = iv; // Probably could copy right to the byte array, but that's not guaranteed

        using (var dec = aes.CreateDecryptor())
        {
          var decBytes = dec.TransformFinalBlock(cipherText, iv.Length, cipherText.Length - iv.Length);

          // Verify hash
          var hash = ComputeHash(decBytes, HashSize, decBytes.Length - HashSize);
          var existingHash = new byte[HashSize];
          Buffer.BlockCopy(decBytes, 0, existingHash, 0, HashSize);

          if (!CompareBytes(existingHash, hash))
          {
            throw new CryptographicException("Message hash incorrect.");
          }

          // Hash is valid, we're done
          var res = new byte[decBytes.Length - HashSize];
          Buffer.BlockCopy(decBytes, HashSize, res, 0, res.Length);
          return res;
        }
      }
    }

    private static bool CompareBytes(byte[] a1, byte[] a2)
    {
      bool result = a1.Length == a2.Length;

      if (result)
      {
        for (int i = 0; i < a1.Length; i++)
        {
          result = (a1[i] == a2[i]) && result;
        }
      }

      return result;
    }

    private static Aes CreateAes(string password, byte[] salt)
    {
      // Salt may not be needed if password is safe
      if (password.Length < 8)
      {
        throw new ArgumentException("Password must be at least 8 characters.", "password");
      }

      if (salt.Length < 8)
      {
        throw new ArgumentException("Salt must be at least 8 bytes.", "salt");
      }

      #pragma warning disable 612,618
      var pdb = new PasswordDeriveBytes(password, salt, "SHA512", 129);
      var aes = Aes.Create();
      aes.Mode = CipherMode.CBC;
      aes.Key = pdb.GetBytes(aes.KeySize / 8);
      return aes;
      #pragma warning restore 612,618
    }

    private static byte[] ComputeHash(byte[] data, int offset, int count)
    {
      byte[] result;

      using (var sha = SHA256.Create())
      {
        result = sha.ComputeHash(data, offset, count);
      }

      return result;
    }
  }
}
