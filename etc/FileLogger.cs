using System;
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

        private string logFilePath = "";

        public void Init(string path)
        {
            lock (this)
            {
                this.logFilePath = path;
            }
        }

        public void Print(string message)
        {
            lock (this)
            {
                try
                {
                    if (logFilePath != "")
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(logFilePath, true /* APPEND */, Encoding.UTF8))
                        {
                            sw.WriteLine(message);
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugLogger.Instance.Print(e.ToString());
                }
            }
        }
    }
}
