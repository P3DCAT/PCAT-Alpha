using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;

namespace PCATApp {

    internal class MFExtractWindow : Form {

        private static long WARNING_SIZE = 2147483648;
        private static Color COMPLETE_COLOR = Color.LimeGreen;

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
        private GroupBox BuildOptionsPanel;
        private GroupBox EncryptionOptionsGroup;
        private RadioButton EnterPasswordRadioButton;
        private RadioButton ChooseFileRadioButton;
        private TextBox PasswordBox;
        private Button PasswordTextFileOpen;
        private Label DirectoryLabel;
        private CheckBox EncryptFilesButton;
        private TreeNode selectedNode = null;

        public MFExtractWindow() {
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
            this.BuildOptionsPanel = new System.Windows.Forms.GroupBox();
            this.EncryptionOptionsGroup = new System.Windows.Forms.GroupBox();
            this.EnterPasswordRadioButton = new System.Windows.Forms.RadioButton();
            this.ChooseFileRadioButton = new System.Windows.Forms.RadioButton();
            this.PasswordBox = new System.Windows.Forms.TextBox();
            this.PasswordTextFileOpen = new System.Windows.Forms.Button();
            this.DirectoryLabel = new System.Windows.Forms.Label();
            this.EncryptFilesButton = new System.Windows.Forms.CheckBox();
            this.BuildOptionsPanel.SuspendLayout();
            this.EncryptionOptionsGroup.SuspendLayout();
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
            // BuildOptionsPanel
            // 
            this.BuildOptionsPanel.Controls.Add(this.EncryptionOptionsGroup);
            this.BuildOptionsPanel.Controls.Add(this.EncryptFilesButton);
            this.BuildOptionsPanel.Location = new System.Drawing.Point(569, 346);
            this.BuildOptionsPanel.Name = "BuildOptionsPanel";
            this.BuildOptionsPanel.Size = new System.Drawing.Size(420, 306);
            this.BuildOptionsPanel.TabIndex = 10;
            this.BuildOptionsPanel.TabStop = false;
            this.BuildOptionsPanel.Text = "Build Options";
            this.BuildOptionsPanel.Enter += new System.EventHandler(this.BuildOptionsPanel_Enter);
            // 
            // EncryptionOptionsGroup
            // 
            this.EncryptionOptionsGroup.Controls.Add(this.EnterPasswordRadioButton);
            this.EncryptionOptionsGroup.Controls.Add(this.ChooseFileRadioButton);
            this.EncryptionOptionsGroup.Controls.Add(this.PasswordBox);
            this.EncryptionOptionsGroup.Controls.Add(this.PasswordTextFileOpen);
            this.EncryptionOptionsGroup.Controls.Add(this.DirectoryLabel);
            this.EncryptionOptionsGroup.Location = new System.Drawing.Point(25, 64);
            this.EncryptionOptionsGroup.Name = "EncryptionOptionsGroup";
            this.EncryptionOptionsGroup.Size = new System.Drawing.Size(379, 80);
            this.EncryptionOptionsGroup.TabIndex = 6;
            this.EncryptionOptionsGroup.TabStop = false;
            this.EncryptionOptionsGroup.Visible = false;
            // 
            // EnterPasswordRadioButton
            // 
            this.EnterPasswordRadioButton.AutoSize = true;
            this.EnterPasswordRadioButton.Checked = true;
            this.EnterPasswordRadioButton.Location = new System.Drawing.Point(7, 19);
            this.EnterPasswordRadioButton.Name = "EnterPasswordRadioButton";
            this.EnterPasswordRadioButton.Size = new System.Drawing.Size(99, 17);
            this.EnterPasswordRadioButton.TabIndex = 4;
            this.EnterPasswordRadioButton.TabStop = true;
            this.EnterPasswordRadioButton.Text = "Enter Password";
            this.EnterPasswordRadioButton.UseVisualStyleBackColor = true;
            this.EnterPasswordRadioButton.CheckedChanged += new System.EventHandler(this.EnterPasswordRadioButton_CheckedChanged);
            // 
            // ChooseFileRadioButton
            // 
            this.ChooseFileRadioButton.AutoSize = true;
            this.ChooseFileRadioButton.Location = new System.Drawing.Point(7, 50);
            this.ChooseFileRadioButton.Name = "ChooseFileRadioButton";
            this.ChooseFileRadioButton.Size = new System.Drawing.Size(103, 17);
            this.ChooseFileRadioButton.TabIndex = 5;
            this.ChooseFileRadioButton.TabStop = true;
            this.ChooseFileRadioButton.Text = "Choose from File";
            this.ChooseFileRadioButton.UseVisualStyleBackColor = true;
            // 
            // PasswordBox
            // 
            this.PasswordBox.Location = new System.Drawing.Point(122, 18);
            this.PasswordBox.Name = "PasswordBox";
            this.PasswordBox.PasswordChar = '*';
            this.PasswordBox.Size = new System.Drawing.Size(251, 20);
            this.PasswordBox.TabIndex = 1;
            this.PasswordBox.TextChanged += new System.EventHandler(this.PasswordBox_TextChanged);
            // 
            // PasswordTextFileOpen
            // 
            this.PasswordTextFileOpen.Enabled = false;
            this.PasswordTextFileOpen.Location = new System.Drawing.Point(122, 45);
            this.PasswordTextFileOpen.Name = "PasswordTextFileOpen";
            this.PasswordTextFileOpen.Size = new System.Drawing.Size(64, 26);
            this.PasswordTextFileOpen.TabIndex = 2;
            this.PasswordTextFileOpen.Text = "Open File";
            this.PasswordTextFileOpen.UseVisualStyleBackColor = true;
            this.PasswordTextFileOpen.Click += new System.EventHandler(this.PasswordTextFileOpen_Click);
            // 
            // DirectoryLabel
            // 
            this.DirectoryLabel.AutoSize = true;
            this.DirectoryLabel.Location = new System.Drawing.Point(195, 52);
            this.DirectoryLabel.Name = "DirectoryLabel";
            this.DirectoryLabel.Size = new System.Drawing.Size(47, 13);
            this.DirectoryLabel.TabIndex = 3;
            this.DirectoryLabel.Text = "directory";
            this.DirectoryLabel.Visible = false;
            // 
            // EncryptFilesButton
            // 
            this.EncryptFilesButton.AutoSize = true;
            this.EncryptFilesButton.Location = new System.Drawing.Point(25, 41);
            this.EncryptFilesButton.Name = "EncryptFilesButton";
            this.EncryptFilesButton.Size = new System.Drawing.Size(163, 17);
            this.EncryptFilesButton.TabIndex = 0;
            this.EncryptFilesButton.Text = "Files have known password?";
            this.EncryptFilesButton.UseVisualStyleBackColor = true;
            this.EncryptFilesButton.CheckedChanged += new System.EventHandler(this.EncryptFilesButton_CheckedChanged);
            // 
            // MFExtractWindow
            // 
            this.ClientSize = new System.Drawing.Size(1035, 752);
            this.Controls.Add(this.BuildOptionsPanel);
            this.Controls.Add(this.folderView);
            this.Controls.Add(this.AddMoreFiles);
            this.Controls.Add(this.StartExtractButton);
            this.Controls.Add(this.OpenFileLocButton);
            this.Controls.Add(this.DialogText);
            this.Controls.Add(this.MFProgressBar);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::PCATApp.Properties.Settings.Default, "Preferences", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MFExtractWindow";
            this.Text = global::PCATApp.Properties.Settings.Default.Preferences;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MFBuildWindow_FormClosing);
            this.Load += new System.EventHandler(this.MFBuildWindow_Load);
            this.BuildOptionsPanel.ResumeLayout(false);
            this.BuildOptionsPanel.PerformLayout();
            this.EncryptionOptionsGroup.ResumeLayout(false);
            this.EncryptionOptionsGroup.PerformLayout();
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

