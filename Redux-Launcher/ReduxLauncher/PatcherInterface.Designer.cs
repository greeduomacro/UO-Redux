namespace ReduxLauncher.Modules
{
    partial class PatcherInterface
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatcherInterface));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.Main_action_btn = new System.Windows.Forms.Button();
            this.Background = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Background)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.progressBar.Location = new System.Drawing.Point(14, 464);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(529, 23);
            this.progressBar.TabIndex = 0;
            this.progressBar.UseWaitCursor = true;
            // 
            // Main_action_btn
            // 
            this.Main_action_btn.AccessibleRole = System.Windows.Forms.AccessibleRole.Pane;
            this.Main_action_btn.BackColor = System.Drawing.Color.Gray;
            this.Main_action_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Main_action_btn.Location = new System.Drawing.Point(14, 493);
            this.Main_action_btn.Name = "Main_action_btn";
            this.Main_action_btn.Size = new System.Drawing.Size(531, 41);
            this.Main_action_btn.TabIndex = 3;
            this.Main_action_btn.Text = "Download";
            this.Main_action_btn.UseVisualStyleBackColor = false;
            this.Main_action_btn.Click += new System.EventHandler(this.Main_action_btn_Click);
            // 
            // Background
            // 
            this.Background.Location = new System.Drawing.Point(0, 0);
            this.Background.Name = "Background";
            this.Background.Size = new System.Drawing.Size(555, 547);
            this.Background.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Background.TabIndex = 4;
            this.Background.TabStop = false;
            // 
            // PatcherInterface
            // 
            this.AcceptButton = this.Main_action_btn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(555, 547);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.Main_action_btn);
            this.Controls.Add(this.Background);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(555, 547);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(555, 547);
            this.Name = "PatcherInterface";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Redux Patcher";
            this.TransparencyKey = System.Drawing.Color.Transparent;
            ((System.ComponentModel.ISupportInitialize)(this.Background)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button Main_action_btn;
        private System.Windows.Forms.PictureBox Background;
    }
}

