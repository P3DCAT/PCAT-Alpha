using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace PCATApp {

    internal class MFOutputWindow : Form {

        private ProgressBar MFProgressBar;
        private Label ExtractionText;
        private Button OpenFileLocButton;

        private string[] multifiles;
        private int currentIndex = 0;
        private bool cancelExit = true;
        private Process process;

        public MFOutputWindow() {
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            this.MFProgressBar = new System.Windows.Forms.ProgressBar();
            this.ExtractionText = new System.Windows.Forms.Label();
            this.OpenFileLocButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MFProgressBar
            // 
            this.MFProgressBar.Location = new System.Drawing.Point(10, 102);
            this.MFProgressBar.Name = "MFProgressBar";
            this.MFProgressBar.Size = new System.Drawing.Size(349, 36);
            this.MFProgressBar.TabIndex = 0;
            // 
            // ExtractionText
            // 
            this.ExtractionText.AutoSize = true;
            this.ExtractionText.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExtractionText.Location = new System.Drawing.Point(30, 9);
            this.ExtractionText.Name = "ExtractionText";
            this.ExtractionText.Size = new System.Drawing.Size(417, 92);
            this.ExtractionText.TabIndex = 1;
            this.ExtractionText.Text = "Extracting <phase>\r\n([index] out of [length])";
            // 
            // OpenFileLocButton
            // 
            this.OpenFileLocButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenFileLocButton.Location = new System.Drawing.Point(105, 160);
            this.OpenFileLocButton.Name = "OpenFileLocButton";
            this.OpenFileLocButton.Size = new System.Drawing.Size(131, 34);
            this.OpenFileLocButton.TabIndex = 2;
            this.OpenFileLocButton.Text = "Open File Location";
            this.OpenFileLocButton.UseVisualStyleBackColor = true;
            this.OpenFileLocButton.Click += new System.EventHandler(this.OpenFileLocButton_Click);
            // 
            // MFOutputWindow
            // 
            this.ClientSize = new System.Drawing.Size(371, 222);
            this.Controls.Add(this.OpenFileLocButton);
            this.Controls.Add(this.ExtractionText);
            this.Controls.Add(this.MFProgressBar);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::PCATApp.Properties.Settings.Default, "Preferences", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MFOutputWindow";
            this.Text = global::PCATApp.Properties.Settings.Default.Preferences;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MFOutputWindow_FormClosing);
            this.Load += new System.EventHandler(this.MFOutputWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void SetMultifiles(string[] multifiles) {
            this.multifiles = multifiles;
            this.MFProgressBar.Maximum = multifiles.Length;
        }

        private void OpenFileLocButton_Click(object sender, EventArgs e) {
            OpenMultifile();
        }

        private void MFOutputWindow_FormClosing(object sender, FormClosingEventArgs e) {
            // Disable user closing the form, but no one else
            e.Cancel = cancelExit && (e.CloseReason == CloseReason.UserClosing);
        }

        private void MFOutputWindow_Load(object sender, EventArgs e) {
            currentIndex = 0;
            ExtractMultifile();
        }

        private void CloseNow() {
            cancelExit = false;
            Close();
        }

        private void OpenMultifile() {
            Process.Start(Path.GetDirectoryName(multifiles[currentIndex]));
        }

        private void OpenMultifileAndExit() {
            OpenMultifile();
            CloseNow();
        }

        private void UpdateLabel() {
            MFProgressBar.Value = currentIndex + 1;
            ExtractionText.Text = String.Format("Extracting {0}\n({1} out of {2})", Path.GetFileName(multifiles[currentIndex]), currentIndex + 1, multifiles.Length);
        }

        private void ExtractMultifile() {
            string multifile = multifiles[currentIndex];
            Invoke(new Action(UpdateLabel));

            process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = Program.GetPandaProgram("multify");
            startInfo.Arguments = String.Format("-x -f \"{0}\" -C \"{1}\"", multifile, Path.GetDirectoryName(multifile));

            process.EnableRaisingEvents = true;
            process.StartInfo = startInfo;
            process.Exited += ExtractProcess_Exited;

            try {
                process.Start();
            } catch {
                MessageBox.Show(String.Format("Phase file \"{0}\" could not be extracted, extraction process failed...", multifile), "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Invoke(new Action(CloseNow));
            }
        }

        private void ExtractProcess_Exited(object sender, EventArgs e) {
            if (process.ExitCode != 0) {
                MessageBox.Show(String.Format("Phase file \"{0}\" could not be extracted! Extract process failed with exit code {1}...", multifiles[currentIndex], process.ExitCode), "PCAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Invoke(new Action(CloseNow));
                return;
            }

            if (currentIndex >= multifiles.Length - 1) {
                Invoke(new Action(OpenMultifileAndExit));
                return;
            }

            currentIndex++;
            ExtractMultifile();
        }
    }
}