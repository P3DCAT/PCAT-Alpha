using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Diagnostics;

namespace PCATApp {
    public static class Utils {
        private static char[] trimQuotes = new char[] { '"' };

        public static string GetTemporaryDirectory() {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        // https://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true
        public static void DeleteDirectory(string target_dir) {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files) {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs) {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        /*
         * Takes an egg file (i.e. a converted bam2egg file) and parses it, so we can easily locate texpaths.
         */
        public static List<string> ParseEggFile(string eggFile) {
            List<string> textures = new List<string>();
            bool beganTexture = false;

            foreach (string fLine in File.ReadLines(eggFile, Encoding.UTF8)) {
                string line = fLine.Trim();

                if (line.StartsWith("<VertexPool>")) {
                    break;
                }

                if (line.StartsWith("<Texture>")) {
                    beganTexture = true;
                } else if (beganTexture && line.StartsWith("\"")) {
                    string texture = line.Trim(trimQuotes);
                    textures.Add(texture);
                    beganTexture = false;
                }
            }

            return textures;
        }

        /*
         * Removes the root folder from a full drive path.
         * 
         * For example:
         * full_drive_path: C:/Users/User/phase_3/file.txt
         * root: C:/Users/User
         *
         * Returns: phase_3/file.txt
         */
        public static string RemoveBasePath(string full_drive_path, string root) {
            string path = full_drive_path.Substring(root.Length);
            return path.Trim(new char[] { Path.DirectorySeparatorChar });
        }



        public static void SearchFiles(String item) {
            //wip
        }


        /*
         * Expects a root directory, and list of relative paths (phase_3/maps/test.png)
         * Returns the texture map
         */
        public static Dictionary<string, List<string>> GetTextureMap(string root, List<string> paths) {
            Dictionary<string, List<string>> textures = new Dictionary<string, List<string>>();
            List<string> queuedBams = new List<string>();

            // First things first: parse Egg files our own way.
            foreach (string path in paths) {
                string lower = path.ToLower();

                if (lower.EndsWith(".bam")) {
                    queuedBams.Add(path);
                } else if (lower.EndsWith(".egg")) {
                    textures[path] = ParseEggFile(Path.Combine(root, path));
                } else {
                    textures[path] = new List<string>();
                }
            }

            // Try to get the Python executable from the Panda3D SDK...
            string python = Program.GetPythonExecutable();

            if (python == null) {
                MessageBox.Show("Python is not available to build the model index!", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return textures;
            }
            Console.WriteLine("am i python 2? " + Program.AmIPython2().ToString());
            string tempScript = "";
            // Save our script to the temporary folder!
            string tempFolder = GetTemporaryDirectory();
            if (!Program.AmIPython2()) {
                tempScript = Path.Combine(tempFolder, "list_bam_textures.py");
            } else {
                tempScript = Path.Combine(tempFolder, "list_bam_textures2.py");
            }

            if (!File.Exists(tempScript)) {
                File.WriteAllBytes(tempScript, Properties.Resources.list_bam_textures);
            }
            // Serialize our workload to JSON for the script
            string workload = JsonConvert.SerializeObject(queuedBams);

            // Start the Python script
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = python;
            startInfo.WorkingDirectory = root;
            startInfo.Arguments = "-OO \"" + tempScript + "\"";
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            // Send our workload to the script
            process.StandardInput.WriteLine(workload);
            process.StandardInput.Flush();

            // Read the script output and convert from JSON
            StringBuilder builder = new StringBuilder();

            if (!process.StandardOutput.EndOfStream) {
                builder.Append(process.StandardOutput.ReadToEnd());
                process.StandardOutput.DiscardBufferedData();
            }

            string output = builder.ToString();
            Dictionary<string, List<string>> bamTextures;

            process.WaitForExit();
            DeleteDirectory(tempFolder);

            try {
                bamTextures = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(output);
            } catch (Exception e) {
                MessageBox.Show("The Python script failed to build the model index.", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Console.WriteLine(e);
                return textures;
            }

            // Merge script output with our texture map
            foreach (KeyValuePair<string, List<String>> pair in bamTextures) {
                textures[pair.Key] = pair.Value;
            }

            return textures;
        }

        public static void PopulateTreeView(TreeView treeView, IEnumerable<string> paths, char pathSeparator) {
            TreeNode lastNode = null;
            string subPathAgg;

            foreach (string path in paths) {
                lastNode = null;
                subPathAgg = string.Empty;
                string[] split = path.Split(pathSeparator);
                int lastElement = split.Length - 1;

                for (int i = 0; i < split.Length; i++) {
                    string subPath = split[i];

                    if (!String.IsNullOrWhiteSpace(subPathAgg)) {
                        subPathAgg += Path.DirectorySeparatorChar;
                    }

                    subPathAgg += subPath;
                    TreeNode[] nodes = treeView.Nodes.Find(subPathAgg, true);

                    if (nodes.Length == 0) {
                        if (lastNode == null) {
                            lastNode = treeView.Nodes.Add(subPathAgg, subPath);
                        } else {
                            lastNode = lastNode.Nodes.Add(subPathAgg, subPath);
                        }

                        lastNode.Name = subPathAgg;

                        if (i == lastElement) {
                            lastNode.Tag = FileType.FILE;
                        } else {
                            lastNode.Tag = FileType.FOLDER;
                        }
                    } else {
                        lastNode = nodes[0];
                    }
                }
            }
        }
        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target) {
            foreach (DirectoryInfo dir in source.GetDirectories()) {
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            }

            foreach (FileInfo file in source.GetFiles()) {
                string path = Path.Combine(target.FullName, file.Name);

                // Basically, this will only copy files if it does not exist
                // OR if the source file is newer than the current file
                if (!File.Exists(path) || File.GetLastWriteTime(file.FullName) > File.GetLastWriteTime(path)) {
                    file.CopyTo(path);
                }
            }
        }

        /**
         * SERIALIZING
         */
        public static void SerializeNodes(TreeNode node, List<string> nodes) {
            if (node.Tag.Equals(FileType.FOLDER)) {
                foreach (TreeNode node2 in node.Nodes) {
                    SerializeNodes(node2, nodes);
                }
            } else if (node.Tag.Equals(FileType.FILE)) {
                nodes.Add(node.FullPath);
            }
        }

        public static List<string> SerializeItems(TreeView view) {
            List<string> nodes = new List<string>();

            foreach (TreeNode node in view.Nodes) {
                SerializeNodes(node, nodes);
            }

            return nodes;
        }

        public static void OpenInExplorer(string path) {
            if (File.Exists(path)) {
                Process.Start("explorer.exe", "/select, " + path);
            } else if (Directory.Exists(path)) {
                Process.Start("explorer.exe", path);
            }
        }

        public static void LaunchPview(string basePath, string relativePath) {
            if (!Program.HasPandaPath()) {
                MessageBox.Show("You haven't set the Panda3D SDK path yet!", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Program.GetPandaProgram("pview");
            startInfo.WorkingDirectory = basePath;
            startInfo.Arguments = relativePath;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