        private string GetPassword() {
            if (!EncryptFilesButton.Checked) {
                return null;
                }

            if (EnterPasswordRadioButton.Checked) {
                return PasswordBox.Text;
                }
            else if (ChooseFileRadioButton.Checked) {
                if (encryptFilePath != null) {
                    return File.ReadAllText(encryptFilePath);
                    }
                }

            return null;
            }

        private void EncryptFilesButton_CheckedChanged(object sender, EventArgs e) {
            if (!EncryptFilesButton.Checked) {
                EncryptionOptionsGroup.Visible = false;
                }
            else {
                EncryptionOptionsGroup.Visible = true;
                }
            }

        private void EnterPasswordRadioButton_CheckedChanged(object sender, EventArgs e) {

            }

        private void PasswordTextFileOpen_Click(object sender, EventArgs e) {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK) {
                    return;
                    }
                DirectoryLabel.Visible = true;
                DirectoryLabel.Text = Path.GetFileName(openFileDialog.FileName);
                encryptFilePath = openFileDialog.FileName;
                //OutWindow.SetMultifiles(openFileDialog.FileNames);
                }
            }


        private void ChooseFileRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (ChooseFileRadioButton.Checked) {
                PasswordTextFileOpen.Enabled = true;
                PasswordBox.Enabled = false;
                }
            else {
                PasswordTextFileOpen.Enabled = false;
                PasswordBox.Enabled = true;
                }
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
         * START EXTRACTING* DA FILES
         */
        private void ExtractMultifile() {
            Tuple<string, string> nextFile = GetNext();
            string destinationFolder = nextFile.Item1;
            string multifileFile = nextFile.Item2;
            string multifileName = Path.GetFullPath(multifileFile); 
            string outputFolder = Path.GetFullPath(destinationFolder);
            string password = GetPassword();
            string arguments = "-x -f \"{0}\" -C \"{1}\"";
            Invoke(new Action(UpdateLabel));

            process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            Environment.CurrentDirectory = Directory.GetParent(multifileFile).ToString();
            //Console.WriteLine(Environment.CurrentDirectory);

            if (password != null) {
                arguments += " -e -p \"" + password + "\"";
            }


            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = Program.GetPandaProgram("multify");
            startInfo.Arguments = String.Format(arguments, multifileName, outputFolder);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            process.EnableRaisingEvents = true;
            process.StartInfo = startInfo;
            process.Exited += ExtractProcess_Exited;

            try {
                process.Start();
                string output;
                bool failed = false;

                while ((output = process.StandardOutput.ReadLine()) != null) {
                    if (output.Contains("Enter password:") || output.Contains("Error decrypting") || output.Contains("Unable to decrypt")) {
                        failed = true;
                        break;
                    }
                }

                if (failed) {
                    process.Kill();
                    MessageBox.Show(String.Format("Phase file \"{0}\" could not be extracted, password incorrect or not specified!", multifileFile), "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            } catch {
                MessageBox.Show(String.Format("Phase file \"{0}\" could not be extracted, build process failed...", multifileFile), "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Invoke(new Action(CloseNow));
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
            openFileDialog.Filter = "Multifiles (*.mf)|*.mf|All files (*.*)|*.*";
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