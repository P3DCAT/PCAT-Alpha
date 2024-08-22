using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PCATApp {
    public partial class NewProfileDialog : Form {
        private string ProfileName = null;
        private string BaseFolder = null;

        public NewProfileDialog() {
            InitializeComponent();
            nameBox.KeyDown += NameBox_KeyDown;
        }

        private void NameBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                nextButton_Click(sender, e);
            }
        }

        public string GetProfileName() {
            return ProfileName;
        }

        public string GetBaseFolder() {
            return BaseFolder;
        }

        public bool IsCancelled() {
            return String.IsNullOrWhiteSpace(ProfileName) || String.IsNullOrWhiteSpace(BaseFolder);
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            Close();
        }

        private void nextButton_Click(object sender, EventArgs e) {
            string profileName = nameBox.Text.Trim();
            
            if (String.IsNullOrWhiteSpace(profileName)) {
                MessageBox.Show("Please set your project's name!", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string profileFolder = Program.GetUserFolder(profileName);

            if (Directory.Exists(profileFolder)) {
                MessageBox.Show("This project already exists!", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FolderSelectDialog dialog = new FolderSelectDialog();
            dialog.Title = "Select Base directory";
            dialog.Multiselect = false;
            dialog.ShowDialog();

            if (String.IsNullOrWhiteSpace(dialog.FileName)) {
                return;
            }

            this.ProfileName = profileName;
            this.BaseFolder = dialog.FileName;
            Close();
        }
    }
}
