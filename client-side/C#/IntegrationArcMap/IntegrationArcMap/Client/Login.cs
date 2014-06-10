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

using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Security.Cryptography;
using IntegrationArcMap.Properties;
using IntegrationArcMap.Utilities;

namespace IntegrationArcMap.Client
{
  [XmlRoot("ClientLogin")]
  public class Login
  {
    #region constants

    // =========================================================================
    // Constants
    // =========================================================================
    private const string CheckWord = "1234567890!";
    private const int HashSize = 32; // SHA256

    #endregion

    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private static readonly XmlSerializer XmlLogin;
    private static readonly byte[] Salt;

    private static Login _login;

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    static Login()
    {
      XmlLogin = new XmlSerializer(typeof(Login));
      Salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
    }

    public Login()
    {
      Credentials = false;
    }

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
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

    public static string FileName
    {
      get { return Path.Combine(ArcUtils.FileDir, "GSClientLogin.xml"); }
    }

    public static Login Instance
    {
      get
      {
        if (_login == null)
        {
          Load();
        }

        return _login ?? (_login = new Login());
      }
    }

    #endregion

    #region functions (public)

    // =========================================================================
    // Functions (Public)
    // =========================================================================
    public void SetLoginCredentials(string userName, string password)
    {
      Username = userName;
      Password = password;
      FileStream streamFile = File.Open(FileName, FileMode.Create);
      XmlLogin.Serialize(streamFile, this);
      streamFile.Close();
    }

    public bool Check()
    {
      return
        (Credentials =
          (((!string.IsNullOrEmpty(Username)) && (!string.IsNullOrEmpty(Password))) &&
           GlobeSpotterConfiguration.CheckCredentials()));
    }

    #endregion

    #region functions (static)

    // =========================================================================
    // Functions (Static)
    // =========================================================================
    public static Login Load()
    {
      if (File.Exists(FileName))
      {
        var streamFile = new FileStream(FileName, FileMode.OpenOrCreate);
        _login = (Login) XmlLogin.Deserialize(streamFile);
        streamFile.Close();
      }

      return _login;
    }

    #endregion

    #region functions (Private static)

    // =========================================================================
    // Functions (Private static)
    // =========================================================================
    /// <summary>
    /// Performs encryption with random IV (prepended to output), and includes hash of plaintext for verification.
    /// </summary>
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
        throw new ArgumentException(Resources.Login_CreateAes_Password_must_be_at_least_8_characters_, "password");
      }

      if (salt.Length < 8)
      {
        throw new ArgumentException(Resources.Login_CreateAes_Salt_must_be_at_least_8_bytes_, "salt");
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

    #endregion
  }
}
