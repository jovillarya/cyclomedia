using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace IntegrationArcMap.Utilities
{
  internal class CurrentCult
  {
    private static readonly IDictionary<int, string[]> ConversionDatePattern
      = new Dictionary<int, string[]>
        {
          {1, new[] {"yyyyy", "YYYYY"}},
          {2, new[] {"yyyy", "YYYY"}},
          {3, new[] {"yyy", "YYY"}},
          {4, new[] {"yy", "YY"}},
          {5, new[] {"y", "Y"}},
          {6, new[] {"MMMM", "MMMM"}},
          {7, new[] {"MMM", "MMM"}},
          {8, new[] {"MM", "MM"}},
          {9, new[] {"M", "M"}},
          {10, new[] {"dddd", "EEEE"}},
          {11, new[] {"ddd", "EEE"}},
          {12, new[] {"dd", "DD"}},
          {13, new[] {"d", "D"}}
        };

    private static readonly IDictionary<int, string[]> ConversionTimePattern
      = new Dictionary<int, string[]>
        {
          {1, new[] {"tt", "AA"}},
          {2, new[] {"t", "A"}},
          {3, new[] {"HH", "JJ"}},
          {4, new[] {"H", "J"}},
          {5, new[] {"hh", "KK"}},
          {6, new[] {"h", "K"}},
          {7, new[] {"mm", "NN"}},
          {8, new[] {"m", "N"}},
          {9, new[] {"ss", "SS"}},
          {10, new[] {"s", "S"}},
          {11, new[] {"fff", "QQQ"}}
        };

    private CultureInfo _result;

    public string DateFormat
    {
      get
      {
        DateTimeFormatInfo datetimeFormat = _result.DateTimeFormat;
        string pattern = datetimeFormat.ShortDatePattern;
        return Convert(pattern, ConversionDatePattern);
      }
    }

    public string TimeFormat
    {
      get
      {
        DateTimeFormatInfo datetimeFormat = _result.DateTimeFormat;
        string pattern = datetimeFormat.LongTimePattern;
        return Convert(pattern, ConversionTimePattern);
      }
    }

    private string Convert(string pattern, IDictionary<int, string[]> conversionPattern)
    {
      int elements = conversionPattern.Count;

      for (int i = 1; i <= elements; i++)
      {
        if (conversionPattern.ContainsKey(i))
        {
          string[] conf = conversionPattern[i];

          if (conf.Length == 2)
          {
            pattern = pattern.Replace(conf[0], conf[1]);
          }
        }
      }

      return pattern;
    }

    public static CurrentCult Get()
    {
      Thread.CurrentThread.CurrentCulture.ClearCachedData();
      var thread = new Thread(s => ((CurrentCult) s)._result = Thread.CurrentThread.CurrentCulture);
      var state = new CurrentCult();
      thread.Start(state);
      thread.Join();
      return state;
    }
  }
}
