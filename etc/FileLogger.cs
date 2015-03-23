using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover.etc
{
  public sealed class FileLogger
  {
    private static volatile FileLogger instance;
    private static object syncRoot = new Object();

    private FileLogger() { }

    public static FileLogger Instance
    {
      get
      {
        if (instance == null)
        {
          lock (syncRoot)
          {
            if (instance == null)
            {
              instance = new FileLogger();
            }
          }
        }

        return instance;
      }
    }

    private string path = "";

    public void Init(string path)
    {
      this.path = path;
    }

    public void Print(string category, string format, params object[] args)
    {
      try
      {
        if (path != "")
        {
          using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, Encoding.UTF8))
          {
            sw.WriteLine(category + " * " + DateTime.Now.ToUniversalTime().ToString() + " * " + format, args);
          }
        }
      }
      catch (Exception e)
      {
        System.Diagnostics.Debug.Print(e.ToString());
      }
    }
  }
}
