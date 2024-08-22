using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace PCATApp {
    public class MainFileView {

        private PCAT_PanelWindow main;
        private TreeView fileView;
        private Button pviewButton;
        private PictureBox imagePreview;
        private bool updateAssociated;
        private string selectedPath = null;
        private string lastSelectedMedia = null;

        public MainFileView(
            PCAT_PanelWindow main, TreeView fileView,
            Button pviewButton, PictureBox imagePreview, bool updateAssociated) {
            this.main = main;
            this.fileView = fileView;
            this.pviewButton = pviewButton;
            this.imagePreview = imagePreview;
            this.updateAssociated = updateAssociated;
        }

        /**
         * Method that should be ran during your form load.
         * This will add the necessary event listeners to the GUI.
         */
        public void Load() {
            fileView.AfterSelect += FileView_AfterSelect;
            pviewButton.Click += PviewButton_Click;
        }

        /**
         * Clears the entire file view, removes all files and hides open files.
         */
        public void Clear() {
            fileView.Nodes.Clear();
            pviewButton.Visible = false;
            imagePreview.Image = null;
        }

        /**
         * Replaces the content of this file view with the contents of a list.
         */
        public void UpdateContent(List<string> files) {
            Clear();

            foreach (string file in files) {
                TreeNode node = new TreeNode(file);
                node.Name = file;
                node.Tag = FileType.FILE;
                string fullPath = main.GetProfile().ConvertRelativeToFull(file);

                if (!(File.Exists(fullPath))) {
                    node.ForeColor = Color.Red;
                }
                fileView.Nodes.Add(node);
            }
        }

        /* 
         * Using the file tree, if a user selects a file that's a supported image format, 
         * the file will become the image displayed on the image panel.
         */
        private void FileView_AfterSelect(object sender, TreeViewEventArgs e) {
            pviewButton.Visible = false;
            imagePreview.Image = null;

            string relativePath = e.Node.Name;
            string fullPath = main.GetProfile().ConvertRelativeToFull(relativePath);
            string userContentPath = main.GetProfile().GetUserContent(relativePath);
            string lower = relativePath.ToLower();
            this.selectedPath = relativePath;

            /*
             * https://social.msdn.microsoft.com/Forums/windows/en-US/790cd8be-0ba8-4f10-95a8-c88f1023d6e7/how-can-i-create-a-quotrightclick-menuquot-i-c?forum=winforms
             * this only shows on the node that's selected (im sleepy rn)
             */
            ContextMenuStrip mnu = new ContextMenuStrip();
            ToolStripMenuItem mnuAdd = new ToolStripMenuItem("Add User File");
            ToolStripMenuItem mnuReplace = new ToolStripMenuItem("Replace with File");
            ToolStripMenuItem mnuSrcOpenLoc = new ToolStripMenuItem("Open Source Location");
            ToolStripMenuItem mnuOpenLoc = new ToolStripMenuItem("Open Folder Location");

            //Assign event handlers
            mnuAdd.Click += MnuAdd_Click;
            mnuReplace.Click += MnuReplace_Click;
            mnuSrcOpenLoc.Click += MnuOpenLoc_Click;

            if (Directory.Exists(userContentPath) || File.Exists(userContentPath)) {
                mnuOpenLoc.Click += MnuSrcOpenLoc_Click;
            } else {
                mnuOpenLoc.Enabled = false;
            }

            mnuReplace.Enabled = !Directory.Exists(fullPath);

            //Add to main context menu
            mnu.Items.AddRange(new ToolStripItem[] { mnuAdd, mnuReplace, mnuSrcOpenLoc, mnuOpenLoc });
            fileView.ContextMenuStrip = mnu;

            if (e.Node.Tag.Equals(FileType.FOLDER)) {
                return;
            }

            if (lower.EndsWith(".jpg") || lower.EndsWith(".png") || lower.EndsWith(".jpeg") || lower.EndsWith(".bmp") || lower.EndsWith(".tiff")) {
                try {
                    imagePreview.Image = new Bitmap(fullPath);
                } catch(Exception exception) {
                    MessageBox.Show("We had a problem showing this image");
                }
            } else if (lower.EndsWith(".bam")) {
                pviewButton.Visible = true;
            }

            this.lastSelectedMedia = relativePath;

            if (updateAssociated) {
                List<string> associatedFiles = main.GetProfile().GetAssociatedFiles(relativePath);
                main.GetAssociatedFileView().UpdateContent(associatedFiles);
            }
        }

        /**
         * Context menu events
         */
        private void MnuAdd_Click(object sender, EventArgs e) {
            string targetPath = main.GetProfile().GetUserContent(this.selectedPath);
            string fullPath = main.GetProfile().ConvertRelativeToFull(this.selectedPath);

            if (Directory.Exists(fullPath)) {
                if (!Directory.Exists(targetPath)) {
                    Directory.CreateDirectory(targetPath);
                }

                Utils.CopyFilesRecursively(new DirectoryInfo(fullPath), new DirectoryInfo(targetPath));
            } else if (File.Exists(fullPath)) {
                string targetFolder = Path.GetDirectoryName(targetPath);

                if (!Directory.Exists(targetFolder)) {
                    Directory.CreateDirectory(targetFolder);
                }

                if (File.Exists(targetPath) && File.GetLastWriteTime(targetPath) <= File.GetLastWriteTime(fullPath)) {
                    MessageBox.Show("This file would replace your current work, so we've decided not to add it.", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else {
                    File.Copy(fullPath, targetPath);
                }
            } else {
                MessageBox.Show(String.Format("{0} does not exist.", fullPath));
                return;
            }

            Utils.OpenInExplorer(targetPath);
        }

        private void MnuReplace_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = String.Format("Replace {0} with...", Path.GetFileName(this.lastSelectedMedia));
            dialog.ShowDialog();

            if (String.IsNullOrWhiteSpace(dialog.FileName)) {
                return;
            }

            string targetPath = main.GetProfile().GetUserContent(this.selectedPath);
            string fullPath = main.GetProfile().ConvertRelativeToFull(this.selectedPath);

            if (File.Exists(targetPath)) {
                File.Delete(targetPath);
            }

            File.Copy(dialog.FileName, targetPath);
            Utils.OpenInExplorer(targetPath);
        }

        private void MnuOpenLoc_Click(object sender, EventArgs e) {
            // TODO:
            // directoryNode.Nodes[i].ForeColor = Color.DimGray;
            // directoryNode.Nodes[i].NodeFont = new Font(DefaultFont.FontFamily, DefaultFont.Size, FontStyle.Bold);
            string fullPath = main.GetProfile().ConvertRelativeToFull(this.selectedPath);
            Utils.OpenInExplorer(fullPath);
        }

        private void MnuSrcOpenLoc_Click(object sender, EventArgs e) {
            string fullPath = main.GetProfile().GetUserContent(this.selectedPath);
            Utils.OpenInExplorer(fullPath);
        }

        /**
         * Pview button event
         */
        private void PviewButton_Click(object sender, EventArgs e) {
            Utils.LaunchPview(main.GetProfile().basePath, this.lastSelectedMedia);
        }
    }
}
