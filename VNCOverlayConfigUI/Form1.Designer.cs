namespace VNCOverlayConfigUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnSave = new Button();
            btnStartService = new Button();
            txtPortNumber = new TextBox();
            btnStopService = new Button();
            lblStatus = new Label();
            btnRestartService = new Button();
            btnOverlayBasic = new Button();
            btnOverlayLoud = new Button();
            SuspendLayout();
            // 
            // btnSave
            // 
            btnSave.Location = new Point(118, 12);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 0;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnStartService
            // 
            btnStartService.Location = new Point(12, 41);
            btnStartService.Name = "btnStartService";
            btnStartService.Size = new Size(75, 23);
            btnStartService.TabIndex = 1;
            btnStartService.Text = "Start";
            btnStartService.UseVisualStyleBackColor = true;
            btnStartService.Click += btnStartService_Click;
            // 
            // txtPortNumber
            // 
            txtPortNumber.Location = new Point(12, 12);
            txtPortNumber.Name = "txtPortNumber";
            txtPortNumber.Size = new Size(100, 23);
            txtPortNumber.TabIndex = 2;
            txtPortNumber.TextChanged += txtPortNumber_TextChanged;
            // 
            // btnStopService
            // 
            btnStopService.Location = new Point(93, 41);
            btnStopService.Name = "btnStopService";
            btnStopService.Size = new Size(75, 23);
            btnStopService.TabIndex = 3;
            btnStopService.Text = "Stop";
            btnStopService.UseVisualStyleBackColor = true;
            btnStopService.Click += btnStopService_Click;
            // 
            // lblStatus
            // 
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.Location = new Point(12, 67);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(237, 27);
            lblStatus.TabIndex = 4;
            // 
            // btnRestartService
            // 
            btnRestartService.Location = new Point(174, 41);
            btnRestartService.Name = "btnRestartService";
            btnRestartService.Size = new Size(75, 23);
            btnRestartService.TabIndex = 5;
            btnRestartService.Text = "Restart";
            btnRestartService.UseVisualStyleBackColor = true;
            btnRestartService.Click += btnRestartService_Click;
            // 
            // btnOverlayBasic
            // 
            btnOverlayBasic.BackgroundImage = Properties.Resources.overlayBasicPreview;
            btnOverlayBasic.BackgroundImageLayout = ImageLayout.Zoom;
            btnOverlayBasic.Location = new Point(255, 12);
            btnOverlayBasic.Name = "btnOverlayBasic";
            btnOverlayBasic.Size = new Size(72, 64);
            btnOverlayBasic.TabIndex = 6;
            btnOverlayBasic.UseVisualStyleBackColor = true;
            btnOverlayBasic.Click += btnOverlayBasic_Click;
            // 
            // btnOverlayLoud
            // 
            btnOverlayLoud.BackgroundImage = Properties.Resources.overlayLoudPreview;
            btnOverlayLoud.BackgroundImageLayout = ImageLayout.Zoom;
            btnOverlayLoud.Location = new Point(333, 12);
            btnOverlayLoud.Name = "btnOverlayLoud";
            btnOverlayLoud.Size = new Size(72, 64);
            btnOverlayLoud.TabIndex = 7;
            btnOverlayLoud.UseVisualStyleBackColor = true;
            btnOverlayLoud.Click += btnOverlayLoud_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(462, 110);
            Controls.Add(btnOverlayLoud);
            Controls.Add(btnOverlayBasic);
            Controls.Add(btnRestartService);
            Controls.Add(lblStatus);
            Controls.Add(btnStopService);
            Controls.Add(txtPortNumber);
            Controls.Add(btnStartService);
            Controls.Add(btnSave);
            Name = "Form1";
            Text = "VNCOverlay Config";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSave;
        private Button btnStartService;
        private TextBox txtPortNumber;
        private Button btnStopService;
        private Label lblStatus;
        private Button btnRestartService;
        private Button btnOverlayBasic;
        private Button btnOverlayLoud;
    }
}
