using System;

namespace OneNoteDuplicatesRemover.etc
{
    public sealed class DebugLogger
    {
        private static volatile DebugLogger instance;
        private static object syncRoot = new Object();

        private DebugLogger() { }

        public static DebugLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new DebugLogger();
                        }
                    }
                }

                return instance;
            }
        }

        public void Print(string message)
        {
            lock (this)
            {
                System.Diagnostics.Debug.WriteLine(message);
            }
        }
    }
}
