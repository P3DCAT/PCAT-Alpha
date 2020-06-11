using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PCATApp {
    public static class Program {
        private static string[] PythonExecutables = { "python3.exe", "python.exe" };

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        /*
         *Todo:
         * Redirect output to a log file, start of file should be like
         * PCAT Version xxx (date of launch)
         */
        static void Main() {
            Console.WriteLine("PCAT Version " + getPCATVersion());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PCAT());
        }

        public static string getPCATVersion() {
            return (string)Properties.Settings.Default["ProgramVersion"];
        }

        public static bool SetPandaPath(string path) {
            if (!IsPandaPathValid(path)) {
                Properties.Settings.Default["pandaPath"] = "";
                Properties.Settings.Default.Save();
                return false;
            } else {
                Properties.Settings.Default["pandaPath"] = path;
                Properties.Settings.Default.Save();
                return true;
            }
        }

        public static bool AmIPython2() {
            return Directory.EnumerateFiles(GetPandaPath() + "/python/", "python2?.dll").Any();
        }

        public static bool IsPandaPathValid(string path) {
            return !String.IsNullOrWhiteSpace(path) && File.Exists(GetPandaProgram(path, "multify"));
        }

        public static string GetPandaPath() {
            return (string)Properties.Settings.Default["pandaPath"];
        }

        public static string GetPythonExecutable() {
            string pandaPath = GetPandaPath();

            foreach (string executable in PythonExecutables) {
                string path = Path.Combine(pandaPath, "python", executable);

                if (File.Exists(path)) {
                    return path;
                }
            }

            return null;
        }

        public static bool HasPandaPath() {
            return IsPandaPathValid(GetPandaPath());
        }

        public static string GetPandaProgram(string path, string program) {
            return Path.Combine(path, "bin", program + ".exe");
        }

        public static string GetPandaProgram(string program) {
            return GetPandaProgram(GetPandaPath(), program);
        }

        public static string GetUserFolder(string profileName) {
            return Path.Combine(Directory.GetCurrentDirectory(), "profiles", profileName);
        }
    }
}
