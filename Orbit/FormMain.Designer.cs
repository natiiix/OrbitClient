﻿namespace Orbit
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
            this.timerGetMap = new System.Windows.Forms.Timer(this.components);
            this.labelRenderTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMap)).BeginInit();
            this.SuspendLayout();
            // 
            // labelLatency
            // 
            this.labelLatency.AutoSize = true;
            this.labelLatency.Location = new System.Drawing.Point(12, 439);
            this.labelLatency.Name = "labelLatency";
            this.labelLatency.Size = new System.Drawing.Size(45, 13);
            this.labelLatency.TabIndex = 2;
            this.labelLatency.Text = "Latency";
            // 
            // timerUpdateUI
            // 
            this.timerUpdateUI.Tick += new System.EventHandler(this.timerUpdateUI_Tick);
            // 
            // pictureBoxMap
            // 
            this.pictureBoxMap.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBoxMap.Location = new System.Drawing.Point(13, 13);
            this.pictureBoxMap.Name = "pictureBoxMap";
            this.pictureBoxMap.Size = new System.Drawing.Size(500, 400);
            this.pictureBoxMap.TabIndex = 3;
            this.pictureBoxMap.TabStop = false;
            this.pictureBoxMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMap_MouseClick);
            // 
            // timerGetMap
            // 
            this.timerGetMap.Interval = 500;
            this.timerGetMap.Tick += new System.EventHandler(this.timerGetMap_Tick);
            // 
            // labelRenderTime
            // 
            this.labelRenderTime.AutoSize = true;
            this.labelRenderTime.Location = new System.Drawing.Point(12, 426);
            this.labelRenderTime.Name = "labelRenderTime";
            this.labelRenderTime.Size = new System.Drawing.Size(68, 13);
            this.labelRenderTime.TabIndex = 4;
            this.labelRenderTime.Text = "Render Time";
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.labelRenderTime);
            this.Controls.Add(this.pictureBoxMap);
            this.Controls.Add(this.labelLatency);
            this.DoubleBuffered = true;
            this.Name = "FormMain";
            this.Text = "Orbit";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelLatency;
        private System.Windows.Forms.Timer timerUpdateUI;
        private System.Windows.Forms.PictureBox pictureBoxMap;
        private System.Windows.Forms.Timer timerGetMap;
        private System.Windows.Forms.Label labelRenderTime;
    }
}

