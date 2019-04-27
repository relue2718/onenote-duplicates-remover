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

        private string path = "";

        public void Init(string path)
        {
            lock (this)
            {
                this.path = path;
            }
        }

        public void Print(string message)
        {
            lock (this)
            {
                try
                {
                    if (path != "")
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, Encoding.UTF8))
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
