using System;
using System.Windows.Forms;

namespace PCATApp {
    internal class AboutWindow : Form
    {
        private Label MayaDialog;
        private PictureBox PCATLogo;
        private Label label1;
        private Label Detail;
        private Label label2;
        private Label label3;
        private Label ALPHANOTE;
        private FolderBrowserDialog folderBrowserDialog1;

        public AboutWindow() {
            Console.WriteLine("maybe this wirjs");
            this.InitializeComponent();

            }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutWindow));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.MayaDialog = new System.Windows.Forms.Label();
            this.PCATLogo = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Detail = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ALPHANOTE = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PCATLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // MayaDialog
            // 
            this.MayaDialog.Location = new System.Drawing.Point(0, 0);
            this.MayaDialog.Name = "MayaDialog";
            this.MayaDialog.Size = new System.Drawing.Size(100, 23);
            this.MayaDialog.TabIndex = 2;
            // 
            // PCATLogo
            // 
            this.PCATLogo.Image = ((System.Drawing.Image)(resources.GetObject("PCATLogo.Image")));
            this.PCATLogo.InitialImage = null;
            this.PCATLogo.Location = new System.Drawing.Point(-4, 0);
            this.PCATLogo.Name = "PCATLogo";
            this.PCATLogo.Size = new System.Drawing.Size(764, 167);
            this.PCATLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PCATLogo.TabIndex = 1;
            this.PCATLogo.TabStop = false;
            this.PCATLogo.Click += new System.EventHandler(this.PCATLogo_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Impress BT", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(304, 204);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 34);
            this.label1.TabIndex = 3;
            this.label1.Text = "v1.0.0 ALPHA";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Detail
            // 
            this.Detail.AutoSize = true;
            this.Detail.Font = new System.Drawing.Font("Impress BT", 15F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Detail.Location = new System.Drawing.Point(109, 339);
            this.Detail.Name = "Detail";
            this.Detail.Size = new System.Drawing.Size(532, 75);
            this.Detail.TabIndex = 4;
            this.Detail.Text = "Dedicated to my really close hardworking friends who\r\ndevelop and create great th" +
    "ings for the Toontown community\r\n and all who aspire to be like them.";
            this.Detail.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Impress BT", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(182, 170);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(373, 34);
            this.label2.TabIndex = 5;
            this.label2.Text = "Created by Disyer and Loonatic";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Impress BT", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(124, 428);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(478, 50);
            this.label3.TabIndex = 6;
            this.label3.Text = "For support, please contact loonatic#1337 on Discord.\r\nDocumentation coming soon*" +
    "\r\n";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ALPHANOTE
            // 
            this.ALPHANOTE.AutoSize = true;
            this.ALPHANOTE.Font = new System.Drawing.Font("Impress BT", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ALPHANOTE.Location = new System.Drawing.Point(12, 238);
            this.ALPHANOTE.Name = "ALPHANOTE";
            this.ALPHANOTE.Size = new System.Drawing.Size(727, 72);
            this.ALPHANOTE.TabIndex = 7;
            this.ALPHANOTE.Text = resources.GetString("ALPHANOTE.Text");
            this.ALPHANOTE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AboutWindow
            // 
            this.ClientSize = new System.Drawing.Size(758, 487);
            this.Controls.Add(this.ALPHANOTE);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Detail);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PCATLogo);
            this.Controls.Add(this.MayaDialog);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::PCATApp.Properties.Settings.Default, "Preferences", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Font = new System.Drawing.Font("Impress BT", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AboutWindow";
            this.Text = global::PCATApp.Properties.Settings.Default.Preferences;
            this.Load += new System.EventHandler(this.PrefWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PCATLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void PrefWindow_Load(object sender, EventArgs e)
        {

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

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                MessageBox.Show("nice");
        }

        private void PCATLogo_Click(object sender, EventArgs e) {
        }
    }
    }