namespace VTSCore.Layers.Tracks.CCTV
{
    partial class TrackVideoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param video="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            this.timerUpdateAdjustment = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tbLatency = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbHeight = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFocusPlus = new System.Windows.Forms.Button();
            this.btnFocusMinus = new System.Windows.Forms.Button();
            this.btnZoomPlus = new System.Windows.Forms.Button();
            this.btnZoomMinus = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.videoPanel = new System.Windows.Forms.Panel();
            this.statusStrip1.SuspendLayout();
            this.controlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerUpdateAdjustment
            // 
            this.timerUpdateAdjustment.Interval = 1000;
            this.timerUpdateAdjustment.Tick += new System.EventHandler(this.timerUpdateAdjustment_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 431);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(32, 17);
            this.tsslStatus.Text = "状态";
            // 
            // tbLatency
            // 
            this.tbLatency.Location = new System.Drawing.Point(137, 347);
            this.tbLatency.Name = "tbLatency";
            this.tbLatency.Size = new System.Drawing.Size(60, 21);
            this.tbLatency.TabIndex = 11;
            this.tbLatency.Text = "0";
            this.tbLatency.TextChanged += new System.EventHandler(this.tbLatency_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 350);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "航线微调：";
            // 
            // tbHeight
            // 
            this.tbHeight.Location = new System.Drawing.Point(137, 302);
            this.tbHeight.Name = "tbHeight";
            this.tbHeight.Size = new System.Drawing.Size(60, 21);
            this.tbHeight.TabIndex = 9;
            this.tbHeight.Text = "0";
            this.tbHeight.TextChanged += new System.EventHandler(this.tbHeight_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 305);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "船高微调：";
            // 
            // btnFocusPlus
            // 
            this.btnFocusPlus.Location = new System.Drawing.Point(137, 238);
            this.btnFocusPlus.Name = "btnFocusPlus";
            this.btnFocusPlus.Size = new System.Drawing.Size(60, 30);
            this.btnFocusPlus.TabIndex = 7;
            this.btnFocusPlus.Text = "聚焦+";
            this.btnFocusPlus.UseVisualStyleBackColor = true;
            this.btnFocusPlus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnFocusPlus_MouseDown);
            this.btnFocusPlus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnFocusPlus_MouseUp);
            // 
            // btnFocusMinus
            // 
            this.btnFocusMinus.Location = new System.Drawing.Point(65, 238);
            this.btnFocusMinus.Name = "btnFocusMinus";
            this.btnFocusMinus.Size = new System.Drawing.Size(60, 30);
            this.btnFocusMinus.TabIndex = 6;
            this.btnFocusMinus.Text = "聚焦-";
            this.btnFocusMinus.UseVisualStyleBackColor = true;
            this.btnFocusMinus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnFocusMinus_MouseDown);
            this.btnFocusMinus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnFocusMinus_MouseUp);
            // 
            // btnZoomPlus
            // 
            this.btnZoomPlus.Location = new System.Drawing.Point(137, 202);
            this.btnZoomPlus.Name = "btnZoomPlus";
            this.btnZoomPlus.Size = new System.Drawing.Size(60, 30);
            this.btnZoomPlus.TabIndex = 5;
            this.btnZoomPlus.Text = "变倍+";
            this.btnZoomPlus.UseVisualStyleBackColor = true;
            this.btnZoomPlus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnZoomPlus_MouseDown);
            this.btnZoomPlus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnZoomPlus_MouseUp);
            // 
            // btnZoomMinus
            // 
            this.btnZoomMinus.Location = new System.Drawing.Point(65, 202);
            this.btnZoomMinus.Name = "btnZoomMinus";
            this.btnZoomMinus.Size = new System.Drawing.Size(60, 30);
            this.btnZoomMinus.TabIndex = 4;
            this.btnZoomMinus.Text = "变倍-";
            this.btnZoomMinus.UseVisualStyleBackColor = true;
            this.btnZoomMinus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnZoomMinus_MouseDown);
            this.btnZoomMinus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnZoomMinus_MouseUp);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(157, 82);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(40, 40);
            this.btnRight.TabIndex = 3;
            this.btnRight.Text = "右";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRight_MouseDown);
            this.btnRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnRight_MouseUp);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(65, 82);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(40, 40);
            this.btnLeft.TabIndex = 2;
            this.btnLeft.Text = "左";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnLeft_MouseDown);
            this.btnLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnLeft_MouseUp);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(111, 121);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(40, 40);
            this.btnDown.TabIndex = 1;
            this.btnDown.Text = "下";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDown_MouseDown);
            this.btnDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnDown_MouseUp);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(111, 41);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(40, 40);
            this.btnUp.TabIndex = 0;
            this.btnUp.Text = "上";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnUp_MouseDown);
            this.btnUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnUp_MouseUp);
            // 
            // controlPanel
            // 
            this.controlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.controlPanel.Controls.Add(this.tbLatency);
            this.controlPanel.Controls.Add(this.label2);
            this.controlPanel.Controls.Add(this.tbHeight);
            this.controlPanel.Controls.Add(this.label1);
            this.controlPanel.Controls.Add(this.btnFocusPlus);
            this.controlPanel.Controls.Add(this.btnFocusMinus);
            this.controlPanel.Controls.Add(this.btnZoomPlus);
            this.controlPanel.Controls.Add(this.btnZoomMinus);
            this.controlPanel.Controls.Add(this.btnRight);
            this.controlPanel.Controls.Add(this.btnLeft);
            this.controlPanel.Controls.Add(this.btnDown);
            this.controlPanel.Controls.Add(this.btnUp);
            this.controlPanel.Location = new System.Drawing.Point(533, -1);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(253, 432);
            this.controlPanel.TabIndex = 4;
            // 
            // videoPanel
            // 
            this.videoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.videoPanel.Location = new System.Drawing.Point(-1, -1);
            this.videoPanel.Name = "videoPanel";
            this.videoPanel.Size = new System.Drawing.Size(528, 432);
            this.videoPanel.TabIndex = 3;
            // 
            // TrackVideoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 453);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.videoPanel);
            this.Name = "TrackVideoForm";
            this.Text = "TrackVideoForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrackVideoForm_FormClosing);
            this.Load += new System.EventHandler(this.TrackVideoForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timerUpdateAdjustment;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.TextBox tbLatency;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnFocusPlus;
        private System.Windows.Forms.Button btnFocusMinus;
        private System.Windows.Forms.Button btnZoomPlus;
        private System.Windows.Forms.Button btnZoomMinus;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Panel videoPanel;

    }
}