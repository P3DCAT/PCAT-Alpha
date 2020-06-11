using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PCATApp {
    enum FileType {
        FILE,
        FOLDER
    }

    public partial class PCAT : Form {
        Dictionary<String, String> filePathMap = new Dictionary<String, String>();
        private Task indexTask = null;
        private Profile profile = null;
        private ContextMenuStrip menu;
        private MainFileView mainFileView;
        private MainFileView associatedFileView;

        public PCAT() {
            filePathMap.Add("PNGInput", string.Empty);
            filePathMap.Add("JPGInput", string.Empty);
            filePathMap.Add("RGBInput", string.Empty);
            filePathMap.Add("fileDir", string.Empty);
            filePathMap.Add("noExtInput", string.Empty);

            InitializeComponent();
        }

        public Profile GetProfile() {
            return profile;
        }

        public MainFileView GetMainFileView() {
            return mainFileView;
        }

        public MainFileView GetAssociatedFileView() {
            return associatedFileView;
        }

        private void OpenLabel_Click(object sender, EventArgs e) {
            if (mainFileTreeView.SelectedNode != null) {
                Process.Start(mainFileTreeView.SelectedNode.Name);
            }
        }

        private void RemoveLabel_Click(object sender, EventArgs e) {
            if (mainFileTreeView.SelectedNode != null) {
                mainFileTreeView.SelectedNode.Remove();
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e) {
            using (PreferencesWindow PrefWindow = new PreferencesWindow()) {
                PrefWindow.ShowDialog();
            }
        }

        /*
        * Note: 
        * Using this method can prove the dictionary feature a bit redundant when bit comes to having them as global variables.
        * Probably recommended to eventually instantiate the dictionary in the scope of the method below.
        * We could still keep the feature of the global dictionary for checking with selected files, however.
        */
        private void PNGSplit(object sender, EventArgs e) {
            this.openToolFileWindow();

            if (!filePathMap["PNGInput"].Equals(string.Empty) && !filePathMap["noExtInput"].Equals(string.Empty)) {
                Console.WriteLine(filePathMap["PNGInput"]);
                PNGSplit split = new PNGSplit(filePathMap["PNGInput"]);
                split.exportAlphaImage(filePathMap["fileDir"] + "\\" + filePathMap["noExtInput"] + "_a.png");
            } else {
                /*
                 * All code here will be ran when the user cancels out of the menu.
                 */
                Console.WriteLine("Filepath cannot be null!");
            }

        }

        /*
         * This method is only used to open up a new file window for tools such as PNG split, combine, etc.
         * Will not append those images to the list
         */
        private void openToolFileWindow() {
            //var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    //Get the path of specified file
                    filePathMap["PNGInput"] = openFileDialog.FileName;
                    filePathMap["noExtInput"] = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    filePathMap["fileDir"] = Path.GetDirectoryName(openFileDialog.FileName);
                }
            }

        }

        private void MainTreeViewPlsLoadProfilekthx(TreeView treeview) {

        }

        private void toolStrippngSplitButton_Click(object sender, EventArgs e) {
            PNGSplit(sender, e);

        }

        private void toolStripButton2_Click(object sender, EventArgs e) {
            this.openToolFileWindow();

            if (!filePathMap["JPGInput"].Equals(string.Empty) && !filePathMap["RGBInput"].Equals(string.Empty)) {
                PNGCombine combine = new PNGCombine(filePathMap["JPGInput"], filePathMap["RGBInput"]);
                combine.exportImage(filePathMap["fileDir"] + "\\" + filePathMap["noExtInput"] + "_o.png");
            } else {
                Console.WriteLine("Filepaths cannot be null!");
            }
        }

        private void fileFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            //var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    //Get the path of specified file
                    filePathMap["PNGInput"] = openFileDialog.FileName;
                    filePathMap["noExtInput"] = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    filePathMap["fileDir"] = Path.GetDirectoryName(openFileDialog.FileName);
                    //this.filePath = filePath;

                    //Read the contents of the file into a bitmap
                    Bitmap fs = new Bitmap(openFileDialog.FileName);
                    mainFileImageView.Image = fs;
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e) {
            BuildingIndexLabel.Hide();

            mainFileView = new MainFileView(this, mainFileTreeView, mainFilePviewButton, mainFileImageView, true);
            associatedFileView = new MainFileView(this, associatedFileTreeView, associatedFilePviewButton, associatedFileImageView, false);

            mainFileView.Load();
            associatedFileView.Load();
        }

        private void PNGCombineButtonOld_Click(object sender, EventArgs e) {
            Form dlg1 = new Form();
            dlg1.ShowDialog();
        }

        /*
         * Used to import file(s) into the TreeNode
         * NOT THE BASE FILES mind you!
         */
        private void fileFolderToolStripMenuItem1_Click(object sender, EventArgs e) {
            if (IsBuildingIndex()) {
                MessageBox.Show("You cannot import new files while the index is building!", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select files";
            dialog.Multiselect = true;
            dialog.ShowDialog();

            /*
            foreach (string filename in dialog.FileNames) {
                if (fileTreeView.Nodes.ContainsKey(filename)) {
                    fileTreeView.Nodes.RemoveByKey(filename);
                }

                TreeNode node = new TreeNode(Path.GetFileName(filename));
                node.Name = filename;
                node.Tag = FileType.FILE;
                fileTreeView.Nodes.Add(node);
            }
            */

            StartUpdateTextureMap();
        }

        /*
          Intent:
          1) open folder, a bulk of files, or one, 
          2) pick where to save the newly extracted files, 
          3) run extract code on those selected files with progress bar (output in folder previously stated in 2
          4) show dialog when done & ask if you want to open folder to saved directory
         */
        // thank you microsoft https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.openfiledialog.multiselect?view=netframework-4.8
        private void extractMultifilesToolStripMenuItem_Click(object sender, EventArgs e) {
            using (MFExtractWindow OutWindow = new MFExtractWindow()) {
                OutWindow.ShowDialog();
            }

        }

        /* 
         * We probably need to refactor this code to the MFBuildWindow code since it gets called before the window is shown.
         */
        private void buildMultifilesToolStripMenuItem_Click(object sender, EventArgs e) {

            using (MFBuildWindow OutWindow = new MFBuildWindow()) {
                OutWindow.ShowDialog();
            }
        }

        private void PNGSplitTextureButton_Click(object sender, EventArgs e) {
            PNGSplit(sender, e);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            var fileContent = string.Empty;
            //var filePath = string.Empty;

            using (AboutWindow AboutWindow = new AboutWindow()) {
                AboutWindow.ShowDialog();
            }
        }

        /*
         * Intent:
         * Open selected multifile(s), copy them to the project (or temp) folder, extract them in a new subfolder.
         * ./project1/resources/extractMF/
         * Unless there's a better approach for this...
         */
        private void ImportMultifileButton_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Multifiles";
            dialog.Filter = "Multifiles (*.mf)|*.MF";
            dialog.Multiselect = true;
            dialog.ShowDialog();

            /*
            string thisDirectory = Directory.GetCurrentDirectory();
            string tempFolder = Utils.GetTemporaryDirectory();

            //copy the files
            foreach (string filename in dialog.FileNames) {

                File.Copy(thisDirectory + filename, tempFolder + filename);


                if (fileTreeView.Nodes.ContainsKey(filename)) {
                    fileTreeView.Nodes.RemoveByKey(filename);
                }

                TreeNode node = new TreeNode(Path.GetFileName(filename));
                node.Name = filename;
                node.Tag = FileType.FILE;
                fileTreeView.Nodes.Add(node);
            }
            */

            StartUpdateTextureMap();
        }

        /*
         * Opens a open folder dialog to add folder(s) to the Tree
         * Dialog that displays when appending phase files/content to the list.
         */
        private void toolStripMenuItem3_Click(object sender, EventArgs e) {
            if (IsBuildingIndex()) {
                MessageBox.Show("You cannot import new folders while the index is building!", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FolderSelectDialog dialog = new FolderSelectDialog();
            dialog.Title = "Select multifile folders";
            dialog.Multiselect = true;
            dialog.ShowDialog();

            /*
            foreach (string directory in dialog.FileNames) {
                DirectoryInfo dir = new DirectoryInfo(directory);
                string directoryName = dir.FullName;
                Console.WriteLine("DirectoryName: " + directoryName);
                if (fileTreeView.Nodes.ContainsKey(directoryName)) {
                    //if(FilesAreEqual(new FileInfo(directoryName)))
                    fileTreeView.Nodes.RemoveByKey(directoryName);
                }

                fileTreeView.Nodes.Add(CreateDirectoryNode(dir));
            }*/

            StartUpdateTextureMap();
        }

        private void PNGCombineTextureButton_Click(object sender, EventArgs e) {
            toolStripButton2_Click(sender, e);
        }

        /**
         * BUILDING INDEX
         */
        private bool IsBuildingIndex() {
            return indexTask != null;
        }

        private void StartUpdateTextureMap(bool force) {
            if (!force && !autoBuildModelIndexToolStripMenuItem.Checked) {
                return;
            }

            if (profile == null) {
                MessageBox.Show("You have no profile set!", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (indexTask != null) {
                MessageBox.Show("The index is already building!", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            BuildingIndexLabel.Show();
            indexTask = Task.Run(() => UpdateTextureMap());
        }

        private void StartUpdateTextureMap() {
            StartUpdateTextureMap(false);
        }
        private void FinishUpdateTextureMap() {
            BuildingIndexLabel.Hide();
        }

        private void UpdateTextureMap() {
            // First things first: get our list of models that have not been indexed yet.
            List<string> unindexedModels = profile.GetUnindexedFilenames(mainFileTreeView.Nodes);

            // Index them and merge the results with our global texture map!
            profile.UpdateTextureMap(Utils.GetTextureMap(profile.basePath, unindexedModels));

            // Cleanup time...
            indexTask = null;
            Invoke(new Action(FinishUpdateTextureMap));
        }
        private void BuildBaseModelIndexButton_Click(object sender, EventArgs e) {
            if (IsBuildingIndex()) {
                MessageBox.Show("Cannot change base folder while index is still building.", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var msg = MessageBox.Show("The Index has been previously built before. Rebuild the model index?", "PCAT", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (msg != DialogResult.OK) {
                return;
            }

            StartUpdateTextureMap(true);
        }

        /*
         * SERIALIZING PROFILES
         */
        private void NewProject_Click(object sender, EventArgs e) {
            if (profile != null && MessageBox.Show("Your unsaved progress will be lost. Are you sure?", "PCAT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) {
                return;
            }

            NewProfileDialog dialog = new NewProfileDialog();
            dialog.ShowDialog();

            if (dialog.IsCancelled()) {
                return;
            }

            profile = new Profile(dialog.GetProfileName(), dialog.GetBaseFolder());
            LoadProfile(profile);
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e) {
            if (profile == null) {
                MessageBox.Show("Please start a new project first!", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "PCAT Profile|*.pcat";
            dialog.Title = "Save PCAT Profile";
            dialog.ShowDialog();

            if (String.IsNullOrWhiteSpace(dialog.FileName)) {
                return;
            }

            File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(profile));
            MessageBox.Show(String.Format("Saved PCAT profile to {0}!", dialog.FileName), "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void loadProjectToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PCAT Profile|*.pcat";
            dialog.Title = "Load PCAT Profile";
            dialog.ShowDialog();

            if (String.IsNullOrWhiteSpace(dialog.FileName)) {
                return;
            }

            Profile loadedProfile;

            try {
                loadedProfile = JsonConvert.DeserializeObject<Profile>(File.ReadAllText(dialog.FileName));
            } catch (Exception ex) {
                MessageBox.Show("Could not load profile.", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadProfile(loadedProfile);
        }

        public void LoadProfile(Profile profile) {
            this.profile = profile;
            mainFileView.Clear();
            associatedFileView.Clear();
            Utils.PopulateTreeView(mainFileTreeView, profile.baseContent, '\\');
        }

        private void autoBuildModelIndexToolStripMenuItem_Click(object sender, EventArgs e) {
            if (autoBuildModelIndexToolStripMenuItem.Checked) {
                StartUpdateTextureMap();
            }
        }

        private void UserFileSearchBox_TextChanged(object sender, EventArgs e) {
            //wip

        }

        private void CPTools_Click(object sender, EventArgs e) {

        }
    }
}
