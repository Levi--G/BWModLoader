using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ModLoader
{
    public class ModLogger
    {
        string prefix;
        string file;

        /// <summary>
        /// Log history
        /// </summary>
        public List<string> Logs { get; } = new List<string>();

        public ModLogger(string prefix, string file = null)
        {
            this.prefix = prefix;
            this.file = file;
            if (!string.IsNullOrEmpty(file) && !File.Exists(file))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                File.WriteAllText(file, "");
            }
        }

        public void ClearLog()
        {
            Logs.Clear();
            if (!string.IsNullOrEmpty(file) && File.Exists(file))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Log to debugger and output.txt
        /// </summary>
        public void Log(string output)
        {
            Console.WriteLine(prefix + output);
            Logs.Add(prefix + output);
            if (!string.IsNullOrEmpty(file))
            {
                File.AppendAllText(file, prefix + output + Environment.NewLine);
            }
        }

        public void DebugLog(string output)
        {
#if DEBUG
            Log(output);
#endif // DEBUG
        }
    }
}
