using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace IntegrationArcMap.Client
{
  public class LogClient
  {
    private const int LogTypeDebug = 1;
    private const int LogTypeInfo = 2;
    private const int LogTypeWarning = 3;
    private const int LogTypeError = 4;
    private const int LogTypeFatal = 5;

    private const string ClassName = "Logger";
    private const string StackTraceConst = "StackTrace";
    private const string SeperationCharacters = ": ";

    internal readonly ILog Log;

    private readonly string _newLine = Environment.NewLine;

    private const string LogPattern = "%utcdate [%thread] %-5level %logger [%ndc] \t%property{auth}\t - %message%newline";
    private const string LogHeader = "[Start of log]";
    private const string LogFooter = "[End of log]";
    private const string DatePattern = "\"GlobeSpotter for ArcGIS Desktop\".yyyy.MM.dd.\"log\"";

    static LogClient()
    {
      Config config = Config.Instance;

      if (config.UseLogging && (!string.IsNullOrEmpty(config.LoggingLocation)) && Directory.Exists(config.LoggingLocation))
      {
        var hierarchy = (Hierarchy) LogManager.GetRepository();

        var patternLayout = new PatternLayout
        {
          ConversionPattern = LogPattern,
          Header = LogHeader + Environment.NewLine,
          Footer = LogFooter + Environment.NewLine
        };

        patternLayout.ActivateOptions();

        var fileAppender = new RollingFileAppender
        {
          Layout = patternLayout,
          DatePattern = DatePattern,
          AppendToFile = true,
          StaticLogFileName = false,
          RollingStyle = RollingFileAppender.RollingMode.Date,
          File = config.LoggingLocation
        };

        fileAppender.ActivateOptions();
        hierarchy.Root.AddAppender(fileAppender);
        hierarchy.Root.Level = Level.All;
        hierarchy.Configured = true;
      }
    }

    public LogClient(Type type)
    {
      Log = LogManager.GetLogger(type.Name);
    }

    public bool IsDebugEnabled
    {
      get { return Log.IsDebugEnabled; }
    }

    public bool IsInfoEnabled
    {
      get { return Log.IsInfoEnabled; }
    }

    public bool IsWarnEnabled
    {
      get { return Log.IsWarnEnabled; }
    }

    public bool IsErrorEnabled
    {
      get { return Log.IsErrorEnabled; }
    }

    public bool IsFatalEnabled
    {
      get { return Log.IsFatalEnabled; }
    }

    public void Debug(string message)
    {
      if (IsDebugEnabled)
      {
        string trace = Environment.StackTrace;
        Log.Debug(string.Concat(ExtractMethodName(ClassName, trace), SeperationCharacters, message));
      }
    }

    public void Debug(string message, Exception e)
    {
      if (IsDebugEnabled)
      {
        string trace = Environment.StackTrace;
        Log.Debug(string.Concat(ExtractMethodName(ClassName, trace), SeperationCharacters, message, GetExceptionInfo(e, LogTypeDebug)));
      }
    }

    public void Debug(string methodName, string message)
    {
      if (IsDebugEnabled)
      {
        Log.Debug(string.Concat(methodName, SeperationCharacters, message));
      }
    }

    public void Debug(string methodName, string message, Exception e)
    {
      if (IsDebugEnabled)
      {
        Log.Debug(string.Concat(methodName, SeperationCharacters, message, GetExceptionInfo(e, LogTypeDebug)));
      }
    }

    public void Info(string message)
    {
      if (IsInfoEnabled)
      {
        string trace = Environment.StackTrace;
        Log.Info(string.Concat(ExtractMethodName(ClassName, trace), SeperationCharacters, message));
      }
    }

    public void Info(string message, Exception e)
    {
      if (IsInfoEnabled)
      {
        string trace = Environment.StackTrace;
        Log.Info(string.Concat(ExtractMethodName(ClassName, trace), SeperationCharacters, message, GetExceptionInfo(e, LogTypeInfo)));
      }
    }

    public void Info(string methodName, string message)
    {
      if (IsInfoEnabled)
      {
        Log.Info(string.Concat(methodName, SeperationCharacters, message));
      }
    }

    public void Info(string methodName, string message, Exception e)
    {
      if (IsInfoEnabled)
      {
        Log.Info(string.Concat(methodName, SeperationCharacters, message, GetExceptionInfo(e, LogTypeInfo)));
      }
    }

    public void Warn(string message)
    {
      if (IsWarnEnabled)
      {
        string trace = Environment.StackTrace;
        Log.Warn(string.Concat(ExtractMethodName(ClassName, trace), SeperationCharacters, message));
      }
    }

    public void Warn(string message, Exception e)
    {
      if (IsWarnEnabled)
      {
        string trace = Environment.StackTrace;
        Log.Warn(string.Concat(ExtractMethodName(ClassName, trace), SeperationCharacters, message, GetExceptionInfo(e, LogTypeWarning)));
      }
    }

    public void Warn(string methodName, string message)
    {
      if (IsWarnEnabled)
      {
        Log.Warn(string.Concat(methodName, SeperationCharacters, message));
      }
    }

    public void Warn(string methodName, string message, Exception e)
    {
      if (IsWarnEnabled)
      {
        Log.Warn(string.Concat(methodName, SeperationCharacters, message, GetExceptionInfo(e, LogTypeWarning)));
      }
    }

    public void Error(string message)
    {
      if (IsErrorEnabled)
      {
        string trace = Environment.StackTrace;
        Log.Error(string.Concat(ExtractMethodName(ClassName, trace), SeperationCharacters, message));
      }
    }

    public void Error(string message, Exception e)
    {
      if (IsErrorEnabled)
      {
        string trace = Environment.StackTrace;
        Log.Error(string.Concat(ExtractMethodName(ClassName, trace), SeperationCharacters, message, GetExceptionInfo(e, LogTypeError)));
      }
    }

    public void Error(string methodName, string message)
    {
      if (IsErrorEnabled)
      {
        Log.Error(string.Concat(methodName, SeperationCharacters, message));
      }
    }

    public void Error(string methodName, string message, Exception e)
    {
      if (IsErrorEnabled)
      {
        Log.Error(string.Concat(methodName, SeperationCharacters, message, GetExceptionInfo(e, LogTypeError)));
      }
    }

    public void Fatal(string message)
    {
      if (IsFatalEnabled)
      {
        string trace = Environment.StackTrace;
        Log.Fatal(string.Concat(ExtractMethodName(ClassName, trace), SeperationCharacters, message));
      }
    }

    public void Fatal(string message, Exception e)
    {
      if (IsFatalEnabled)
      {
        string trace = Environment.StackTrace;
        Log.Fatal(string.Concat(ExtractMethodName(ClassName, trace), SeperationCharacters, message, GetExceptionInfo(e, LogTypeFatal)));
      }
    }

    public void Fatal(string methodName, string message)
    {
      if (IsFatalEnabled)
      {
        Log.Fatal(string.Concat(methodName, SeperationCharacters, message));
      }
    }

    public void Fatal(string methodName, string message, Exception e)
    {
      if (IsFatalEnabled)
      {
        Log.Fatal(string.Concat(methodName, SeperationCharacters, message, GetExceptionInfo(e, LogTypeFatal)));
      }
    }

    private string ExtractMethodName(string className, string trace)
    {
      string result = string.Empty;

      if (!string.IsNullOrEmpty(trace))
      {
        if (trace.Contains(className))
        {
          string[] traceLines = trace.Split('\n');

          for (int i = 0; i < traceLines.Length; i++)
          {
            if (traceLines[i].Contains(className))
            {
              int posDot = traceLines[i + 1].IndexOf('.');
              int posBrace = traceLines[i + 1].IndexOf('(');
              int posNextDot = traceLines[i + 1].IndexOf('.', posDot + 1);

              while (posNextDot > 0 && posNextDot < posBrace)
              {
                posDot = posNextDot;
                posNextDot = traceLines[i + 1].IndexOf('.', posDot + 1);
              }

              result = traceLines[i + 1].Substring(posDot + 1, posBrace - posDot - 1);
              break;
            }
          }
        }
      }

      return result;
    }

    private string GetExceptionInfo(Exception exception, int logType)
    {
      StringBuilder sb = null;

      while (exception != null)
      {
        if (sb == null)
        {
          sb = new StringBuilder(256);
          sb.Append(SeperationCharacters);
          sb.Append(exception.Message);
        }

        AddExceptionInfo(exception, sb, logType);
        exception = exception.InnerException;
      }

      return sb == null ? string.Empty : sb.ToString();
    }

    private void AddExceptionInfo(Exception exception, StringBuilder sb, int logType)
    {
      if (exception != null)
      {
        sb.Append(_newLine);
        sb.Append("Exception: [");
        sb.Append(exception.GetType().Name);
        sb.Append("] :");
        sb.Append(exception.Message);
        bool logStackTraceForErrors = ((logType == LogTypeFatal) || (logType == LogTypeError));
        bool forceLogForExceptions = false;

        if (!logStackTraceForErrors)
        {
          forceLogForExceptions =
            (exception is NullReferenceException) ||
            (exception is IndexOutOfRangeException) ||
            (exception is ArgumentOutOfRangeException) ||
            (exception is ArgumentNullException) ||
            (exception is OutOfMemoryException) ||
            (exception.GetType().Name == "InvalidCallException");
        }

        if (logStackTraceForErrors || forceLogForExceptions)
        {
          var st = new StackTrace(exception);

          if (st.FrameCount > 0)
          {
            sb.Append(_newLine);
            sb.Append(StackTraceConst);
            sb.Append(st);
          }
        }
      }
    }
  }
}
