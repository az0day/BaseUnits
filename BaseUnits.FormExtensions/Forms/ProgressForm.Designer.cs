namespace BaseUnits.FormExtensions.Forms
{
    partial class ProgressForm
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
            this.TxtState = new System.Windows.Forms.Label();
            this.BarProgress = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // TxtState
            // 
            this.TxtState.AutoSize = true;
            this.TxtState.Dock = System.Windows.Forms.DockStyle.Top;
            this.TxtState.Location = new System.Drawing.Point(8, 8);
            this.TxtState.Name = "TxtState";
            this.TxtState.Padding = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.TxtState.Size = new System.Drawing.Size(103, 19);
            this.TxtState.TabIndex = 0;
            this.TxtState.Text = "Service is running...";
            // 
            // BarProgress
            // 
            this.BarProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.BarProgress.Location = new System.Drawing.Point(8, 27);
            this.BarProgress.Margin = new System.Windows.Forms.Padding(0);
            this.BarProgress.Name = "BarProgress";
            this.BarProgress.Size = new System.Drawing.Size(594, 23);
            this.BarProgress.TabIndex = 1;
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 58);
            this.Controls.Add(this.BarProgress);
            this.Controls.Add(this.TxtState);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Initializing ...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TxtState;
        private System.Windows.Forms.ProgressBar BarProgress;
    }
}