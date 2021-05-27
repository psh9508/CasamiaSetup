using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Logger
    {
        private static readonly object _syncObject = new object();
        private static readonly string _logFileFullPath = string.Empty;

        static Logger()
        {
            _logFileFullPath = Path.Combine(Constants.LOG_PATH, $@"{DateTime.Today.ToString("yyyy-MM-dd")}.txt");
            CreateLogFolder();

            Write("-------------------------");
            Write("---- 프로그램 시작 ----");
            Write("-------------------------");
        }

        public static void WriteError(string message)
        {
            WriteBody($@"| Error | {message}");
        }

        public static void Write(string message)
        {
            WriteBody(($@"| Info | {message}"));
        }

        private static void WriteBody(string message)
        {
            CreateLogFolder();

            lock (_syncObject)
            {
                using (var logFile = new StreamWriter(_logFileFullPath, true))
                {
                    logFile.WriteLine($@"[{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffff")}] {message}");
                    logFile.Close();
                }
            }
        }

        private static void CreateLogFolder()
        {
            if (Directory.Exists(Constants.LOG_PATH) == false)
                Directory.CreateDirectory(Constants.LOG_PATH);

            if (File.Exists(_logFileFullPath) == false)
            {
                var file = File.Create(_logFileFullPath);
                file.Close();
            }
        }

    }
}
