using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace PCATApp {
    public class Profile {
        public string name { get; set; }
        public string basePath { get; set; }
        public List<string> baseContent { get; set; }
        public Dictionary<string, List<string>> bamToTextureMap { get; set; }

        [JsonIgnore]
        public Dictionary<string, List<string>> textureToBamMap { get; set; }

        public Profile(string name, string basePath, List<string> baseContent, Dictionary<string, List<string>> bamToTextureMap, Dictionary<string, List<string>> textureToBamMap) {
            this.name = name;
            this.basePath = basePath;
            this.baseContent = baseContent;
            this.bamToTextureMap = bamToTextureMap;
            this.textureToBamMap = textureToBamMap;
        }

        [JsonConstructor]
        public Profile(string name, string basePath, List<string> baseContent, Dictionary<string, List<string>> bamToTextureMap) :
            this(name, basePath, baseContent, bamToTextureMap, new Dictionary<string, List<string>>()) {
                UpdateReverseTextureMap();
        }

        public Profile(string name, string basePath) : this(name, basePath, new List<string>(), new Dictionary<string, List<string>>()) {
            BuildInitialFileList();
        }

        public void RecurseDirectory(string directory) {
            foreach (string file in Directory.GetFiles(directory)) {
                baseContent.Add(Utils.RemoveBasePath(file, basePath));
            }

            foreach (string file in Directory.GetDirectories(directory)) {
                RecurseDirectory(Path.Combine(directory, file));
            }
        }

        public void BuildInitialFileList() {
            this.baseContent.Clear();
            RecurseDirectory(basePath);
        }

        public void UpdateTextureMap(Dictionary<string, List<string>> updated) {
            foreach (KeyValuePair<string, List<string>> pair in updated) {
                bamToTextureMap[pair.Key] = pair.Value;
            }

            UpdateReverseTextureMap();
        }

        public void UpdateReverseTextureMap() {
            textureToBamMap.Clear();

            foreach (string bamFile in bamToTextureMap.Keys) {
                foreach (string textureFile in bamToTextureMap[bamFile]) {
                    if (!textureToBamMap.ContainsKey(textureFile)) {
                        textureToBamMap[textureFile] = new List<string>();
                    }

                    textureToBamMap[textureFile].Add(bamFile);
                }
            }
        }

        /*
         * Returns the list of filenames that must be indexed soon
         * in the output list
         */
        public void FillUnindexedFilenames(List<string> output, TreeNodeCollection collection) {
            foreach (TreeNode node in collection) {
                if (node.Tag.Equals(FileType.FOLDER)) {
                    FillUnindexedFilenames(output, node.Nodes);
                } else {
                    string relativePath = node.Name;
                    string lower = relativePath.ToLower();

                    if (lower.EndsWith(".bam") || lower.EndsWith(".egg")) {
                        // We use relative paths in our index.
                        if (!bamToTextureMap.ContainsKey(relativePath)) {
                            output.Add(relativePath);
                        }
                    }
                }
            }
        }

        public List<string> GetUnindexedFilenames(TreeNodeCollection collection) {
            List<string> unindexedModels = new List<string>();
            FillUnindexedFilenames(unindexedModels, collection);
            return unindexedModels;
        }

        public string ConvertRelativeToFull(string relativePath) {
            return Path.Combine(basePath, relativePath);
        }

        public string GetUserContentDir() {
            return Program.GetUserFolder(name);
        }
        public string GetUserContent(string relativePath) {
            return Path.Combine(GetUserContentDir(), relativePath);
        }

        public List<string> GetAssociatedFiles(string relativePath) {
            List<string> associatedFiles = new List<string>();

            if (bamToTextureMap.ContainsKey(relativePath)) {
                associatedFiles = bamToTextureMap[relativePath];
            } else if (textureToBamMap.ContainsKey(relativePath)) {
                associatedFiles = textureToBamMap[relativePath];
            }

            return associatedFiles;
        }
    }
}
