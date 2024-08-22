using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;

namespace PCATApp {

    internal class PNGSplitWindow : Form {

        private static long WARNING_SIZE = 2147483648;
        private static Color COMPLETE_COLOR = Color.LimeGreen;
        Dictionary<String, String> filePathMap = new Dictionary<String, String>();


        private ProgressBar MFProgressBar;
        private Label DialogText;
        private Button OpenFileLocButton;
        private string encryptFilePath = null;


        private int currentIndex = 0;
        private int initialCount = 0;
        private bool cancelExit = false;
        private Button StartExtractButton;
        private Button AddMoreFiles;
        private TreeView folderView;
        private Process process;

        private ContextMenuStrip menu;
        private TreeNode selectedNode = null;

        public PNGSplitWindow() {
            filePathMap.Add("PNGInput", string.Empty);
            filePathMap.Add("JPGInput", string.Empty);
            filePathMap.Add("RGBInput", string.Empty);
            filePathMap.Add("fileDir", string.Empty);
            filePathMap.Add("noExtInput", string.Empty);
            this.InitializeComponent();
        }

        public static long GetDirectorySize(DirectoryInfo directory, int depth) {
            long size = 0;

            if (depth == 3) {
                return size;
            }

            try {
                foreach (FileInfo info in directory.GetFiles()) {
                    size += info.Length;
                }
            } catch {
                // AIDS
            }
            
            foreach (DirectoryInfo info in directory.GetDirectories()) {
                try {
                    size += GetDirectorySize(info, depth + 1);
                } catch {
                    // Coronavirus
                }
            }

            return size;
        }

        private void InitializeComponent() {
            this.MFProgressBar = new System.Windows.Forms.ProgressBar();
            this.DialogText = new System.Windows.Forms.Label();
            this.OpenFileLocButton = new System.Windows.Forms.Button();
            this.StartExtractButton = new System.Windows.Forms.Button();
            this.AddMoreFiles = new System.Windows.Forms.Button();
            this.folderView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // MFProgressBar
            // 
            this.MFProgressBar.Location = new System.Drawing.Point(594, 139);
            this.MFProgressBar.Name = "MFProgressBar";
            this.MFProgressBar.Size = new System.Drawing.Size(349, 36);
            this.MFProgressBar.TabIndex = 0;
            // 
            // DialogText
            // 
            this.DialogText.AutoSize = true;
            this.DialogText.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DialogText.Location = new System.Drawing.Point(656, 78);
            this.DialogText.Name = "DialogText";
            this.DialogText.Size = new System.Drawing.Size(232, 58);
            this.DialogText.TabIndex = 1;
            this.DialogText.Text = "Process paused.\r\nClick \"start\" to begin.";
            this.DialogText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OpenFileLocButton
            // 
            this.OpenFileLocButton.Enabled = false;
            this.OpenFileLocButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenFileLocButton.Location = new System.Drawing.Point(704, 252);
            this.OpenFileLocButton.Name = "OpenFileLocButton";
            this.OpenFileLocButton.Size = new System.Drawing.Size(131, 50);
            this.OpenFileLocButton.TabIndex = 2;
            this.OpenFileLocButton.Text = "Open File Location";
            this.OpenFileLocButton.UseVisualStyleBackColor = true;
            this.OpenFileLocButton.Click += new System.EventHandler(this.OpenFileLocButton_Click);
            // 
            // StartExtractButton
            // 
            this.StartExtractButton.Enabled = false;
            this.StartExtractButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartExtractButton.Location = new System.Drawing.Point(594, 199);
            this.StartExtractButton.Name = "StartExtractButton";
            this.StartExtractButton.Size = new System.Drawing.Size(131, 34);
            this.StartExtractButton.TabIndex = 3;
            this.StartExtractButton.Text = "Start";
            this.StartExtractButton.UseVisualStyleBackColor = true;
            this.StartExtractButton.Click += new System.EventHandler(this.StartExtractButton_Click);
            // 
            // AddMoreFiles
            // 
            this.AddMoreFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddMoreFiles.Location = new System.Drawing.Point(812, 199);
            this.AddMoreFiles.Name = "AddMoreFiles";
            this.AddMoreFiles.Size = new System.Drawing.Size(131, 34);
            this.AddMoreFiles.TabIndex = 4;
            this.AddMoreFiles.Text = "Add Folders";
            this.AddMoreFiles.UseVisualStyleBackColor = true;
            this.AddMoreFiles.Click += new System.EventHandler(this.AddMoreFiles_Click);
            // 
            // folderView
            // 
            this.folderView.ItemHeight = 20;
            this.folderView.Location = new System.Drawing.Point(23, 51);
            this.folderView.Name = "folderView";
            this.folderView.Size = new System.Drawing.Size(509, 624);
            this.folderView.TabIndex = 5;
            // 
            // PNGSplitWindow
            // 
            this.ClientSize = new System.Drawing.Size(1035, 752);
            this.Controls.Add(this.folderView);
            this.Controls.Add(this.AddMoreFiles);
            this.Controls.Add(this.StartExtractButton);
            this.Controls.Add(this.OpenFileLocButton);
            this.Controls.Add(this.DialogText);
            this.Controls.Add(this.MFProgressBar);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::PCATApp.Properties.Settings.Default, "Preferences", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PNGSplitWindow";
            this.Text = global::PCATApp.Properties.Settings.Default.Preferences;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MFBuildWindow_FormClosing);
            this.Load += new System.EventHandler(this.MFBuildWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        private void OpenFileLocButton_Click(object sender, EventArgs e) {
            OpenMultifile();
        }

        private void MFBuildWindow_FormClosing(object sender, FormClosingEventArgs e) {
            // Disable user closing the form, but no one else
            e.Cancel = cancelExit && (e.CloseReason == CloseReason.UserClosing);
        }

        private void MFBuildWindow_Load(object sender, EventArgs e) {
            currentIndex = 0;
            menu = new ContextMenuStrip();

            ToolStripMenuItem openLabel = new ToolStripMenuItem();
            openLabel.Text = "Open Folder Location";
            openLabel.Click += OpenLabel_Click;
            menu.Items.Add(openLabel);

            ToolStripSeparator separator = new ToolStripSeparator();
            menu.Items.Add(separator);

            ToolStripMenuItem removeLabel = new ToolStripMenuItem();
            removeLabel.Text = "Remove";
            removeLabel.Click += RemoveLabel_Click;
            menu.Items.Add(removeLabel);

            folderView.NodeMouseClick += FolderView_NodeMouseClick;
            folderView.KeyDown += FolderView_KeyDown;
        }

        private void FolderView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (folderView.SelectedNode != null)
                {
                    folderView.SelectedNode.Remove();
                    ToggleBuildButton();
                }
            }
        }

