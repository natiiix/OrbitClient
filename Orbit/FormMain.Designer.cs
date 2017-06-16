namespace Orbit
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            this.labelLatency = new System.Windows.Forms.Label();
            this.timerUpdateUI = new System.Windows.Forms.Timer(this.components);
            this.pictureBoxMap = new System.Windows.Forms.PictureBox();
            this.labelFPS = new System.Windows.Forms.Label();
            this.labelLocation = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMap)).BeginInit();
            this.SuspendLayout();
            // 
            // labelLatency
            // 
            this.labelLatency.AutoSize = true;
            this.labelLatency.BackColor = System.Drawing.Color.Transparent;
            this.labelLatency.ForeColor = System.Drawing.Color.White;
            this.labelLatency.Location = new System.Drawing.Point(100, 9);
            this.labelLatency.Name = "labelLatency";
            this.labelLatency.Size = new System.Drawing.Size(45, 13);
            this.labelLatency.TabIndex = 2;
            this.labelLatency.Text = "Latency";
            this.labelLatency.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LabelLatency_MouseClick);
            // 
            // timerUpdateUI
            // 
            this.timerUpdateUI.Tick += new System.EventHandler(this.TimerUpdateUI_Tick);
            // 
            // pictureBoxMap
            // 
            this.pictureBoxMap.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBoxMap.Location = new System.Drawing.Point(13, 13);
            this.pictureBoxMap.Name = "pictureBoxMap";
            this.pictureBoxMap.Size = new System.Drawing.Size(500, 400);
            this.pictureBoxMap.TabIndex = 3;
            this.pictureBoxMap.TabStop = false;
            this.pictureBoxMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictureBoxMap_MouseClick);
            // 
            // labelFPS
            // 
            this.labelFPS.AutoSize = true;
            this.labelFPS.BackColor = System.Drawing.Color.Transparent;
            this.labelFPS.ForeColor = System.Drawing.Color.White;
            this.labelFPS.Location = new System.Drawing.Point(12, 9);
            this.labelFPS.Name = "labelFPS";
            this.labelFPS.Size = new System.Drawing.Size(27, 13);
            this.labelFPS.TabIndex = 4;
            this.labelFPS.Text = "FPS";
            this.labelFPS.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LabelFPS_MouseClick);
            // 
            // labelLocation
            // 
            this.labelLocation.AutoSize = true;
            this.labelLocation.BackColor = System.Drawing.Color.Transparent;
            this.labelLocation.ForeColor = System.Drawing.Color.White;
            this.labelLocation.Location = new System.Drawing.Point(200, 9);
            this.labelLocation.Name = "labelLocation";
            this.labelLocation.Size = new System.Drawing.Size(48, 13);
            this.labelLocation.TabIndex = 5;
            this.labelLocation.Text = "Location";
            this.labelLocation.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LabelLocation_MouseClick);
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.labelLocation);
            this.Controls.Add(this.labelFPS);
            this.Controls.Add(this.labelLatency);
            this.Controls.Add(this.pictureBoxMap);
            this.DoubleBuffered = true;
            this.Name = "FormMain";
            this.Text = "Orbit";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelLatency;
        private System.Windows.Forms.Timer timerUpdateUI;
        private System.Windows.Forms.PictureBox pictureBoxMap;
        private System.Windows.Forms.Label labelFPS;
        private System.Windows.Forms.Label labelLocation;
    }
}

