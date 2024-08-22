using System;
using System.Windows.Forms;

namespace PCATApp {
    internal class PreferencesWindow : Form
    {
        private Label MayaDialog;
        private ComboBox MayaVersion;
        private Label PandaLocDialog;
        private Button PandaFilePathButton;
        private Label p3dLabel;
        private FolderBrowserDialog folderBrowserDialog2;
        private FolderBrowserDialog folderBrowserDialog1;

        public PreferencesWindow() {
            Console.WriteLine("maybe this wirjs");
            this.InitializeComponent();

            }

        private void InitializeComponent()
        {
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.MayaDialog = new System.Windows.Forms.Label();
            this.MayaVersion = new System.Windows.Forms.ComboBox();
            this.PandaLocDialog = new System.Windows.Forms.Label();
            this.PandaFilePathButton = new System.Windows.Forms.Button();
            this.p3dLabel = new System.Windows.Forms.Label();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // MayaDialog
            // 
            this.MayaDialog.AutoSize = true;
            this.MayaDialog.Location = new System.Drawing.Point(6, 15);
            this.MayaDialog.Name = "MayaDialog";
            this.MayaDialog.Size = new System.Drawing.Size(185, 20);
            this.MayaDialog.TabIndex = 0;
            this.MayaDialog.Text = "Autodesk Maya Version";
            this.MayaDialog.Click += new System.EventHandler(this.label1_Click);
            // 
            // MayaVersion
            // 
            this.MayaVersion.FormattingEnabled = true;
            this.MayaVersion.Items.AddRange(new object[] {
            "Autodesk Maya 2016",
            "Autodesk Maya 2017",
            "Autodesk Maya 2018"});
            this.MayaVersion.Location = new System.Drawing.Point(131, 12);
            this.MayaVersion.Name = "MayaVersion";
            this.MayaVersion.Size = new System.Drawing.Size(234, 28);
            this.MayaVersion.TabIndex = 1;
            // 
            // PandaLocDialog
            // 
            this.PandaLocDialog.AutoSize = true;
            this.PandaLocDialog.Location = new System.Drawing.Point(6, 57);
            this.PandaLocDialog.Name = "PandaLocDialog";
            this.PandaLocDialog.Size = new System.Drawing.Size(147, 20);
            this.PandaLocDialog.TabIndex = 2;
            this.PandaLocDialog.Text = "Panda3D Location";
            this.PandaLocDialog.Click += new System.EventHandler(this.label2_Click);
            // 
            // PandaFilePathButton
            // 
            this.PandaFilePathButton.AccessibleName = "PandaFilePathButton";
            this.PandaFilePathButton.Location = new System.Drawing.Point(131, 52);
            this.PandaFilePathButton.Name = "PandaFilePathButton";
            this.PandaFilePathButton.Size = new System.Drawing.Size(75, 23);
            this.PandaFilePathButton.TabIndex = 3;
            this.PandaFilePathButton.Text = "Open Folder";
            this.PandaFilePathButton.UseVisualStyleBackColor = true;
            this.PandaFilePathButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // p3dLabel
            // 
            this.p3dLabel.AutoSize = true;
            this.p3dLabel.Location = new System.Drawing.Point(212, 57);
            this.p3dLabel.Name = "p3dLabel";
            this.p3dLabel.Size = new System.Drawing.Size(53, 20);
            this.p3dLabel.TabIndex = 4;
            this.p3dLabel.Text = "Unset";
            // 
            // folderBrowserDialog2
            // 
            this.folderBrowserDialog2.HelpRequest += new System.EventHandler(this.folderBrowserDialog2_HelpRequest);
            // 
            // PreferencesWindow
            // 
            this.ClientSize = new System.Drawing.Size(541, 395);
            this.Controls.Add(this.p3dLabel);
            this.Controls.Add(this.PandaFilePathButton);
            this.Controls.Add(this.PandaLocDialog);
            this.Controls.Add(this.MayaVersion);
            this.Controls.Add(this.MayaDialog);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::PCATApp.Properties.Settings.Default, "Preferences", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PreferencesWindow";
            this.Text = global::PCATApp.Properties.Settings.Default.Preferences;
            this.Load += new System.EventHandler(this.PrefWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void PrefWindow_Load(object sender, EventArgs e)
        {
            UpdatePathLabel();
        }

        private void UpdatePathLabel() {
            if (Program.HasPandaPath()) {
                p3dLabel.Text = Program.GetPandaPath();
            } else {
                p3dLabel.Text = "Unset";
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() != DialogResult.OK) {
                return;
            }

            if (Program.SetPandaPath(fbd.SelectedPath)) {
                MessageBox.Show("Your Panda3D path has been updated to " + fbd.SelectedPath + "!", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("Panda3D path could not be set, SDK tools missing...", "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdatePathLabel();
        }

        private void folderBrowserDialog2_HelpRequest(object sender, EventArgs e) {

            }
        }
}