        private void OpenLabel_Click(object sender, EventArgs e)
        {
            if (selectedNode != null)
            {
                Process.Start(selectedNode.Name);
            }
        }

        private void FolderView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            selectedNode = e.Node;
        }

        private void RemoveLabel_Click(object sender, EventArgs e)
        {
            if (selectedNode != null)
            {
                selectedNode.Remove();
                ToggleBuildButton();
            }
        }

        private void ToggleBuildButton()
        {
            bool enabled = (GetNext() != null);
            StartExtractButton.Enabled = enabled;
            OpenFileLocButton.Enabled = enabled;
        }

        private void CloseNow() {
            cancelExit = false;
            Close();
        }

        private void MarkCurrentNodeAsDone() {
            foreach (TreeNode directoryNode in folderView.Nodes) {
                foreach (TreeNode multifileNode in directoryNode.Nodes) {
                    if (multifileNode.BackColor != COMPLETE_COLOR) {
                        multifileNode.BackColor = COMPLETE_COLOR;
                        return;
                    }
                }
            }
        }

        private Tuple<string, string> GetNext(bool force) {
            foreach (TreeNode directoryNode in folderView.Nodes) {
                foreach (TreeNode multifileNode in directoryNode.Nodes) {
                    if (force || multifileNode.BackColor != COMPLETE_COLOR) {
                        return new Tuple<string, string>(directoryNode.Name, multifileNode.Name);
                    }
                }
            }

            return null;
        }

        private Tuple<string, string> GetNext() {
            return GetNext(false);
        }

        private int GetRemainingCount() {
            int count = 0;

            foreach (TreeNode directoryNode in folderView.Nodes) {
                foreach (TreeNode multifileNode in directoryNode.Nodes) {
                    if (multifileNode.BackColor != COMPLETE_COLOR) {
                        count++;
                    }
                }
            }

            return count;
        }
        
        /*
         * Open file location
         * */
        private void OpenMultifile() {
            Tuple<string, string> nextFile = GetNext();

            if (nextFile == null) {
                nextFile = GetNext(false);
            }

            if (nextFile != null) {
                Process.Start("explorer.exe", nextFile.Item1); // Destination
                }
            }

        private void BuildComplete() {
            cancelExit = false;
            menu.Enabled = true;
            MarkCurrentNodeAsDone();
            OpenMultifile();
            MessageBox.Show("Build complete!", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateLabel() {
            Tuple<string, string> nextFile = GetNext();
            MFProgressBar.Value = currentIndex + 1;
            DialogText.Text = String.Format("Building {0}\n({1} out of {2})", Path.GetFileName(nextFile.Item2), currentIndex + 1, initialCount);
        }

        /*
         * START SPLITTIN DA FILES
         */
        private void ExtractMultifile() {
            Tuple<string, string> nextFile = GetNext();
            string destinationFolder = nextFile.Item1;
            string pngFile = nextFile.Item2;
            string pngName = Path.GetFullPath(pngFile); 
            string outputFolder = Path.GetFullPath(destinationFolder); // absolute path
            Invoke(new Action(UpdateLabel));


            Environment.CurrentDirectory = Directory.GetParent(pngFile).ToString();

            filePathMap["PNGInput"] = pngFile;
            filePathMap["noExtInput"] = Path.GetFileNameWithoutExtension(pngFile);
            filePathMap["fileDir"] = Path.GetDirectoryName(pngName); // or pngFile?

            if (!filePathMap["PNGInput"].Equals(string.Empty) && !filePathMap["noExtInput"].Equals(string.Empty)) {
                Console.WriteLine(filePathMap["PNGInput"]);
                PNGSplit split = new PNGSplit(filePathMap["PNGInput"]);
                split.exportAlphaImage(filePathMap["fileDir"] + "\\" + filePathMap["noExtInput"] + "_a.rgb");
            } else {
                /*
                 * All code here will be ran when the user cancels out of the menu.
                 */
                Console.WriteLine("Filepath cannot be null!");
            }

        }

        private void ExtractProcess_Exited(object sender, EventArgs e) {
            Tuple<string, string> nextFile = GetNext();

            if (process.ExitCode != 0) {
                if (nextFile != null) {
                    MessageBox.Show(String.Format("Phase file \"{0}\" could not be extracted! Build process failed with exit code {1}...", nextFile.Item2, process.ExitCode), "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Invoke(new Action(CloseNow));
                return;
            }
            
            if (GetRemainingCount() < 2) {
                Invoke(new Action(BuildComplete));
                return;
            }

            Invoke(new Action(MarkCurrentNodeAsDone));
            currentIndex++;
            ExtractMultifile();
        }

        private void StartExtractButton_Click(object sender, EventArgs e) {
            cancelExit = true;
            menu.Enabled = false;
            StartExtractButton.Enabled = false;
            new Thread(ExtractMultifile).Start();
        }

        private void AddMoreFiles_Click(object sender, EventArgs e) {
            if (!Program.HasPandaPath()) {
                MessageBox.Show("You haven't set the Panda3D SDK path yet!", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "PNG Files (*.png)|*.png|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK) {
                return;
                }

            
            FolderSelectDialog outputDialog = new FolderSelectDialog();
            DirectoryInfo parent = Directory.GetParent(openFileDialog.FileNames[0]);

            if (parent == null) {
                outputDialog.InitialDirectory = openFileDialog.FileNames[0];
            } else {
                outputDialog.InitialDirectory = parent.ToString();
            }

            outputDialog.Title = "Select the destination folder";
            outputDialog.Multiselect = false;

            if (!outputDialog.ShowDialog()) {
                return;
            }

            string destination = outputDialog.FileName;
            TreeNode[] nodes = folderView.Nodes.Find(destination, false);
            TreeNode destNode = null;

            if (nodes.Length == 0) {
                destNode = new TreeNode(String.Format("Destination: {0}", destination));
                destNode.Name = destination;
                destNode.ContextMenuStrip = menu;
            } else {
                destNode = nodes[0];
            }

            // Should we do this with the multifiles list or dynamically add like this?
            foreach (string mfFile in openFileDialog.FileNames) {

                /*
                // Check for duplicates
                TreeNode[] directories = folderView.Nodes.Find(directory, true);

                if (directories.Length > 0) {
                    // Only non-green nodes are duplicates
                    bool green = true;

                    foreach (TreeNode dirNode in directories) {
                        if (dirNode.BackColor != COMPLETE_COLOR) {
                            green = false;
                            break;
                        }
                    }

                    if (!green && MessageBox.Show(String.Format("The folder {0} is already specified as a build target. Are you sure?", directory), "PCAT", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No) {
                        continue;
                    }
                }
                */
                

                TreeNode mfNode = new TreeNode(mfFile);
                mfNode.Name = mfFile;
                mfNode.ContextMenuStrip = menu;
                destNode.Nodes.Add(mfNode);
            }

            if (nodes.Length == 0 && destNode.Nodes.Count > 0) {
                folderView.Nodes.Add(destNode);
            }

            currentIndex = 0;
            initialCount = GetRemainingCount();

            if (initialCount > 0) {
                StartExtractButton.Enabled = true;
                OpenFileLocButton.Enabled = true;
            }

            MFProgressBar.Maximum = initialCount;
        }

        private void PasswordBox_TextChanged(object sender, EventArgs e) {

            }

        private void BuildOptionsPanel_Enter(object sender, EventArgs e) {

        }
    }
}